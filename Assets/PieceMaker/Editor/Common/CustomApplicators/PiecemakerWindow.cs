using UnityEditor;
using UnityEngine;
using System.Linq;
using System.IO;

public class PiecemakerWindow : PiecemakerWindowBase 
{
    public PiecemakerWindow()
    {
        if (!SerializaahNS.Serializers.Serializer.AvailableSerializers.Any(s => s is SerializaahNS.Serializers.UnityObjectSerializer))
            SerializaahNS.Serializers.Serializer.AvailableSerializers.Insert(0, new SerializaahNS.Serializers.UnityObjectSerializer());
    }

    [MenuItem("Window/Piecemaker/Piecemaker")]
	static void Init() 
	{
		var rect = new Rect();
		rect.width = 1030;
		rect.height = 700;
        rect.x = 100;
        rect.y = 100;

        var window = EditorWindow.GetWindow<PiecemakerWindow>(false, "Piecemaker", true);
        window.position = rect;

        window.LoadSettings("LastSettings.piece");
	}

	protected override void CleanDirectory(string path)
	{
		foreach (var fsEntry in Directory.GetFileSystemEntries(path).ToArray())
			AssetDatabase.DeleteAsset(fsEntry);
		AssetDatabase.Refresh();
	}

    protected override void SaveSettings(string path)
    {
        GenerateLastUsedPath(path);
        using (var streamWriter = File.CreateText(path))
            streamWriter.Write(SerializaahNS.Serializers.Serializer.Serialize(settings));
    }
}
