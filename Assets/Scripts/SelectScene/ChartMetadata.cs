using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ChartMetadata : Dictionary<string, Dictionary<string, object>>
{
    public string folder, resourcePath, chartPath;

    public ChartMetadata(string folder, string chartPath)
    {
        this.folder = folder;
        this.chartPath = chartPath;

        int index = folder.IndexOf("Resources/");
        if (index >= 0)
            resourcePath = folder[(index + "Resources/".Length)..];
        else
            resourcePath = folder;

        try
        {
            using StreamReader reader = new(chartPath);
            string sectionName = "";
            Add("", new());

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine().Trim();

                if (line.StartsWith("[HitObjects]"))
                    break;

                Match match = Regex.Match(line, @"\[(.*?)\]");
                if (match.Success)
                {
                    sectionName = match.Groups[1].Value;
                    TryAdd(sectionName, new());
                    continue;
                }

                if (line.Contains(":"))
                {
                    string[] parts = line.Split(new char[] { ':' }, 2);
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (int.TryParse(value, out int i)) this[sectionName].TryAdd(key, i);
                    else if (float.TryParse(value, out float f)) this[sectionName].TryAdd(key, f);
                    else this[sectionName].TryAdd(key, value);
                }

                if (sectionName == "Events" && !line.StartsWith("//"))
                {
                    match = Regex.Match(line, @"0,0,(?:""(.*?)""|(.*?))(?:,|\n|$)");
                    if (match.Success)
                    {
                        string bgPath = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                        this["Events"].TryAdd("__BG__", bgPath);
                    }
                }
            }
        }
        catch (IOException e)
        {
            Debug.LogError($"failed to parse path: {chartPath}\n{e.Message}");
        }
    }

    public ChartMetadata(string textContent)
    {
        string[] lines = textContent.Split(
            new[] { "\r\n", "\n" },
            StringSplitOptions.None
        );
        string sectionName = "";
        Add("", new());

        foreach (string line in lines)
        {
            if (line.StartsWith("[HitObjects]"))
                break;

            Match match = Regex.Match(line, @"\[(.*?)\]");
            if (match.Success)
            {
                sectionName = match.Groups[1].Value;
                TryAdd(sectionName, new());
                continue;
            }

            if (line.Contains(":"))
            {
                string[] parts = line.Split(new char[] { ':' }, 2);
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (int.TryParse(value, out int i)) this[sectionName].TryAdd(key, i);
                else if (float.TryParse(value, out float f)) this[sectionName].TryAdd(key, f);
                else this[sectionName].TryAdd(key, value);
            }

            if (sectionName == "Events")
            {
                match = Regex.Match(line, @"0,0,""?(.*?)""?");
                if (match.Success)
                    this["Events"].TryAdd("__BG__", match.Groups[1].Value);
            }
        }
    }

    public object TryGet(string section, string key)
    {
        if (!TryGetValue(section, out Dictionary<string, object> sectionDict))
        {
            Debug.LogError($"ChartMetadata: get section {section} failed");
            return $"<get section {section} failed>";
        }

        if (!sectionDict.TryGetValue(key, out object value))
        {
            Debug.LogError($"ChartMetadata: get key {section}: {key} failed");
            return $"<get key {section}: {key} failed>";
        }

        return value;
    }

    public string TryGetStr(string section, string key)
    {
        return TryGet(section, key).ToString();
    }

    public float TryGetFloat(string section, string key)
    {
        object obj = TryGet(section, key);
        if (obj is int i)
            return i;
        else if (obj is float f)
            return f;
        else
        {
            Debug.LogError($"ChartMetadata: key {section}: {key} ({obj}) is not float.");
            return float.NaN;
        }
    }

    public int TryGetInt(string section, string key)
    {
        object obj = TryGet(section, key);
        if (obj is int i)
            return i;
        else
        {
            Debug.LogError($"ChartMetadata: key {section}: {key} ({obj}) is not int.");
            return int.MinValue;
        }
    }

    public IEnumerator LoadAudioClipAsync(Action<AudioClip> onLoaded)
    {
        string audioUrl = Uri.EscapeUriString("file://" + Path.Combine(
            Application.dataPath,
            "..",
            resourcePath,
            TryGetStr("General", "AudioFilename")
        ));
        using UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.UNKNOWN);
        yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
        {
            Debug.LogError($"Error loading audio clip: {uwr.error}\nurl: {audioUrl}");
            onLoaded?.Invoke(null);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
            onLoaded?.Invoke(clip);
        }
    }

    [Obsolete]
    public TextAsset LoadTextAsset()
    {
        return Resources.Load<TextAsset>(
            resourcePath + "/" +
            Path.GetFileNameWithoutExtension(chartPath)
        );
    }

    private static readonly Dictionary<string, Texture2D> textureCache = new();
    private Texture2D texture = null;
    public Texture2D LoadBgTexture()
    {
        // return Resources.Load<Sprite>(
        //     resourcePath + "/" +
        //     Path.GetFileNameWithoutExtension(TryGetStr("Events", "__BG__"))
        // );

        if (texture != null) return texture;

        string bgFullPath = Path.Combine(resourcePath, TryGetStr("Events", "__BG__"));
        if (textureCache.TryGetValue(bgFullPath, out Texture2D tex)) return tex;

        FileStream fs = new(bgFullPath, FileMode.Open, FileAccess.Read);
        // fs.Seek(0, SeekOrigin.Begin); // 游标的操作，可有可无
        byte[] bytes = new byte[fs.Length];

        try
        {
            fs.Read(bytes, 0, bytes.Length); // 开始读取，这里最好用trycatch语句，防止读取失败报错
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fs.Close();

        tex = new(4, 4);
        if (tex.LoadImage(bytes))
        {
            // Debug.Log($"Load texture2d success: {tex.width} {tex.height}");
            texture = tex;
            textureCache.Add(bgFullPath, tex);
            return texture;
        }
        else
        {
            Debug.LogError("Load texture2d failed");
            return null;
        }
    }

    [Obsolete]
    public Sprite LoadBgSprite()
    {
        Texture2D texture = LoadBgTexture();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }

    public float GetHighScore()
    {
        return PlayerPrefs.GetFloat($"HighScore/title={TryGetStr("Metadata", "Title")}&artist={TryGetStr("Metadata", "Artist")}&creator={TryGetStr("Metadata", "Creator")}&version={TryGetStr("Metadata", "Version")}&mode={TryGetStr("General", "Mode")}", 0f);
    }

    public bool TrySetHighScore(float newScore, out float highScoreDiff)
    {
        string chartIdentifier = $"HighScore/title={TryGetStr("Metadata", "Title")}&artist={TryGetStr("Metadata", "Artist")}&creator={TryGetStr("Metadata", "Creator")}&version={TryGetStr("Metadata", "Version")}&mode={TryGetStr("General", "Mode")}";
        float highScore = PlayerPrefs.GetFloat(chartIdentifier, 0f);
        if (newScore > highScore)
        {
            // Debug.Log($"Highscore: {highScore} -> {newScore}\n chart id: {chartIdentifier}");
            highScoreDiff = newScore - highScore;
            PlayerPrefs.SetFloat(chartIdentifier, newScore);
            return true;
        }
        else
        {
            // Debug.Log($"Not Highscore: {highScore} -x-> {newScore} chart id: {chartIdentifier}");
            highScoreDiff = 0f;
            return false;
        }
    }

    public override string ToString()
    {
        string s = "";
        foreach (KeyValuePair<string, Dictionary<string, object>> section in this)
        {
            s += $"[{section.Key}]\n";
            foreach (KeyValuePair<string, object> kv in section.Value)
                s += $"\t{kv.Key}: {kv.Value}\n";
        }
        return s;
    }
}
