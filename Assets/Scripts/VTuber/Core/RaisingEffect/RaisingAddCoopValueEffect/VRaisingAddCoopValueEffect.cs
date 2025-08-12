using VTuber.BattleSystem.Effect;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddCoopValueEffect : VRaisingEffect
    {
        public uint cooperatorID;
        public VUpgradableValue<int> value;
        public VRaisingAddCoopValueEffect(VRaisingAddCoopValueEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            cooperatorID = configuration.cooperatorID;
            value = new VUpgradableValue<int>(int.Parse(parameter.Trim()), int.Parse(upgradedParameter.Trim()));
        }

        public override void ApplyEffect(VCharacter character)
        {
            base.ApplyEffect(character);
            character.CooperatorManager.GetCooperator(cooperatorID).AddCoopValue(value.Value);
        }
    }
}