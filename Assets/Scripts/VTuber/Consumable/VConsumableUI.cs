using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableUI : VUIBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] public Image background;
        [SerializeField] public GameObject descriptionObject;
        [SerializeField] public TMP_Text description;
        [SerializeField] public TMP_Text consumableName;
        public VConsumable consumable;

        public void SetConsumable(VConsumable consumable)
        {
            this.consumable = consumable;
            icon.sprite = consumable.Icon;

            consumableName.text = consumable.Name;
            description.text = consumable.Description;
        }

        public void Clear()
        {
            consumable = null;
            consumableName.text = "";
            description.text = "";
            icon.sprite = null;
            descriptionObject.SetActive(false);
            gameObject.SetActive(false);
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