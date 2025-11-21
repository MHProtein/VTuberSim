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
        
        public void SetTips(string title, string description, Sprite image)
        {
            this.description.text = description;
            this.image.sprite = image;
        }

        public void ShowTip()
        {
            ui.SetActive(true);
            
            panel.onClick = () => ui.SetActive(false);
        }
    }
}