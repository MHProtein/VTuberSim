using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRelicEffect : VRaisingEffect
    {
        private uint _relicId;
        public VRaisingAddRelicEffect(VRaisingAddRelicEffectConfiguration configuration) : base(configuration)
        {
            _relicId = configuration.relicId;
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            character.CharacterRelicManager.AddRelic(VResourcesManager.Instance.CreateRelicByID(_relicId));
        }
    }
}