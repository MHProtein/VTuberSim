using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VTuber.ScheduleSystem.UI
{
    /// <summary>
    /// 挂载在 Up/Down/Left/Right Indicator 游戏对象上
    /// 负责管理单个指示器的视觉表现
    /// </summary>
    public class VSchedulingConditionIndicatorUI : MonoBehaviour
    {
        [Header("UI 组件引用")] [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;
        [SerializeField] private TMP_Text hintText;

        /// <summary>
        /// 显示指示器并设置内容
        /// </summary>
        public void Show(string text, Color color, Sprite icon)
        {
            gameObject.SetActive(true);

            // 设置背景颜色
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }

            // 设置提示文字
            if (hintText != null)
            {
                hintText.text = text;
            }

            // 设置图标
            if (iconImage != null)
            {
                if (icon != null)
                {
                    iconImage.gameObject.SetActive(true);
                    iconImage.sprite = icon;
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 隐藏指示器
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}