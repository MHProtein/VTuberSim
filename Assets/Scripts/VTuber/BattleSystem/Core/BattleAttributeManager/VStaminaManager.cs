using VTuber.BattleSystem.BattleAttribute;

namespace VTuber.BattleSystem.Core
{
    public class VStaminaManagerSaveData
    {
        public VValueModifierSaveData<int> consumePointsModifier;
        public VValueModifierSaveData<float> consumeRateModifier;
    }

    public class VStaminaManager
    {
        private readonly VBattleStaminaAttribute _shieldAttribute;

        private readonly VBattleStaminaAttribute _staminaAttribute;
        protected VValueModifier<int> consumePointsModifier;
        protected VValueModifier<float> consumeRateModifier;

        public VStaminaManager(VBattleStaminaAttribute staminaAttribute, VBattleStaminaAttribute shieldAttribute,
            VStaminaManagerSaveData saveData)
        {
            _staminaAttribute = staminaAttribute;
            _shieldAttribute = shieldAttribute;
            consumePointsModifier = saveData.consumePointsModifier.LoadModifier(true);
            consumeRateModifier = saveData.consumeRateModifier.LoadModifier(true);
        }

        public VStaminaManager(VBattleStaminaAttribute staminaAttribute, VBattleStaminaAttribute shieldAttribute)
        {
            _staminaAttribute = staminaAttribute;
            _shieldAttribute = shieldAttribute;
            consumePointsModifier = new VValueModifier<int>(0, true);
            consumeRateModifier = new VValueModifier<float>(0.0f, true);
        }

        public VValueModifier<float> ConsumeRateModifier => consumeRateModifier;

        public VValueModifier<int> ConsumePointsModifier => consumePointsModifier;

        public VStaminaManagerSaveData Save()
        {
            return new VStaminaManagerSaveData
            {
                consumePointsModifier = consumePointsModifier.Save(),
                consumeRateModifier = consumeRateModifier.Save()
            };
        }

        public void ApplyCost(int cost, bool ignoreShield = false)
        {
            var calculatedCost = CalculateCost(cost);

            var costAfterShield = calculatedCost;
            if (!ignoreShield)
            {
                costAfterShield = calculatedCost - _shieldAttribute.Value;

                _shieldAttribute.AddTo(-calculatedCost >= 0 ? 0 : -calculatedCost, false);
                if (costAfterShield <= 0)
                    return;
            }

            _staminaAttribute.AddTo(-costAfterShield >= 0 ? 0 : -costAfterShield, false);
        }

        public bool TestCost(int cost, bool ignoreShield = false)
        {
            var calculatedCost = CalculateCost(cost);

            var costAfterShield = calculatedCost;
            if (!ignoreShield)
            {
                costAfterShield = calculatedCost - _shieldAttribute.Value;

                if (costAfterShield <= 0)
                    return true;
            }

            return _staminaAttribute.TestCost(-costAfterShield >= 0 ? 0 : -costAfterShield);
        }

        public int CalculateCost(int delta)
        {
            delta = (int)(delta * (1.0f - VValueModifier<int>.GetModifierFloatValue(consumeRateModifier, false)))
                    - VValueModifier<int>.GetModifierIntValue(consumePointsModifier, false);

            return delta;
        }

        public void Reset()
        {
            consumeRateModifier.Reset();
            consumePointsModifier.Reset();
        }

        public void OnTurnEnd()
        {
            foreach (var mod in consumePointsModifier.Modifiers)
                if (mod.Value.DecreaseTurnCount())
                    consumePointsModifier.RemoveModifier(mod.Key);

            foreach (var mod in consumeRateModifier.Modifiers)
                if (mod.Value.DecreaseTurnCount())
                    consumeRateModifier.RemoveModifier(mod.Key);
        }
    }
}