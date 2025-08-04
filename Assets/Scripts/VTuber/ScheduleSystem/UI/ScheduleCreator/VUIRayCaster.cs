using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleUIHelper : VSingletonMonobehaviour<VScheduleUIHelper>
    {
        public RectTransform CanvasRect => canvasRect;
        public RectTransform ScheduleUIRect => scheduleUIRect;
        [SerializeField] GraphicRaycaster m_Raycaster;
        PointerEventData m_PointerEventData;
        [SerializeField] UnityEngine.EventSystems.EventSystem m_EventSystem;
        [SerializeField] RectTransform canvasRect;
        [SerializeField] RectTransform scheduleUIRect;

        public List<RaycastResult> RaycastFromMouse()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem) {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);
            return results;
        }
    }
}


