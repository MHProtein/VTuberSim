using UnityEngine;

namespace VTuber.ScheduleSystem.UI.ScheduleCreator
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BreathingLightEffect : MonoBehaviour
    {
        [Tooltip("透明度变化的最低值")]
        [Range(0f, 1f)]
        public float minAlpha = 0.3f;

        [Tooltip("透明度变化的最高值")]
        [Range(0f, 1f)]
        public float maxAlpha = 1.0f;

        [Tooltip("完成一次呼吸（从min到max再到min）所需的时间")]
        public float speed = 2.0f;

        private CanvasGroup canvasGroup;

        void Awake()
        {
            // 自动获取CanvasGroup组件
            canvasGroup = GetComponent<CanvasGroup>();
        }

        void OnEnable()
        {
            // 每次激活时，重置alpha到最小值，以保证动画同步
            if (canvasGroup != null)
            {
                canvasGroup.alpha = minAlpha;
            }
        }

        void Update()
        {
            if (canvasGroup == null) return;

            // 使用 Mathf.PingPong 来创建一个在 minAlpha 和 maxAlpha 之间来回变化的值
            float range = maxAlpha - minAlpha;
            float value = Mathf.PingPong(Time.time * (range / speed), range);
            canvasGroup.alpha = minAlpha + value;
        }
    }
}