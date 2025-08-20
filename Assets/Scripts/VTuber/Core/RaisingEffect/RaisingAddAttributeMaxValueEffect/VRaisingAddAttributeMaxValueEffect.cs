using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeMaxValueEffect : VRaisingEffect
    {
        private string _attributeName;
        private VUpgradableValue<int> _value;
        public VRaisingAddAttributeMaxValueEffect(VRaisingAddAttributeMaxValueEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _attributeName = configuration.attributeName;
            _value = new VUpgradableValue<int>(int.Parse(parameter.Trim()), int.Parse(upgradedParameter.Trim()));
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                attribute.AddMaxValue(_value.Value);   
            }
        }

        public override void Upgrade()
        {
            _value.Upgrade();
        }

        public override void DownGrade()
        {
            _value.Downgrade();
        }
    }
}