using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VCardUI : VUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [FormerlySerializedAs("Background")] [SerializeField] public Image background;
        [FormerlySerializedAs("Facade")] [SerializeField] public Image facade;
        
        [FormerlySerializedAs("Name")] [SerializeField] public TMP_Text name;
        [FormerlySerializedAs("Description")] [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text cost;

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}