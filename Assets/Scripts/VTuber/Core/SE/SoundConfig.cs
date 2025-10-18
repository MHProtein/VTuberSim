using UnityEngine;
using System.Collections.Generic;
using System.IO;
using VTuber.Core.Foundation;


#if UNITY_EDITOR
using UnityEditor;
#endif


namespace VTuber.Core.SE
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Audio/Sound Configuration")]
    public class SoundConfig : VScriptableObject
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

            // ȷ��·����ʽ��ȷ
            soundFolderPath = soundFolderPath.Replace('\\', '/');
            if (!soundFolderPath.StartsWith("Assets/"))
            {
                Debug.LogError("Sound folder path must start with 'Assets/'");
                return;
            }

            // ��ȡ������Ƶ�ļ���·��
            List<string> audioPaths = new List<string>();
            GetAllAudioPaths(soundFolderPath, includeSubfolders, audioPaths);

            // ������Ƶ����
            soundEffects = new AudioClip[audioPaths.Count];
            for (int i = 0; i < audioPaths.Count; i++)
            {
                soundEffects[i] = AssetDatabase.LoadAssetAtPath<AudioClip>(audioPaths[i]);
            }

            Debug.Log($"Loaded {soundEffects.Length} sound effects from {soundFolderPath}" +
                     (includeSubfolders ? " (including subfolders)" : ""));
            EditorUtility.SetDirty(this);
        }

        // �ݹ��ȡ������Ƶ�ļ�·��
        private void GetAllAudioPaths(string folderPath, bool recursive, List<string> result)
        {
            // ��ȡ��ǰ�ļ����е�������Ƶ�ļ�
            string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!result.Contains(path)) // �����ظ�����
                {
                    result.Add(path);
                }
            }

            // �����Ҫ�ݹ飬�����������ļ���
            if (recursive)
            {
                // ʹ��AssetDatabase��ȡ���ļ���
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