using System;
using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Core
{
    public class VBattleAttributeManagerSaveData
    {
        public List<VBattleAttributeSaveData> attributeSaveDatas;
        public VMultiplierManagerSaveData multiplierManagerSaveData;
        public VStaminaManagerSaveData staminaManagerSaveData;
    }

    public class VBattleAttributeManager
    {
        private bool _isPhaseEnding;

        public VBattleAttributeManager(bool isPhaseEnding, VBattleAttributeManagerSaveData saveData)
        {
            if (saveData is not null)
            {
                _isPhaseEnding = isPhaseEnding;
                BattleAttributes = new Dictionary<string, VBattleAttribute>();
                foreach (var attributeSaveData in saveData.attributeSaveDatas)
                {
                    var attribute =
                        Activator.CreateInstance(Type.GetType(attributeSaveData.AttributeType), attributeSaveData) as
                            VBattleAttribute;
                    BattleAttributes.Add(attribute.AttributeName, attribute);
                }

                StaminaManager = new VStaminaManager(
                    BattleAttributes.TryGetValue("BAStamina", out var stamina)
                        ? (VBattleStaminaAttribute)stamina
                        : null,
                    BattleAttributes.TryGetValue("BAShield", out var shield) ? (VBattleStaminaAttribute)shield : null,
                    saveData.staminaManagerSaveData
                );

                MultiplierManager = new VMultiplierManager(
                    BattleAttributes.TryGetValue("BASingingMultiplier", out var singing)
                        ? (VBattleMultiplierAttribute)singing
                        : null,
                    BattleAttributes.TryGetValue("BAGamingMultiplier", out var gaming)
                        ? (VBattleMultiplierAttribute)gaming
                        : null,
                    BattleAttributes.TryGetValue("BAChattingMultiplier", out var chatting)
                        ? (VBattleMultiplierAttribute)chatting
                        : null,
                    saveData.multiplierManagerSaveData
                );

                MultiplierManager.OnEnable();
                return;
            }

            _isPhaseEnding = isPhaseEnding;
            BattleAttributes = new Dictionary<string, VBattleAttribute>();
        }

        public Dictionary<string, VBattleAttribute> BattleAttributes { get; }

        public VStaminaManager StaminaManager { get; private set; }

        public VMultiplierManager MultiplierManager { get; private set; }

        public VBattleAttributeManagerSaveData Save()
        {
            var saveData = new VBattleAttributeManagerSaveData();
            saveData.attributeSaveDatas = new List<VBattleAttributeSaveData>();
            foreach (var attribute in BattleAttributes) saveData.attributeSaveDatas.Add(attribute.Value.Save());
            saveData.staminaManagerSaveData = StaminaManager.Save();
            saveData.multiplierManagerSaveData = MultiplierManager.Save();
            return saveData;
        }

        public void AttributesConversion(VCharacterAttributeManager characterAttributeManager)
        {
            ConvertFromCharacterAttributes(characterAttributeManager);
        }

        public void Clear()
        {
            BattleAttributes.Clear();
            if (MultiplierManager is not null)
                MultiplierManager.Reset();
            StaminaManager.Reset();
        }

        public void InitializeInternalManagers(int mainAttributeIndex, List<int> abilityTurnCounts)
        {
            StaminaManager = new VStaminaManager(
                BattleAttributes.TryGetValue("BAStamina", out var stamina) ? (VBattleStaminaAttribute)stamina : null,
                BattleAttributes.TryGetValue("BAShield", out var shield) ? (VBattleStaminaAttribute)shield : null
            );

            MultiplierManager = new VMultiplierManager(
                mainAttributeIndex,
                4,
                abilityTurnCounts,
                BattleAttributes.TryGetValue("BASingingMultiplier", out var singing)
                    ? (VBattleMultiplierAttribute)singing
                    : null,
                BattleAttributes.TryGetValue("BAGamingMultiplier", out var gaming)
                    ? (VBattleMultiplierAttribute)gaming
                    : null,
                BattleAttributes.TryGetValue("BAChattingMultiplier", out var chatting)
                    ? (VBattleMultiplierAttribute)chatting
                    : null,
                BattleAttributes.TryGetValue("BATurn", out var turnAttribute)
                    ? (VBattleTurnAttribute)turnAttribute
                    : null
            );

            var viewerCount = BattleAttributes["BAViewerCount"].Value;
            foreach (var multiplier in MultiplierManager.Multipliers)
                multiplier.AddTo(VMathUtils.FloatToInt(viewerCount * 0.1f), false);

            MultiplierManager.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnViewerCountChange, OnViewerCountChange);
        }

        public void ConvertFromCharacterAttributes(VCharacterAttributeManager characterAttributeManager)
        {
            foreach (var attribute in characterAttributeManager.Attributes)
            {
                if (!attribute.Value.IsConvertToBattleAttribute)
                    continue;
                var battleAttribute = attribute.Value.ConvertToBattleAttribute();
                if (battleAttribute.Value != null)
                {
                    AddAttribute(battleAttribute.Key, battleAttribute.Value);
                    battleAttribute.Value.OnEnable();
                }
            }
        }

        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterChange, OnParameterChange);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }

        public void OnDisable()
        {
            foreach (var attribute in BattleAttributes) attribute.Value.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterChange, OnParameterChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnViewerCountChange, OnViewerCountChange);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
            MultiplierManager.OnDisable();
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            StaminaManager.OnTurnEnd();
        }

        private void OnViewerCountChange(Dictionary<string, object> messagedict)
        {
            var delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            foreach (var multiplier in MultiplierManager.Multipliers)
                multiplier.AddTo(VMathUtils.FloatToInt(delta * 0.2f), false);
        }

        private void OnParameterChange(Dictionary<string, object> messagedict)
        {
            if (BattleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                var multiplier = MultiplierManager.Multiplier.Value / 100f;
                var delta = (int)messagedict["Delta"];
                if (delta <= 0)
                    return;
                (BattleAttributes["BAPopularity"] as VBattlePopularityAttribute).AddPopularity(
                    (int)(delta * multiplier), MultiplierManager.Multiplier.AttributeName,
                    messagedict["IsFromCard"] as bool? ?? false,
                    messagedict["ShouldPlayTwice"] as bool? ?? false);
            }
        }

        public int PreviewPopularityChange(int delta)
        {
            if (BattleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                var multiplier = MultiplierManager.Multiplier.Value / 100f;
                var parameterDelta = parameter.PreviewAddTo(delta) - parameter.Value;
                return (int)(parameterDelta * multiplier);
            }

            return 0;
        }

        public int PreviewShieldChange(int delta)
        {
            if (BattleAttributes.TryGetValue("BAShield", out var parameter))
            {
                var parameterDelta = parameter.PreviewAddTo(delta) - parameter.Value;
                return parameterDelta;
            }

            return 0;
        }

        public bool TryGetAttribute(string name, out VBattleAttribute attribute)
        {
            return BattleAttributes.TryGetValue(name.Trim(), out attribute);
        }

        public void AddAttribute(string name, VBattleAttribute attribute)
        {
            BattleAttributes.Add(name, attribute);
            attribute.AttributeName = name;
            attribute.OnEnable();
        }

        public void RemoveAttribute(string name)
        {
            if (BattleAttributes.TryGetValue(name, out var attribute))
            {
                attribute.OnDisable();
                BattleAttributes.Remove(name);
            }
        }

        public void SkipTurnRecoverStamina()
        {
            BattleAttributes.TryGetValue("BAStamina", out var stamina);
            BattleAttributes.TryGetValue("BASkipTurnStaminaRecovery", out var recoveryAmount);
            stamina.AddTo(recoveryAmount.Value, false);
        }
    }
}