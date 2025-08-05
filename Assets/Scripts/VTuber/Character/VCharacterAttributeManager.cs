using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Character
{
    public class VCharacterAttributeManager
    {
        public Dictionary<string, VCharacterAttribute> Attributes { get; set; }
        
        public VCharacterAttributeManager()
        {
            Attributes = new Dictionary<string, VCharacterAttribute>();
        }

        public void AddAttribute(string name, VCharacterAttribute attribute)
        {
            Attributes.TryAdd(name, attribute);
            attribute.AttributeName = name;
            attribute.SetAttributeManager(this);
        }
        
        public bool TryGetAttribute(string name, out VCharacterAttribute attribute)
        {
            return Attributes.TryGetValue(name.Trim(), out attribute);
        }

        public bool TryGetAttributeValue(string name, out int value, out bool isPercentage)
        {
            if(Attributes.TryGetValue(name, out var attribute))
            {
                value = attribute.Value;
                isPercentage = attribute.IsPercentage;
                return true;
            }

            value = 0;
            isPercentage = false;
            return false;
        }
        
        public void ConvertToCharacterAttributes(Dictionary<string, VBattleAttribute> attributes)
        {
            foreach (var attribute in Attributes)
            {
                attribute.Value.ConvertToAttribute(attributes);
            }
        }

        public bool TestCost(VScheduleEvent e)
        {
            switch (e.CostType)
            {
                case VEventCostType.Stamina:
                    return TryGetAttribute("CAStamina", out var stamina) && stamina.PreviewAddTo(-e.Cost) >= 0;
                case VEventCostType.Money:
                    return TryGetAttribute("CAMoney", out var money) && money.PreviewAddTo(-e.Cost) >= 0;
            }

            return false;
        }

        public void ApplyCost(VScheduleEvent e)
        {
            switch (e.CostType)
            {
                case VEventCostType.Stamina:
                    if (TryGetAttribute("CAStamina", out var stamina))
                    {
                        stamina.AddTo(-e.Cost);
                    }

                    break;
                case VEventCostType.Money:
                    if (TryGetAttribute("CAMoney", out var money))
                    {
                        money.AddTo(-e.Cost);
                    }
                    break;
            }
        }
    }
}