using UnityEngine;
using System.Collections.Generic;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace VTuber.Core.SE
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Audio/Sound Configuration")]
    public class SoundConfig : ScriptableObject
    {
        [SerializeField] private AudioClip[] soundEffects;
        public AudioClip[] SoundEffects => soundEffects;

#if UNITY_EDITOR
        [Header("Editor Settings")]
        [SerializeField] private string soundFolderPath = "Assets/Audio";
        [SerializeField] private bool includeSubfolders = true;

        [ContextMenu("Load Sound Effects")]
        public void LoadSoundEffects()
        {
            if (string.IsNullOrEmpty(soundFolderPath))
            {
                Debug.LogError("Sound folder path is not set!");
                return;
            }

            // 确保路径格式正确
            soundFolderPath = soundFolderPath.Replace('\\', '/');
            if (!soundFolderPath.StartsWith("Assets/"))
            {
                Debug.LogError("Sound folder path must start with 'Assets/'");
                return;
            }

            // 获取所有音频文件的路径
            List<string> audioPaths = new List<string>();
            GetAllAudioPaths(soundFolderPath, includeSubfolders, audioPaths);

            // 加载音频剪辑
            soundEffects = new AudioClip[audioPaths.Count];
            for (int i = 0; i < audioPaths.Count; i++)
            {
                soundEffects[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPaths[i]);
            }

            Debug.Log($"Loaded {soundEffects.Length} sound effects from {soundFolderPath}" +
                     (includeSubfolders ? " (including subfolders)" : ""));
            EditorUtility.SetDirty(this);
        }

        // 递归获取所有音频文件路径
        private void GetAllAudioPaths(string folderPath, bool recursive, List<string> result)
        {
            // 获取当前文件夹中的所有音频文件
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!result.Contains(path)) // 避免重复添加
                {
                    result.Add(path);
                }
            }

            // 如果需要递归，继续搜索子文件夹
            if (recursive)
            {
                // 使用AssetDatabase获取子文件夹
                string[] subFolders = AssetDatabase.GetSubFolders(folderPath);
                foreach (string subFolder in subFolders)
                {
                    GetAllAudioPaths(subFolder, true, result);
                }
            }
        }
#endif
    }
}