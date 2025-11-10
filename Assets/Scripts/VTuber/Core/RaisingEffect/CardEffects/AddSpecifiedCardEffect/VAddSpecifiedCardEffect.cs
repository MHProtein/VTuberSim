using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VAddSpecifiedCardEffect : VRaisingEffect
    {
        public VAddSpecifiedCardEffect(VRaisingEffectConfiguration configuration, string parameter) : base(
            configuration)
        {
            CardId = uint.Parse(parameter);
        }

        public uint CardId { get; }

        protected override void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CardLibrary.AddCard(VDataManager.Instance.CreateCardByID(CardId));
        }

        public override void Upgrade()
        {
        }

        public override void DownGrade()
        {
        }

        public override string GetParameter()
        {
            return VDataManager.Instance.GetCardConfigurationByID(CardId).cardName;
        }
    }
}