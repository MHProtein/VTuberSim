using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableUI : VUIBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] public Image background;
        public VConsumable consumable;
        
        public void SetConsumable(VConsumable consumable)
        {
            this.consumable = consumable;
            //icon.sprite = consumable.Icon;
        }

        public bool HasConsumable()
        {
            return consumable is not null;
        }
        
        public void UseConsumable()
        {
            consumable.ApplyEffect();
        }

        public void DiscardConsumable()
        {
            consumable.Discard();
            consumable = null;
        }

        public bool CanUse()
        {
            return consumable.CanApply();
        }
    }
}