using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddRelicEffect : VRaisingEffect
    {
        public VRaisingAddRelicEffect(VRaisingAddRelicEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            RelicId = uint.Parse(parameter);
        }

        public uint RelicId { get; }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CharacterRelicManager.AddRelic
                (VDataManager.Instance.CreateRelicByID(RelicId));
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return VDataManager.Instance.Relics[RelicId].relicName;
        }
    }
}