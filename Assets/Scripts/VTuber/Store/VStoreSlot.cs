using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Store
{
    public class VStoreConsumableSlot : VStoreSlot
    {
        public VConsumable consumable;

        public VStoreConsumableSlot(bool isDiscount, bool isGlobalDiscount, int originalPrice, float discount,
            VConsumable consumable)
            : base(isDiscount, isGlobalDiscount, originalPrice, discount)
        {
            this.consumable = consumable;
        }

        public override void Buy(VCharacter character)
        {
            base.Buy(character);
            
            VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(VAnimationRequestFactory.CreateAddConsumableRequest(consumable, false));
            VRaisingAnimationSystem.Instance.ExecuteAnimations(null);
        }
    }

    public class VStoreCardSlot : VStoreSlot
    {
        public VCard card;

        public VStoreCardSlot(bool isDiscount, bool isGlobalDiscount, int originalPrice, float discount, VCard card)
            : base(isDiscount, isGlobalDiscount, originalPrice, discount)
        {
            this.card = card;
        }

        public override void Buy(VCharacter character)
        {
            base.Buy(character);
            
            VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(VAnimationRequestFactory.CreateAddCardRequest(card));
            VRaisingAnimationSystem.Instance.ExecuteAnimations(null);
        }
    }

    public class VStoreSlot
    {
        public VStoreSlot(bool isDiscount, bool isGlobalDiscount, int originalPrice, float discount)
        {
            IsDiscount = isDiscount;
            IsGlobalDiscount = isGlobalDiscount;
            OriginalPrice = originalPrice;
            if (isDiscount || isGlobalDiscount)
                Price = (int)(originalPrice * (1.0f - discount));
            else
                Price = originalPrice;
            Discount = discount;
        }

        public int OriginalPrice { get; protected set; }
        public int Price { get; protected set; }
        public float Discount { get; protected set; }
        public bool IsDiscount { get; protected set; }
        public bool IsGlobalDiscount { get; protected set; }

        public bool Affordable(VCharacter character)
        {
            return character.AttributeManager.Attributes["CAMoney"].Value >= Price;
        }

        public virtual void Buy(VCharacter character)
        {
            character.AttributeManager.Attributes["CAMoney"].AddTo(-Price, true);
        }
    }
}