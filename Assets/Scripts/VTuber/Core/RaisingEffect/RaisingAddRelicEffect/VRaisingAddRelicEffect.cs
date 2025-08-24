using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Foundation;
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

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CharacterRelicManager.AddRelic
                (VDataManager.Instance.CreateRelicByID(_relicId));
            
            VDebug.Log("hihi");
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