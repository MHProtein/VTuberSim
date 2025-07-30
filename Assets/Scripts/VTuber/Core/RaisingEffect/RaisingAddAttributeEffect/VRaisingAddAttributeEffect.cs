using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core.RaisingEffect.VRaisingAddAttributeEffect
{
    public class VRaisingAddAttributeEffect : VRaisingEffect
    {
        public string attributeName;
        public int value;
        public VRaisingAddAttributeEffect(VRaisingEffectConfiguration configuration) : base(configuration)
        {
            
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            if(character.AttributeManager.TryGetAttribute(attributeName, out var attribute))
            {
                attribute.AddTo(value);
                VDebug.Log("Added " + value + " To " + attributeName);
            }
        }
    }
}