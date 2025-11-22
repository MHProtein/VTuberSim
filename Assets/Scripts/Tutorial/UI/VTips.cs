using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Consumable;
using VTuber.Core.Foundation;

namespace Tutorial.UI
{
    public class VTips : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image image;
        [SerializeField] private VClickDetectionPanel panel;
        [SerializeField] private Sprite defaultImage;
        
        public void SetTips(string title, string description, Sprite image)
        {
            this.description.text = description;
            this.image.sprite = image;
            if (image == null)
            {
                this.image.sprite = defaultImage;
            }
        }

        public void ShowTip()
        {
            if(description.text.IsNullOrWhitespace())
                return;
            ui.SetActive(true);
            
            panel.onClick = () => ui.SetActive(false);
        }
    }
}