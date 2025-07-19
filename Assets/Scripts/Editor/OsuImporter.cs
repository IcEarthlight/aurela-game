using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, "osu")]
public class OsuImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string fileContent = System.IO.File.ReadAllText(ctx.assetPath);
        TextAsset textAsset = new(fileContent);
        ctx.AddObjectToAsset("TextAsset", textAsset);
        ctx.SetMainObject(textAsset);
    }
}
