using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.KPIs.UI
{
    public class VKPIUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text text;
        private int _abilityIndex;
        private string _abilityName;
        private string _eventName;
        private int _requiredAmount;
        private int count;
        public uint ID { get; private set; }

        public void Initialize(VKPI kpi)
        {
            ID = kpi.ID;
            _eventName = kpi.EventName;
            _requiredAmount = kpi.RequiredAmount;
            _abilityName = kpi.AbilityName;
            _abilityIndex = kpi.AbilityIndex;

            if (kpi.EventType != VEventType.Stream) _abilityName = "";

            ResetText();
        }

        public void SetText(int count, bool satisfied)
        {
            if (count == this.count) return;
            this.count = count;
            text.text = $"安排{_abilityName}{_eventName}: {count}/{_requiredAmount}";
            if (satisfied)
                text.color = Color.green;
            else
                text.color = Color.red;

            Tween.PunchScale(transform, Vector3.one * 1.3f, 0.3f);
        }

        public void ResetText()
        {
            text.text = $"安排{_abilityName}{_eventName}: 0/{_requiredAmount}";
            text.color = Color.red;
            count = 0;
        }
    }
}