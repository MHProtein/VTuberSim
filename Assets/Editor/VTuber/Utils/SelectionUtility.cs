using System.IO;
using UnityEditor;

namespace VTuber.Editor.Utils
{
    public static class SelectionUtility
    {
        public static string GetCurrentPath()
        {
            var path = "Assets";
            var obj = Selection.activeObject; //当前鼠标选中的 Object
            if (obj && AssetDatabase.Contains(obj))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!Directory.Exists(path)) //主要用来判断所选的是文件还是文件夹
                {
                    path = Path.GetDirectoryName(path); //如果是文件则获取对应文件夹的全名称
                }
            }
            return path;
        }
    }
}