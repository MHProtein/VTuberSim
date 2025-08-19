using System;
using PrimeTween;
using TMPro;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Core;

namespace VTuber.BattleSystem.Core.KPIs.UI
{
    public class VKPIUI : VUIBehaviour
    {
        public uint ID { get; private set; }
        [SerializeField] TMP_Text text;
        private string _eventName;
        private int _requiredAmount;

        public void Initialize(VKPI kpi)
        {
            ID = kpi.ID;
            _eventName = kpi.EventName;
            _requiredAmount = kpi.RequiredAmount;
            
            text.color = Color.red;
        }
        
        public void SetText(int count, bool satisfied)
        {
            text.text = $"安排{_eventName}事件数: {count}/{_requiredAmount}";
            if (satisfied)
                text.color = Color.green;
            else
                text.color = Color.red;
            
            Tween.PunchScale(transform, Vector3.one * 1.3f, 0.3f);
        }
        
        public void ResetText()
        {
            text.text = $"安排{_eventName}事件数: 0/{_requiredAmount}";
            text.color = Color.red;
        }
    }
}