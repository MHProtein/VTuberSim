using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.Consumable
{
    public class VConsumableUI : VUIBehaviour
    {
        [SerializeField] private Image icon;
        private VConsumable _consumable;
        
        public void SetConsumable(VConsumable consumable)
        {
            _consumable = consumable;
        }

        public bool HasConsumable()
        {
            return _consumable is not null;
        }
        
        public void UseConsumable()
        {
            _consumable.ApplyEffect();
        }
    }
}