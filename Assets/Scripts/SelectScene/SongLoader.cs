using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongLoader : MonoBehaviour
{
    public SongButtonHolder songButtonHolder;
    public ChartMetadata cmd;
    public Text errorMsgText;

    void Start()
    {
        ReadDataFiles();
    }

    void ReadDataFiles()
    {
        if (!Directory.Exists("charts"))
        {
            errorMsgText.text = $"\"chart\" folder not found in the execution directory.\n({Directory.GetCurrentDirectory()})";
            errorMsgText.gameObject.SetActive(true);
            return;
        }

        string[] folders = Directory.GetDirectories("charts");

        foreach (string folder in folders)
        {
            string[] dataFiles = Directory.GetFiles(folder, "*.osu");
            ChartMetadata[] charts = new ChartMetadata[dataFiles.Length];

            if (charts.Length == 0) continue;

            for (int i = 0; i < dataFiles.Length; i++)
                charts[i] = new(folder, dataFiles[i]);

            songButtonHolder.AddSong(charts);
        }
        songButtonHolder.AddOver();

        songButtonHolder.enabled = true;
    }
}
