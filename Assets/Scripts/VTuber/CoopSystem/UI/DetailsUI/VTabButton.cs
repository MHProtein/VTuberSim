using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.SE;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VTabButton : VUIBehaviour, IPointerClickHandler
    {
        [SerializeField] Image image;
        [SerializeField] VTabUI tab;
        Action<VTabButton> _onClick;
        private Color _color;
        public void Initialize(Action<VTabButton> onClick, Color clickedColor)
        {
            _onClick = onClick;
            _color = clickedColor;
            tab.gameObject.SetActive(false);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }

        public void Unselect()
        {
            image.color = Color.white;
            tab.gameObject.SetActive(false);
        }

        public void Select()
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            _onClick?.Invoke(this);
            image.color = _color;
            tab.gameObject.SetActive(true);
        }
    }
}