using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;

namespace VTuber.Store
{
    public class VStore
    {
        private int _refreshCount = 0;
        private readonly List<VStoreCardSlot> _cards = new List<VStoreCardSlot>();
        private readonly List<VStoreConsumableSlot> _consumables = new List<VStoreConsumableSlot>();

        private VCharacter _character;
        private readonly VStoreConfiguration _storeConfig;

        private List<float> CardRarityProbabilities => _storeConfig.cardRarityProbabilities;
        private List<float> CardRarityUpgradeProbabilities => _storeConfig.cardRarityUpgradeProbabilities;
        private List<float> ConsumableRarityProbabilities => _storeConfig.consumableRarityProbabilities;
        
        private readonly VStoreDiscardButton _discardButton;
        private readonly VStoreUpgradeButton _upgradeButton;
        private bool _isGlobalDiscount = false;
        private float _globalDiscount = 0.0f;
        
        public VStore(VStoreConfiguration storeConfig)
        {
            _storeConfig = storeConfig;
            _refreshCount = storeConfig.defaultRefreshCount;
            
            _discardButton = new VStoreDiscardButton(storeConfig.discardCardPrice, storeConfig.discardCardPriceIncrease);
            _upgradeButton = new VStoreUpgradeButton(storeConfig.upgradePrice, storeConfig.upgradePriceIncrease);
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreBeginRefresh, OnStoreBeginRefresh);
        }
        
        public void SetGlobalDiscount(float discount)
        {
            _isGlobalDiscount = true;
            _globalDiscount = discount;
        }

        private void OnStoreBeginRefresh(Dictionary<string, object> messagedict)
        { 
            _refreshCount--;
            _cards.Clear();
            _consumables.Clear();
            
            LoadItems();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreEndRefresh, new Dictionary<string, object>()
            {
                { "Character", _character },
                { "CardSlots", _cards },
                { "ConsumableSlots", _consumables },
                { "DiscardButton", _discardButton },
                { "UpgradeButton", _upgradeButton },
                { "RefreshCount", _refreshCount },
            });
        }

        public void EnterStore(VCharacter character)
        {
            _cards.Clear();
            _consumables.Clear();
            _character = character;

            LoadItems();

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEnterStore, new Dictionary<string, object>()
            {
                { "Character", character },
                { "CardSlots", _cards },
                { "ConsumableSlots", _consumables },
                { "DiscardButton", _discardButton },
                { "UpgradeButton", _upgradeButton },
                { "RefreshCount", _refreshCount },
            });
        }

        public void LoadItems()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreLoadItems, new Dictionary<string, object>()
            {
                {"Store", this},
            });
            
            LoadCards();
            LoadConsumables();
            
            _discardButton.SetPrice(_isGlobalDiscount, _globalDiscount);
            _upgradeButton.SetPrice(_isGlobalDiscount, _globalDiscount);
            
            _isGlobalDiscount = false;
        }
        
        public void LoadCards()
        {
            var cards = GetRandomCards(_storeConfig.cardCount, _character.LiveType);
            
            var slot = new VStoreCardSlot(true, false, _storeConfig.cardPrices[(int)(cards[0].Rarity - 1)], 
                Random.Range(_storeConfig.minDiscount, _storeConfig.maxDiscount) * (_isGlobalDiscount ? _globalDiscount : 1.0f), cards[0]);
            _cards.Add(slot);

            for (int i = 1; i < cards.Count; i++)
            {
                slot = new VStoreCardSlot(false, _isGlobalDiscount, _storeConfig.cardPrices[(int)(cards[i].Rarity - 1)], 
                    _globalDiscount, cards[i]);
                _cards.Add(slot);
            }
        }
        
        public void LoadConsumables()
        {
            var consumables = GetRandomConsumables(_storeConfig.consumableCount, _character.LiveType);
            
            var slot = new VStoreConsumableSlot(true, false, _storeConfig.consumablePrices[(int)consumables[0].Rarity], 
                Random.Range(_storeConfig.minDiscount, _storeConfig.maxDiscount) * (_isGlobalDiscount ? _globalDiscount : 1.0f), consumables[0]);
            _consumables.Add(slot);
            
            for (int i = 1; i < consumables.Count; i++)
            {
                slot = new VStoreConsumableSlot(false, _isGlobalDiscount, _storeConfig.consumablePrices[(int)(consumables[i].Rarity)], 
                    _globalDiscount, consumables[i]);
                _consumables.Add(slot);
            }
        }
        
        #region GetItems
        
        public List<VCard> GetRandomCards(int count, string liveType)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            var cardLibrary = _character.CardLibrary.GetCards().Where(card => card.IsUnique).ToList();
            List<VCardConfiguration> cards = VDataManager.Instance.GetAllCardConfigurations().
                Where(card => !cardLibrary.Exists(c => c.configID == card.id) && (card.liveType == liveType || card.liveType == "F") && card.rarity != VCardRarity.Basic && card.rarity != VCardRarity.Special).ToList();

            foreach (var card in cards)
            {
                rarityCounts[(int) card.rarity - 1]++;
            }
            
            if (cards.Count == 0)
                return null;
            
            float totalRarityProb = 0f;
            for (int r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                    totalRarityProb += CardRarityProbabilities[r];

            float[] perCardProbabilityByRarity = new float[3];
            for (int r = 0; r < 3; r++)
            {
                if (rarityCounts[r] > 0)
                {
                    float adjustedRarityProb = CardRarityProbabilities[r] / totalRarityProb; // normalize
                    perCardProbabilityByRarity[r] = adjustedRarityProb / rarityCounts[r];
                }
                else
                {
                    perCardProbabilityByRarity[r] = 0f;
                }
            }
            
            List<VCard> selectedCards = new List<VCard>();
            
            int i = 0;
            while (i < count)
            {
                float probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (int j = 0; j < cards.Count; j++)
                {
                    totalProbability += perCardProbabilityByRarity[(int)cards[j].rarity - 1];
                    if (probability <= totalProbability)
                    {
                        var card = cards[j].CreateCard();
                        if(selectedCards.Exists(c => c.configID == card.configID))
                            break;
                        float upgradeProbability = Random.Range(0, 1.0f);
                        if(upgradeProbability <= CardRarityUpgradeProbabilities[(int)card.Rarity - 1])
                            card.Upgrade(false);
                        selectedCards.Add(card);
                        ++i;
                        break;
                    }
                }
            }
            return selectedCards;
        }

        public List<VConsumable> GetRandomConsumables(int count, string liveType)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            List<VConsumableConfiguration> consumables = VDataManager.Instance.GetAllConsumableConfigurations().
                Where(consumable => (consumable.liveType == liveType || consumable.liveType == "F")).ToList();
            
            if (consumables.Count == 0)
                return null;
            
            foreach (var consumable in consumables)
            {
                rarityCounts[(int) consumable.rarity]++;
            }
            
            float totalRarityProb = 0f;
            for (int r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                    totalRarityProb += ConsumableRarityProbabilities[r];

            float[] perConsumableProbabilityByRarity = new float[3];
            for (int r = 0; r < 3; r++)
            {
                if (rarityCounts[r] > 0)
                {
                    float adjustedRarityProb = ConsumableRarityProbabilities[r] / totalRarityProb; // normalize
                    perConsumableProbabilityByRarity[r] = adjustedRarityProb / rarityCounts[r];
                }
                else
                {
                    perConsumableProbabilityByRarity[r] = 0f;
                }
            }
            
            List<VConsumable> selectedConsumables = new List<VConsumable>();

            int i = 0;
            while (i < count)
            {
                float probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (int j = 0; j < consumables.Count; j++)
                {
                    totalProbability += perConsumableProbabilityByRarity[(int)consumables[j].rarity];
                    if (probability <= totalProbability)
                    {
                        var consumable = consumables[j].CreateConsumable();
                        if (selectedConsumables.Exists(c => c.ConfigId == consumable.ConfigId))
                            break;
                        selectedConsumables.Add(consumable);
                        i++;
                        break;
                    }
                }
            }
            return selectedConsumables;
        }
        #endregion

        public void Reset()
        {
            _refreshCount = _storeConfig.defaultRefreshCount;
            _discardButton.Reset();
            _upgradeButton.Reset();
        }
    }
}