using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.EventCenter;
using VTuber.Core.Managers;

namespace VTuber.Store
{
    public class VStoreButton
    {
        public int OriginalPrice { get; protected set; }
        public int TotalIncrease { get; protected set; }
        public int Price { get; protected set; }
        public int PriceIncrease { get; protected set; }
        public bool IsDiscount { get; protected set; }
        public float Discount { get; protected set; }
        public bool SoldOut { get; protected set; } = false;

        public VStoreButton(int originalPrice, int priceIncrease)
        {
            PriceIncrease = priceIncrease;
            OriginalPrice = originalPrice;
            this.Discount = Discount;
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreEndDeleteCard, OnStoreEndDeleteCard);
        }
        
        public void SetPrice(bool isDiscount, float discount)
        {
            Discount = discount;
            if(isDiscount)
                Price = (int)((OriginalPrice + TotalIncrease) * (1.0f - Discount));
            else
                Price = OriginalPrice + TotalIncrease;
        }

        private void OnStoreEndDeleteCard(Dictionary<string, object> messagedict)
        {
            var deleted = messagedict["Deleted"] as bool? ?? false;
            if (deleted)
            {
                SoldOut = true;
                TotalIncrease += PriceIncrease;
            }
        }
    }
    public class VStoreConsumableSlot : VStoreSlot
    {
        public VConsumable consumable;

        public VStoreConsumableSlot(bool isDiscount, int originalPrice, float discount, VConsumable consumable) : base(isDiscount, originalPrice, discount)
        {
            this.consumable = consumable;
        }
    }
    
    public class VStoreCardSlot : VStoreSlot
    {
        public VCard card;

        public VStoreCardSlot(bool isDiscount, int originalPrice, float discount, VCard card) : base(isDiscount, originalPrice, discount)
        {
            this.card = card;
        }

        public override void Buy(VCharacter character)
        {
            base.Buy(character);
            character.CardLibrary.AddCard(card);
        }
    }
    
    public class VStoreSlot
    {
        public readonly int originalPrice;
        public readonly int price;
        public readonly float discount;
        
        public VStoreSlot(bool isDiscount, int originalPrice, float discount)
        {
            this.originalPrice = originalPrice;
            if(isDiscount)
                price = (int)(originalPrice * (1.0f - discount));
            else
                price = originalPrice;
            this.discount = discount;
        }

        public bool Affordable(VCharacter character)
        {
            return character.AttributeManager.Attributes["CAMoney"].Value >= price;
        }
        
        public virtual void Buy(VCharacter character)
        {
            character.AttributeManager.Attributes["CAMoney"].AddTo(-price);
        }
    }
    
    public class VStore
    {
        private int refreshCount = 0;
        private List<VStoreCardSlot> _cards = new List<VStoreCardSlot>();
        private List<VStoreConsumableSlot> _consumables = new List<VStoreConsumableSlot>();

        private VCharacter _character;
        private VStoreConfiguration _storeConfig;

        private List<float> CardRarityProbabilities => _storeConfig.cardRarityProbabilities;
        private List<float> CardRarityUpgradeProbabilities => _storeConfig.cardRarityUpgradeProbabilities;
        private List<float> ConsumableRarityProbabilities => _storeConfig.consumableRarityProbabilities;
        
        
        public VStore(VStoreConfiguration storeConfig)
        {
            _storeConfig = storeConfig;
        }

        public void EnterStore(VCharacter character)
        {
            _character = character;
            LoadCards();
            LoadConsumables();
            
            
        }

        public void ExitStore()
        {
            _cards.Clear();
            _consumables.Clear();
        }
        
        public void RefreshStore()
        {
            _cards.Clear();
            _consumables.Clear();
            LoadCards();
            LoadConsumables();
            
            refreshCount--;
            if (refreshCount <= 0)
            {
                //todo: set not refreshable
            }
        }
        
        public void LoadCards()
        {
            var cards = GetRandomCards(4);
            
            var slot = new VStoreCardSlot(true, _storeConfig.cardPrices[(int)(cards[0].Rarity - 1)], 
                Random.Range(_storeConfig.minDiscount, _storeConfig.maxDiscount), cards[0]);
            _cards.Add(slot);

            for (int i = 1; i < cards.Count; i++)
            {
                slot = new VStoreCardSlot(false, _storeConfig.cardPrices[(int)(cards[i].Rarity - 1)], 
                    0.0f, cards[i]);
                _cards.Add(slot);
            }
        }
        
        public void LoadConsumables()
        {
            var consumables = GetRandomConsumables(4);
            
            var slot = new VStoreConsumableSlot(true, _storeConfig.cardPrices[(int)consumables[0].Rarity], 
                Random.Range(_storeConfig.minDiscount, _storeConfig.maxDiscount), consumables[0]);
            _consumables.Add(slot);
            
            for (int i = 1; i < consumables.Count; i++)
            {
                slot = new VStoreConsumableSlot(false, _storeConfig.consumablePrices[(int)(consumables[i].Rarity - 1)], 
                    0.0f, consumables[i]);
                _consumables.Add(slot);
            }
        }

        public void DeleteCard()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnStoreBeginDiscardCard, new Dictionary<string, object>()
            {
                {"Character", _character}
            });
        }
        
        #region GetItems
        
        public List<VCard> GetRandomCards(int count)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            List<VCardConfiguration> cards = VResourcesManager.Instance.GetAllCardConfigurations();
            
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
                        if(upgradeProbability <= CardRarityUpgradeProbabilities[(int)card.Rarity - 1])
                            card.Upgrade(false);
                        selectedCards.Add(card);
                        break;
                    }
                }
            }
            return selectedCards;
        }

        public List<VConsumable> GetRandomConsumables(int count)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            List<VConsumableConfiguration> consumables = VResourcesManager.Instance.GetAllConsumableConfigurations();
            
            if (consumables.Count == 0)
                return null;
            
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

            for (int i = 0; i < count; i++)
            {
                float probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (int j = 0; j < consumables.Count; j++)
                {
                    totalProbability += perConsumableProbabilityByRarity[(int)consumables[j].rarity - 1];
                    if (probability <= totalProbability)
                    {
                        var card = consumables[j].CreateConsumable();
                        selectedConsumables.Add(card);
                        break;
                    }
                }
            }
            return selectedConsumables;
        }
        #endregion
    }
}