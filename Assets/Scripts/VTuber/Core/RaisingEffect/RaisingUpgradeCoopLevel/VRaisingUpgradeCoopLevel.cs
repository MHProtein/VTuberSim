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

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            character.CooperatorManager.GetCooperator(cooperatorID).UpgradeLevel();
        }
        
    }
}