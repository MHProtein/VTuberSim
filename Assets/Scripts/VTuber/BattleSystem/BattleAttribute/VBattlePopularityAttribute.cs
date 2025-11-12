using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePopularityAttribute : VBattleAttribute
    {
        public VBattlePopularityAttribute(int value) : base(value, false, VBattleEventKey.OnPopularityChange)
        {
            ScoreForAbilities = new Dictionary<string, int>
            {
                { "BASingingMultiplier", 0 },
                { "BAGamingMultiplier", 0 },
                { "BAChattingMultiplier", 0 }
            };
            gainPointsModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
            gainRateModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
        }

        public VBattlePopularityAttribute(VBattleAttributeSaveData saveData) : base(saveData)
        {
            ScoreForAbilities = saveData.scoreForAbilities;
            gainPointsModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
            gainRateModifier.SetEventKey(VBattleEventKey.OnParameterPopularityModifierChanged);
        }

        public Dictionary<string, int> ScoreForAbilities { get; }

        public override VBattleAttributeSaveData Save()
        {
            var data = base.Save();
            data.scoreForAbilities = ScoreForAbilities;
            return data;
        }

        public void AddPopularity(int delta, string abilityName = "", bool isFromCard = false,
            bool shouldPlayTwice = false)
        {
            if (delta == 0)
                return;

            if (ScoreForAbilities.ContainsKey(abilityName)) ScoreForAbilities[abilityName] += delta;

            AddTo(delta, isFromCard, shouldPlayTwice);
        }
    }
}