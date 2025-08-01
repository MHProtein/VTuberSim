using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddAttributeEffect : VRaisingEffect
    {
        private readonly string _attributeName;
        private readonly int _value;
        public VRaisingAddAttributeEffect(VRaisingAddAttributeEffectConfiguration configuration, int value) : base(configuration)
        {
            
            _attributeName = configuration.AbilityName;
            _value = value;
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            if(character.AttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                attribute.AddTo(_value);
                VDebug.Log("Added " + _value + " To " + _attributeName);
            }
        }
    }
}