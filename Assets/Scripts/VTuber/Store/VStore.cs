using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Consumable;

namespace VTuber.Store
{
    public class VStoreConsumableSlot : VStoreSlot
    {
        public VConsumable consumable;
    }
    
    public class VStoreCardSlot : VStoreSlot
    {
        public VCard card;

        public override void Buy(VCharacter character)
        {
            base.Buy(character);
            character.CardLibrary.AddCard(card);
        }
    }
    
    public class VStoreSlot
    {
        public int price;

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
        private List<VCard> _cards = new List<VCard>();
        private List<VConsumable> _consumables = new List<VConsumable>();

        private VCharacter _character;
        private VStoreConfiguration _storeConfig;
        
        public VStore(VCharacter character, VStoreConfiguration storeConfig)
        {
            _character = character;
            _storeConfig = storeConfig;
        }
        
        
    }
}