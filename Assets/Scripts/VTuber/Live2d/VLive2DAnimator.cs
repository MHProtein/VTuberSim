using System;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;
using Random = UnityEngine.Random;

public class VLive2DAnimator : VMonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Dictionary<VBattleEventKey, List<string>> motionClips;
    private Dictionary<VBattleEventKey, List<int>> motionClipHashes;
    private Dictionary<VBattleEventKey, FunctionWithADict> motionClipPlayers;

    protected override void Awake()
    {
        motionClipHashes = new Dictionary<VBattleEventKey, List<int>>();
        foreach (var clip in motionClips)
        {
            motionClipHashes.Add(clip.Key, clip.Value.ConvertAll(Animator.StringToHash));
        }
        
        motionClipPlayers = new Dictionary<VBattleEventKey, FunctionWithADict>();
        foreach (var clip in motionClipHashes)
        {
            motionClipPlayers.Add(clip.Key, (objects =>
            {
                var hash = clip.Value[Random.Range(0, motionClipHashes[clip.Key].Count)];
                animator.Play(hash);
                VDebug.Log("播放动画: " + hash);
            }));
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var motionClipHash in motionClipPlayers)
        {
            VBattleRootEventCenter.Instance.RegisterListener(motionClipHash.Key, motionClipHash.Value);
        }
    }

    protected override void OnDisable()
    {
        foreach (var motionClipHash in motionClipPlayers)
        {
            VBattleRootEventCenter.Instance.RemoveListener(motionClipHash.Key, motionClipHash.Value);
        }
    }
}