using UnityEditor;
using UnityEngine;

namespace VTuber.Editor.Utils
{
    public static class WindowExtension
    {
        public static void OpenWindow<T>(Vector2 size, GUIContent title) where T : EditorWindow
        {
            T window = EditorWindow.GetWindow<T>();
            window.minSize = size;
            window.titleContent = title;
            window.Show();
        }
    }
}