using System.Collections.Generic;
using UnityEngine.Analytics;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.Character;

namespace VTuber.BattleSystem.Core
{
    public class VBattleAttributeManager
    {
         // //from stamina
         // public int MaxStamina { get; private set; } 
         //
         // //from followers
         // public int InitViewerCount { get; private set; } 
         //
         // //from streaming stats
         // public float SingingScoringMultiplier { get; private set; }
         // public float GamingScoringMultiplier { get; private set; }
         // public float ChattingScoringMultiplier { get; private set; }
         //
         // //from memberships
         // public int MembershipCount { get; private set; }

        private Dictionary<string, VBattleAttribute> _battleAttributes;

        public VBattleAttributeManager(VCharacterAttributeManager characterAttributeManager)
        {
            _battleAttributes = new Dictionary<string, VBattleAttribute>();

            foreach (var attributePair in characterAttributeManager.Attributes)
            {
                var attribute = attributePair.Value;
                if (attribute.IsConvertToBattleAttribute)
                {
                    _battleAttributes.Add(attribute.GetBattleAttributeName(), attribute.ConvertToBattleAttribute());
                }
            }
        }

        public void AddAttribute(string name, VBattleAttribute attribute)
        {
            _battleAttributes.Add(name, attribute);
        }
        
    }
}