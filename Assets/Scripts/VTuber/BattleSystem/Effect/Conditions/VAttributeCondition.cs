using System.Collections.Generic;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect.Conditions
{
    public class VAttributeCondition : VEffectCondition
    {
        private string _attributeName;
        private int _targetValue;

        public VAttributeCondition(CsvReader csv) : base(csv)
        {
            _attributeName = csv.GetField<string>("AttributeName");
            _targetValue = csv.GetField<int>("TargetValue");
        }

        public override bool IsTrue(VBattle battle, Dictionary<string, object> message)
        {
            if (battle.BattleAttributeManager.TryGetAttribute(_attributeName, out var attribute))
            {
                return Compare(attribute.Value, _targetValue);
            }

            return false;
        }
    }
}