using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.Core.AutoSave
{
    [InitializeOnLoad]
    static class VAutoSave
    {
        static VAutoSave()
        {
            EditorApplication.playModeStateChanged -= Save;
            EditorApplication.playModeStateChanged += Save;
        }

        private static void Save(PlayModeStateChange playModeStateChange)
        {
            if(playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                VDebug.Log("AutoSave - Saving Scenes and Assets");

                EditorSceneManager.SaveOpenScenes();
                AssetDatabase.SaveAssets();
            }
        }
    
    }
}