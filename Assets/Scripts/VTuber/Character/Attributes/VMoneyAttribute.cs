using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character.Attribute;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.Character.Attributes
{
    public class VMoneyAttribute : VCharacterAttribute
    {
        public VMoneyAttribute(VCharacterAttributeConfiguration configuration, int initialValue,
            VRaisingEventKey eventKey = VRaisingEventKey.Default,
            int maxValue = int.MaxValue, int minValue = 0)
            : base(configuration, initialValue, eventKey, maxValue, minValue)
        {
        }

        public override void ConvertToAttribute(Dictionary<string, VBattleAttribute> battleAttributes)
        {
            ;
            if (battleAttributes.TryGetValue("BARevenueShareRate", out var revenueShareRate) &&
                battleAttributes.TryGetValue("BARevenue", out var revenue))
            {
                AddTo((int)(revenueShareRate.Value * revenue.Value / 100.0f), false);
                VDebug.Log("Money attribute converted from battle attributes: " +
                           $"Revenue Share Rate: {revenueShareRate.Value}, Revenue: {revenue.Value}, " +
                           $"Converted Value: {(int)(revenueShareRate.Value * revenue.Value / 100.0f)}");
            }
            else
            {
                VDebug.LogWarning("Battle attribute BAPopularity not found in battle attributes.");
            }
        }
    }
}