using UnityEditor;
using UnityEngine;

public static class SimpleIntDataCreator
{
    [MenuItem("Tools/Create PlayerScore Asset")]
    public static void CreateAsset()
    {
        var asset = ScriptableObject.CreateInstance<SimpleIntData>();
        AssetDatabase.CreateAsset(asset, "Assets/PlayerScore.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        Debug.Log("Created Assets/PlayerScore.asset");
    }
}
