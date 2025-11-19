using UnityEngine;
using System.Collections.Generic;
using VTuber.Core.Foundation;

// 这个脚本应该被附加到指示器的父容器上 (ConditionIndicatorsContainer)
public class VIndicatorAnimationManager : VMonoBehaviour
{
    [Tooltip("透明度变化的最低值")]
    [Range(0f, 1f)]
    public float minAlpha = 0.3f;

    [Tooltip("透明度变化的最高值")]
    [Range(0f, 1f)]
    public float maxAlpha = 1.0f;

    [Tooltip("完成一次呼吸（从min到max再到min）所需的时间")]
    public float speed = 2.0f;

    // 存储所有子指示器的CanvasGroup组件
    private List<CanvasGroup> _indicatorCanvasGroups;

    protected override void Awake()
    {
        // 初始化列表
        _indicatorCanvasGroups = new List<CanvasGroup>();
        // 查找所有子对象（包括未激活的）上的CanvasGroup组件并添加到列表中
        GetComponentsInChildren<CanvasGroup>(true, _indicatorCanvasGroups);
    }

    protected override void OnEnable()
    {
        // 每次激活时，重置所有指示器的alpha值以同步动画
        SetAlpha(minAlpha);
    }

    protected override void UpdateImpl()
    {
        base.UpdateImpl();  // 使用Mathf.PingPong计算当前应该有的alpha值
        float range = maxAlpha - minAlpha;
        // 确保speed不为0，避免除零错误
        float currentAlpha = minAlpha + (speed > 0 ? Mathf.PingPong(Time.time * (range / speed), range) : 0);
        
        // 将计算出的alpha值应用到所有子指示器上
        SetAlpha(currentAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (_indicatorCanvasGroups == null) return;
        
        foreach (var cg in _indicatorCanvasGroups)
        {
            cg.alpha = alpha;
        }
    }
}