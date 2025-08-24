using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRelicEffect : VRaisingEffect
    {
        private uint _relicId;
        public VRaisingAddRelicEffect(VRaisingAddRelicEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            _relicId = uint.Parse(parameter);
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CharacterRelicManager.AddRelic
                (VDataManager.Instance.CreateRelicByID(_relicId));
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return VDataManager.Instance.Relics[_relicId].relicName;
        }
    }
}