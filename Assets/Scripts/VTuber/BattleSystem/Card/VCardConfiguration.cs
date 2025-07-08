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
            
        public int cost;
        public Sprite icon;

        public List<VBuffConfiguration> buffs;
        public List<VEffectConfiguration> effects;
        
    }
}