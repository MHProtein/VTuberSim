using UnityEngine;
using UnityEditor;
using System.IO;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.Motion;

public class Live2DConverter
{
    [MenuItem("Live2D/Convert All motion3.json to .anim")]
    public static void ConvertAllMotion3Json()
    {
        // 搜索 Assets 下的所有 .motion3.json 文件
        string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });

        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!path.EndsWith(".motion3.json")) continue;

            // 读取 json
            TextAsset motionJson = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if (motionJson == null) continue;

            // 解析并生成 AnimationClip
            var motion3 = CubismMotion3Json.LoadFrom(motionJson, false);
            var clip = motion3.ToAnimationClip();

            if (clip != null)
            {
                // 保存为 .anim 文件（与原 motion3.json 同目录）
                string animPath = Path.ChangeExtension(path, "anim");
                AssetDatabase.CreateAsset(clip, animPath);

                Debug.Log($"✅ 转换完成: {path} → {animPath}");
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("转换完成", $"共转换 {count} 个 motion3.json → .anim", "OK");
    }
}