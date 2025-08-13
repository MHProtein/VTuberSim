using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffect : VRaisingEffect
    {
        private readonly string _attributeName;
        private readonly VUpgradableValue<int> _value;
        public VRaisingAddAttributeEffect(VRaisingAddAttributeEffectConfiguration configuration, int value, int upgradedValue) : base(configuration)
        {
            
            _attributeName = configuration.AbilityName;
            _value = new VUpgradableValue<int>(value, upgradedValue);
        }

        public override void ApplyEffect(VCharacter character)
        {
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                attribute.AddTo(_value.Value);
                VDebug.Log("Added " + _value + " To " + _attributeName);
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