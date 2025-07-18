using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VTuber.ScheduleSystem.Event;

public class UIEventButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    private ScheduleEventConfiguration _config;
    private System.Action<ScheduleEventConfiguration> _onClick;

    public void Setup(ScheduleEventConfiguration config, System.Action<ScheduleEventConfiguration> onClick)
    {
        _config = config;
        _onClick = onClick;

        Debug.Log("按钮初始化");

        if (nameText != null)
        {
            Debug.Log("按钮名字赋值: " + config.eventName);
            nameText.text = config.eventName;
        }
        else
        {
            Debug.LogError("❌ nameText 未绑定！");
        }

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                Debug.Log("🖱️ 点击事件触发: " + config.eventName);
                _onClick?.Invoke(_config);
            });
        }
        else
        {
            Debug.LogError("❌ Button 组件未找到！");
        }
    }
}