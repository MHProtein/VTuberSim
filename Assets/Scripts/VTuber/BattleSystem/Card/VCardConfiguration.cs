using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Card
{    public enum CostType
    {
        Stamina,
        Buff,
    }
    public class VCardConfiguration : VScriptableObject
    {
        public string cardName;
        [TextArea] public string description;
        
        [StringToEnum] public string cardType;
        
        [StringToEnum] public List<string> cardTags;
        public VCardRarity rarity;
            
        public Sprite background;
        public Sprite facade;
        
        public CostType costType = CostType.Stamina;
        [FormerlySerializedAs("buffId")] [ShowIf("@costType == CostType.Buff")] public int costBuffId;
        public int cost;
        public bool isExaust = false;

        public List<VEffectConfiguration> effects;


        private static int idDistributor = 0;
        
        public VCard CreateCard()
        {
            return new VCard(this, idDistributor++);
        }
    }
}