using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.Store
{
    public class VStoreDiscardButton : VStoreButton
    {
        public VStoreDiscardButton(int originalPrice, int priceIncrease) : base(originalPrice, priceIncrease)
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreEndDeleteCard, OnStoreEndDeleteCard);
        }
        
        private void OnStoreEndDeleteCard(Dictionary<string, object> messagedict)
        {
            var deleted = messagedict["Deleted"] as bool? ?? false;
            if (deleted)
            {
                TotalIncrease += PriceIncrease;
            }
        }
    }
    
    public class VStoreUpgradeButton : VStoreButton
    {
        public VStoreUpgradeButton(int originalPrice, int priceIncrease) : base(originalPrice, priceIncrease)
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStoreEndUpgradeCard, OnStoreEndUpgradeCard);
        }
        
        private void OnStoreEndUpgradeCard(Dictionary<string, object> messagedict)
        {
            var upgraded = messagedict["Upgraded"] as bool? ?? false;
            if (upgraded)
            {
                TotalIncrease += PriceIncrease;
            }
        }
    }
    
    public class VStoreButton
    {
        public int OriginalPrice { get; protected set; }
        public int TotalIncrease { get; protected set; }
        public int Price { get; protected set; }
        public int PriceIncrease { get; protected set; }
        public bool IsDiscount { get; protected set; }
        public bool IsGlobalDiscount { get; protected set; }
        public float Discount { get; protected set; }

        public VStoreButton(int originalPrice, int priceIncrease)
        {
            PriceIncrease = priceIncrease;
            OriginalPrice = originalPrice;
            this.Discount = Discount;
            
        }
        
        public void SetPrice(bool isDiscount, float discount)
        {
            Discount = discount;
            IsDiscount = isDiscount;
            if(isDiscount)
                Price = (int)((OriginalPrice + TotalIncrease) * (1.0f - Discount));
            else
                Price = OriginalPrice + TotalIncrease;
        }

    
        public bool Affordable(VCharacter character)
        {
            return character.AttributeManager.Attributes["CAMoney"].Value >= Price;
        }

        public void Buy(VCharacter character)
        {
            character.AttributeManager.Attributes["CAMoney"].AddTo(-Price);
        }
    }
}