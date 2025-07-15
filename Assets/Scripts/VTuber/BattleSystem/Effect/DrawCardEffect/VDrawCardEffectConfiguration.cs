using CsvHelper;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Effect
{
    public class VDrawCardEffectConfiguration : VEffectConfiguration
    {
        public int drawCardCount;
        public VDrawCardEffectConfiguration(CsvReader csv) : base(csv)
        {
            drawCardCount = csv.GetField<int>("DrawCount");
        }

        public override VEffect CreateEffect()
        {
            return new VDrawCardEffect(this);
        }
    }
}