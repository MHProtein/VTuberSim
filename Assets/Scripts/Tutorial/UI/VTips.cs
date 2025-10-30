using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace Tutorial.UI
{
    public class VTips : VUIBehaviour
    {
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private Image image;
        
        public void SetTips(string title, string description, Sprite image)
        {
            this.title.text = title;
            this.description.text = description;
            this.image.sprite = image;
        }
    }
}