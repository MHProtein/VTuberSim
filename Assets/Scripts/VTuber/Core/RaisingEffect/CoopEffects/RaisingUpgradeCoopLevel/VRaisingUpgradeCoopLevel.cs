using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeCoopLevel : VRaisingEffect
    {
        public uint cooperatorID;

        public VRaisingUpgradeCoopLevel(VRaisingUpgradeCoopLevelConfiguration configuration, string parameter) : base(
            configuration)
        {
            cooperatorID = uint.Parse(parameter);
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CooperatorManager.GetCooperator(cooperatorID).UpgradeLevel();
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return cooperatorID.ToString();
        }
    }
}