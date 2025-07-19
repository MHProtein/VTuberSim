using System;
using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VFollowerCountAttribute : VCharacterAttribute
    {
        public VFollowerCountAttribute(VCharacterAttributeConfiguration configuration, int initialValue, VRaisingEventKey eventKey = VRaisingEventKey.Default, int maxValue = Int32.MaxValue, int minValue = 0, bool isPercentage = false) : base(configuration, initialValue, eventKey, maxValue, minValue, isPercentage)
        {
            
        }

        public override KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            float conversionRate = 0;
            if (_attributeManager.TryGetAttributeValue("CAFollowerToViewerRatio",
                    out var value, out var isPercentage))
            {
                conversionRate = value / 100f;
            }
            
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                (VBattleAttribute)Activator.CreateInstance(BattleAttributeType,
                    (int)(value * conversionRate * 100f),
                    _configuration.isBattleAttributePercentage,
                    _configuration.battleEventKey,
                    _configuration.maxValue,
                    _configuration.minValue));
        }
    }
}