using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePopularityAttribute : VBattleAttribute
    {
        public Dictionary<string, int> ScoreForAbilities => _scoreForAbilities;
        private Dictionary<string, int> _scoreForAbilities;
        
        public VBattlePopularityAttribute(int value) : base(value, false, VBattleEventKey.OnPopularityChange)
        {
            _scoreForAbilities = new Dictionary<string, int>()
            {
                { "BASingingMultiplier", 0 },
                { "BAGamingMultiplier", 0 },
                { "BAChattingMultiplier", 0 },
            };
        }
        
        public void AddPopularity(int delta, string abilityName = "")
        {
            if (delta == 0)
                return;

            if (_scoreForAbilities.ContainsKey(abilityName))
            {
                _scoreForAbilities[abilityName] += delta;
            }
            else
            {
                VDebug.LogError("Ability name not found in popularity attribute: " + abilityName);
            }
            
            AddTo(delta, false, false);
        }
        
    }
}