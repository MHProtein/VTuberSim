using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public class VAddSpecifiedCardEffect : VRaisingEffect
    {
        public uint CardId => cardId;
        private uint cardId;
        public VAddSpecifiedCardEffect(VRaisingEffectConfiguration configuration, string parameter) : base(configuration)
        {
            cardId = uint.Parse(parameter);
        }

        public override void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict)
        {
            character.CardLibrary.AddCard(VDataManager.Instance.CreateCardByID(cardId));
        }

        public override void Upgrade()
        {
            
        }

        public override void DownGrade()
        {
            
        }

        public override string GetParameter()
        {
            return VDataManager.Instance.GetCardConfigurationByID(cardId).cardName;
        }
    }
}