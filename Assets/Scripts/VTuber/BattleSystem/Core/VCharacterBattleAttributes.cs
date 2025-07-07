using UnityEngine.Analytics;
using VTuber.Character;

namespace VTuber.BattleSystem.Core
{
    public class VCharacterBattleAttributes
    {
        //from stamina
        public int MaxStamina { get; private set; } 
        
        //from followers
        public int InitViewerCount { get; private set; } 
        
        //from streaming stats
        public float SingingScoringMultiplier { get; private set; }
        public float GamingScoringMultiplier { get; private set; }
        public float ChattingScoringMultiplier { get; private set; }
        
        //from memberships
        public int MembershipCount { get; private set; }

        public VCharacterBattleAttributes(VCharacterAttributeManager characterAttributeManager)
        {
            
        }
        
    }
}