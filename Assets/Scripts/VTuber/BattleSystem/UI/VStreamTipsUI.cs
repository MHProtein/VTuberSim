using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VStreamTipsUI : VUIBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private Image image;
        private List<Sprite> sprites;
        private int index;
        
        public void SetTips(List<Sprite> sprites)
        {
            this.sprites = sprites;
        }

        public void Show()
        {
            ui.SetActive(true);
            index = 0;
            image.sprite = sprites[index];
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            index++;
            if (index >= sprites.Count)
            {
                ui.SetActive(false);
                return;
            }
            image.sprite = sprites[index];
        }
    }
}