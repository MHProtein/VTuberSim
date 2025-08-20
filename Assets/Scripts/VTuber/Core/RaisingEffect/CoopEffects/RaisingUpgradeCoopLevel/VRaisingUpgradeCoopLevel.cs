using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingUpgradeCoopLevel : VRaisingEffect
    {
        public uint cooperatorID;
        public VRaisingUpgradeCoopLevel(VRaisingUpgradeCoopLevelConfiguration configuration) : base(configuration)
        {
            cooperatorID = configuration.cooperatorID;
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CooperatorManager.GetCooperator(cooperatorID).UpgradeLevel();
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
            
        }
    }
}