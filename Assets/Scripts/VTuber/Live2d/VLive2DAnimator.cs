using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Json;
using Live2D.Cubism.Framework.Motion;

public class MotionPlayer : MonoBehaviour
{
    private CubismMotionController motionController;
    private CubismModel model;

    private AnimationClip motionClip;

    void Start()
    {
        motionController = GetComponent<CubismMotionController>();
        model = GetComponent<CubismModel>();
        
        if (motionClip != null)
        {
            motionController.PlayAnimation(motionClip, isLoop: false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && motionClip != null)
        {
            motionController.PlayAnimation(motionClip, isLoop: true);
        }
    }
}