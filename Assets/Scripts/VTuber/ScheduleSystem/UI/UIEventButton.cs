using UnityEngine;
using UnityEngine.UI;
using VTuber.ScheduleSystem.Event;

public class UIEventButton : MonoBehaviour
{
    public Text nameText;
    public Button button;

    private ScheduleEventConfiguration _config;

    public void Setup(ScheduleEventConfiguration config, System.Action<ScheduleEventConfiguration> onClick)
    {
        _config = config;
        nameText.text = config.eventName;

        button.onClick.AddListener(() => onClick?.Invoke(_config));
    }
}