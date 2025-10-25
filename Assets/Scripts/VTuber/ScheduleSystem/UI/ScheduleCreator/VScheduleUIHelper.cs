using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.ScheduleSystem.UI
{
    public class VScheduleUIHelper : VSingletonMonobehaviour<VScheduleUIHelper>
    {
        [SerializeField] private GraphicRaycaster m_Raycaster;
        private PointerEventData m_PointerEventData;
        [SerializeField] private UnityEngine.EventSystems.EventSystem m_EventSystem;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private RectTransform eventParent;
        [SerializeField] private RectTransform checkMarkParent;
        public RectTransform CanvasRect => canvasRect;
        public RectTransform EventParent => eventParent;
        public RectTransform CheckMarkParent => checkMarkParent;

        public List<RaycastResult> RaycastFromMouse()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            m_Raycaster.Raycast(m_PointerEventData, results);
            return results;
        }
    }
}