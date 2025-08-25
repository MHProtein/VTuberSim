using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public abstract class VRaisingCardEffect : VRaisingEffect
    {
        private readonly List<float> _rarityProbabilities;
        private readonly List<float> _upgradeProbabilities;
        

        public VRaisingCardEffect(VRaisingCardEffectConfiguration configuration) : base(configuration)
        {
            _rarityProbabilities = configuration.rarityProbabilities;
            _upgradeProbabilities = configuration.upgradeProbabilities;
        }

        public List<VCard> GetRandomCards(int count, VCardCondition condition)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            List<VCardConfiguration> cards = VDataManager.Instance.GetAllCardConfigurations().
                Where(card =>
                {
                    if (card.rarity != VCardRarity.Basic && card.rarity != VCardRarity.Special)
                    {
                        if (condition is null)
                        {
                            rarityCounts[(int) card.rarity - 1]++;
                            return true;
                        }
                        
                        
                        if (condition.IsTrue(card))
                        {
                            rarityCounts[(int) card.rarity - 1]++;
                            return true;
                        }
                    }
                    return false;
                }).ToList();
            
            if (cards.Count == 0)
                return null;
            
            float totalRarityProb = 0f;
            for (int r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                    totalRarityProb += _rarityProbabilities[r];

            float[] perCardProbabilityByRarity = new float[3];
            for (int r = 0; r < 3; r++)
            {
                if (rarityCounts[r] > 0)
                {
                    float adjustedRarityProb = _rarityProbabilities[r] / totalRarityProb; // normalize
                    perCardProbabilityByRarity[r] = adjustedRarityProb / rarityCounts[r];
                }
                else
                {
                    perCardProbabilityByRarity[r] = 0f;
                }
            }
            
            List<VCard> selectedCards = new List<VCard>();

            for (int i = 0; i < count; i++)
            {
                float probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (int j = 0; j < cards.Count; j++)
                {
                    totalProbability += perCardProbabilityByRarity[(int)cards[j].rarity - 1];
                    if (probability <= totalProbability)
                    {
                        var card = cards[j].CreateCard();
                        float upgradeProbability = Random.Range(0, 1.0f);
                        if(upgradeProbability <= _upgradeProbabilities[(int)card.Rarity - 1])
                            card.Upgrade(false);
                        selectedCards.Add(card);
                        break;
                    }
                }
            }
            return selectedCards;
        }
    }
}