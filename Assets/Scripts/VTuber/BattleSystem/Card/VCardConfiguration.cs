using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Card
{
    public class VCardConfiguration : VScriptableObject
    {
        public string cardName;
        [TextArea] public string description;
        
        [StringToEnum] public string cardType;
        
        [StringToEnum] public List<string> cardTags;
        public VCardRarity rarity;
            
        public Sprite background;
        public Sprite facade;
        
        public int cost;
        public bool isExaust = false;

        public List<VBuffConfiguration> buffs;
        public List<VEffectConfiguration> effects;


        private static int idDistributor = 0;
        
        public VCard CreateCard()
        {
            return new VCard(this, idDistributor++);
        }
    }
}