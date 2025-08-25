using UnityEngine;
using UnityEditor;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Framework.MotionFade;

public class FadeMotionGenerator
{
    [MenuItem("Live2D/Generate FadeMotionData From Selected Clips")]
    public static void GenerateFadeMotionData()
    {
        foreach (var obj in Selection.objects)
        {
            var clip = obj as AnimationClip;
            if (clip == null) continue;

            var path = AssetDatabase.GetAssetPath(clip);
            // Create fade motion data
            var fadeMotion = ScriptableObject.CreateInstance<CubismFadeMotionData>();
            fadeMotion.MotionName = path;

            // (Optional) set default fade times
            fadeMotion.FadeInTime = 0.5f;
            fadeMotion.FadeOutTime = 0.5f;

            // Save as asset
            path = path.Replace(".anim", "_fade.asset");
            AssetDatabase.CreateAsset(fadeMotion, path);
            Debug.Log($"Generated FadeMotionData: {path}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}