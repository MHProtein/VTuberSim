using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;

namespace VTuber.Character.Attributes
{
    public class VStaminaAttribute : VCharacterAttribute
    {
        public VStaminaAttribute(VCharacterAttributeConfiguration configuration, int initialValue,
            VRaisingEventKey eventKey,
            int maxValue, int minValue) : base(configuration, initialValue, eventKey, maxValue, minValue)
        {
        }

        public override KeyValuePair<string, VBattleAttribute> ConvertToBattleAttribute()
        {
            return new KeyValuePair<string, VBattleAttribute>(_configuration.battleAttributeName,
                new VBattleStaminaAttribute(Value, VBattleEventKey.OnStaminaChange, false, _maxValue, _minValue));
        }
    }
}