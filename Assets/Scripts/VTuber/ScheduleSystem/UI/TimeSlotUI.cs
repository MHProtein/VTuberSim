using UnityEngine;
using UnityEngine.UI;
using VTuber.ScheduleSystem.Core;

public class TimeSlotUI : MonoBehaviour
{
    public Text slotLabel;
    public Button button;

    private int dayIndex;
    private TimeOfDay timeOfDay;

    public void Setup(int day, TimeOfDay time, System.Action<int, TimeOfDay> onClick)
    {
        dayIndex = day;
        timeOfDay = time;
        slotLabel.text = $"Day {day + 1} - {time}";

        button.onClick.AddListener(() => onClick?.Invoke(dayIndex, timeOfDay));
    }
}