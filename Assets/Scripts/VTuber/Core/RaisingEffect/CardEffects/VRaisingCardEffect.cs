using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public enum VCardActionType
    {
        Add,
        Replace,
        Delete
    }

    public abstract class VRaisingCardEffect : VRaisingEffect
    {
        private readonly List<float> _rarityProbabilities;
        private readonly List<float> _upgradeProbabilities;


        public VRaisingCardEffect(VRaisingCardEffectConfiguration configuration) : base(configuration)
        {
            _rarityProbabilities = configuration.rarityProbabilities;
            _upgradeProbabilities = configuration.upgradeProbabilities;
        }

        public List<VCard> GetRandomCards(int count, VCardCondition condition, string liveType, VCharacter character)
        {
            var rarityCounts = new List<int>
            {
                0, 0, 0
            };
            var cardLibrary = character.CardLibrary.GetCards().Where(card => card.IsUnique).ToList();
            var cards = VDataManager.Instance.GetAllCardConfigurations().Where(card =>
            {
                if (!cardLibrary.Exists(c => c.configID == card.id) && card.rarity != VCardRarity.Basic &&
                    card.rarity != VCardRarity.Special &&
                    (card.liveType == liveType || card.liveType == "F"))
                {
                    if (condition is null)
                    {
                        rarityCounts[(int)card.rarity - 1]++;
                        return true;
                    }


                    if (condition.IsTrue(card))
                    {
                        rarityCounts[(int)card.rarity - 1]++;
                        return true;
                    }
                }

                return false;
            }).ToList();

            if (cards.Count == 0)
                return null;

            var totalRarityProb = 0f;
            for (var r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                    totalRarityProb += _rarityProbabilities[r];

            var perCardProbabilityByRarity = new float[3];
            for (var r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                {
                    var adjustedRarityProb = _rarityProbabilities[r] / totalRarityProb; // normalize
                    perCardProbabilityByRarity[r] = adjustedRarityProb / rarityCounts[r];
                }
                else
                {
                    perCardProbabilityByRarity[r] = 0f;
                }

            var selectedCards = new List<VCard>();

            var i = 0;
            while (i < count)
            {
                var probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (var j = 0; j < cards.Count; j++)
                {
                    totalProbability += perCardProbabilityByRarity[(int)cards[j].rarity - 1];
                    if (probability <= totalProbability)
                    {
                        var card = cards[j].CreateCard();
                        if (selectedCards.Find(vCard => vCard.configID == card.configID) != null)
                            break;
                        var upgradeProbability = Random.Range(0, 1.0f);
                        if (upgradeProbability <= _upgradeProbabilities[(int)card.Rarity - 1])
                            card.Upgrade(false);
                        selectedCards.Add(card);
                        ++i;
                        break;
                    }
                }
            }

            return selectedCards;
        }
    }
}