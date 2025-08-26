using VTuber.BattleSystem.Core;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Effect
{
    public class VAddPercentageFromAttributeEffect : VEffect, IVValuePreview
    {
        
        public string attributeNameToAdd;
        public string attributeNameToBeAdded;
        private VUpgradableValue<float> _percentage;

        public VAddPercentageFromAttributeEffect(VAddPercentageFromAttributeEffectConfiguration configuration, string parameter, string upgradedParameter) : base(configuration)
        {
            attributeNameToAdd = configuration.attributeNameToAdd;
            attributeNameToBeAdded = configuration.attributeNameToBeAdded;
            _percentage = new VUpgradableValue<float>(float.Parse(parameter), float.Parse(upgradedParameter));
        }

        public override void ApplyEffect(VBattle battle, int layer = 1, bool isFromCard = false, bool shouldApplyTwice = false)
        {
            base.ApplyEffect(battle, layer, isFromCard, shouldApplyTwice);
            battle.BattleAttributeManager.TryGetAttribute(attributeNameToAdd, out var attributeToAdd);
            battle.BattleAttributeManager.TryGetAttribute(attributeNameToBeAdded, out var attributeToBeAdded);
            
            if (attributeToAdd == null || attributeToBeAdded == null)
            {
                VDebug.LogError($"Attribute {attributeNameToAdd} or {attributeNameToBeAdded} not found.");
                return;
            }
            
            int delta = (int)(_percentage.Value * attributeToAdd.Value);
            if (MultiplyByLayer > 0.0f)
                delta *= (int)(layer * MultiplyByLayer);
            attributeToBeAdded.AddTo(delta, isFromCard, shouldApplyTwice);
            VDebug.Log($"Effect {_configuration.effectName} added {delta} to {attributeNameToBeAdded}. New value: {attributeToBeAdded.Value}");
        }

        public override string GetValue()
        {
            return (int)(_percentage.Value * 100) + "%";
        }

        public string AttributeName => attributeNameToBeAdded;
        public int GetValue(VBattle battle)
        {
            battle.BattleAttributeManager.TryGetAttribute(attributeNameToAdd, out var attributeToAdd);
            battle.BattleAttributeManager.TryGetAttribute(attributeNameToBeAdded, out var attributeToBeAdded);
            
            return attributeToBeAdded.PreviewAddTo((int)(_percentage.Value * attributeToAdd.Value));
        }
    }
}