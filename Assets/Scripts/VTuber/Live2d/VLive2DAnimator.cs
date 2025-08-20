using Live2D.Cubism.Framework.Json;
using UnityEngine;
using Live2D.Cubism.Framework.Motion;

public class MotionImporter : MonoBehaviour
{
    // 在 Inspector 里拖入 .motion3.json (TextAsset)
    public TextAsset motionJson;

    private AnimationClip motionClip;

    void Start()
    {
        if (motionJson == null)
        {
            Debug.LogError("❌ 请在 Inspector 拖入 .motion3.json 文件！");
            return;
        }

        // 读取 motion3.json 并生成 AnimationClip
        var motion3 = CubismMotion3Json.LoadFrom(motionJson, false);
        motionClip = motion3.ToAnimationClip();

        if (motionClip != null)
        {
            Debug.Log("✅ Motion3.json 已转换为 Unity AnimationClip: " + motionClip.name);

            // 你可以把它直接加到 Animator 或者 CubismMotionController 来播放
            var controller = GetComponent<Live2D.Cubism.Framework.Motion.CubismMotionController>();
            if (controller != null)
            {
                controller.PlayAnimation(motionClip, isLoop: false);
            }
        }
    }
}