using System.Collections.Generic;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VBattlePopularityAttribute : VBattleAttribute
    {
        public VBattlePopularityAttribute(int value) : base(value, false)
        {
        }

        public override void AddTo(int delta)
        {
            base.AddTo(delta);
            
            Dictionary<string, object> messageDict = new Dictionary<string, object>
            {
                { "Popularity", Value },
                { "Delta", delta }
            };
            
        }

        public override void MultiplyWith(int delta)
        {
            base.MultiplyWith(delta);
        }
    }
}