using System;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.RaisingEffect;

namespace VTuber.Core.UI
{
    public static class VCardActionUtils
    {
        private static Action<VCard> GetAddCardAction(VCharacter character)
        {
            return card => { character.CardLibrary.AddCard(card); };
        }

        private static Action<VCard> GetDeleteCardAction(VCharacter character)
        {
            return card => { character.CardLibrary.RemoveCard(card); };
        }

        private static Action<VCard> GetReplaceCardAction(VCharacter character, VCard selectedCard)
        {
            return card => { character.CardLibrary.ReplaceCard(card, selectedCard); };
        }

        public static Action<VCard> GetAction(VCardActionType actionType, VCharacter character,
            VCard replaceSelectedCard = null)
        {
            switch (actionType)
            {
                case VCardActionType.Add:
                    return GetAddCardAction(character);
                case VCardActionType.Replace:
                    return GetReplaceCardAction(character, replaceSelectedCard);
                case VCardActionType.Delete:
                    return GetDeleteCardAction(character);
            }

            return null;
        }
    }
}