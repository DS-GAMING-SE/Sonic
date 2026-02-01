using UnityEditor;

public static class GetGuidMenu
{
    [MenuItem("Assets/Copy Guid")]
    public static void GetGUID()
    {
        if (!Selection.activeObject)
            return;

        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        var guid = AssetDatabase.AssetPathToGUID(path);
        UnityEngine.GUIUtility.systemCopyBuffer = guid;
    }
}
