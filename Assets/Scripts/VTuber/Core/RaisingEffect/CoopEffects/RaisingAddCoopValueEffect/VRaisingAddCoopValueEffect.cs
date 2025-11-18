using System.Collections.Generic;
using VTuber.BattleSystem.Effect;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public class VRaisingAddCoopValueEffect : VRaisingEffect
    {
        public uint cooperatorID;
        public VUpgradableValue<int> value;

        public VRaisingAddCoopValueEffect(VRaisingAddCoopValueEffectConfiguration configuration, string parameter,
            string upgradedParameter) : base(configuration)
        {
            cooperatorID = configuration.cooperatorID;
            value = new VUpgradableValue<int>(int.Parse(parameter.Trim()), int.Parse(upgradedParameter.Trim()));
        }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CooperatorManager.GetCooperator(cooperatorID).AddCoopValue(value.Value);
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return value.Value.ToString();
        }

        protected override int GetPreviewValue(VCharacter character)
        {
            return value.Value;
        }
    }
}