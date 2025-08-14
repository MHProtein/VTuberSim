using System.Collections.Generic;

namespace VTuber.Consumable
{
    public class VConsumableManager
    {
        List<VConsumable> consumables = new List<VConsumable>();

        public void AddConsumable(VConsumable consumable)
        {
            consumables.Add(consumable);
        }
        
        public void RemoveConsumable(VConsumable consumable)
        {
            consumables.Remove(consumable);
        }
    }
}