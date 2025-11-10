using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.UI;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;
using VTuber.Relic.UI;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VCoopLevelTab : VCoopTab
    {
        [SerializeField] private int level;
        [SerializeField] private Transform effectContainer;
        [SerializeField] private Transform itemPosition;
        [SerializeField] private TMP_Text itemText;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject relicPrefab;
        [SerializeField] private GameObject effectPrefab;

        private VCardUI _currentCard;
        private List<TMP_Text> _currentEffects;
        private VRelicSlotUI _currentRelic;

        public override void SetTab(VCooperator cooperator)
        {
            var node = VDataManager.Instance
                .DialogueEventConfigs[cooperator.configuration.CoopLevels[level].upgradeEventID]
                .dialogueNode;
            var dialog = VResourcesManager.Instance.TryGetDialog(node);

            var effects = dialog.GetEffects();


            _currentEffects = new List<TMP_Text>();
            foreach (var effect in effects)
            {
                if (effect is IAttributeEffect attributeEffect)
                {
                    var text = "";
                    var effectText = Instantiate(effectPrefab, effectContainer).GetComponent<TMP_Text>();
                    if (effect is VRaisingAddAttributeMaxValueEffect raisingAddAttributeMaxValueEffect) text += "最大";

                    text += VUIUtils.Instance.GetAttributeName(attributeEffect.AttributeName);

                    var parameter = effect.GetParameter();
                    if (parameter.Contains('-'))
                        text += parameter;
                    else
                        text += "+" + parameter;
                    if (attributeEffect.AttributeName.Contains("GainEfficiency"))
                        text += "%";
                    effectText.text = text;
                    _currentEffects.Add(effectText);
                    continue;
                }

                if (effect is VRaisingAddRelicEffect raisingAddRelicEffect)
                {
                    itemText.text = "获得遗物";
                    var relic = VDataManager.Instance.CreateRelicByID(raisingAddRelicEffect.RelicId);
                    var go = Instantiate(relicPrefab, itemPosition);
                    var relicUI = go.GetComponent<VRelicSlotUI>();
                    relicUI.Initialize(relic, false);
                    _currentRelic = relicUI;
                    continue;
                }

                if (effect is VAddSpecifiedCardEffect addSpecifiedCardEffect)
                {
                    itemText.text = "获得卡牌";
                    var card = VDataManager.Instance.CreateCardByID(addSpecifiedCardEffect.CardId);
                    var go = Instantiate(cardPrefab, itemPosition);
                    var cardUI = go.GetComponent<VCardUI>();
                    cardUI.SetCard(card);
                    _currentCard = cardUI;
                    continue;
                }

                var otherEffectText = Instantiate(effectPrefab, effectContainer).GetComponent<TMP_Text>();
                otherEffectText.text = effect.Name;
                _currentEffects.Add(otherEffectText);
            }
        }

        public override void Clear()
        {
            if (_currentEffects is not null)
            {
                foreach (var effectText in _currentEffects) Destroy(effectText.gameObject);
                _currentEffects.Clear();
            }

            if (_currentCard is not null)
            {
                Destroy(_currentCard.gameObject);
                _currentCard = null;
            }

            if (_currentRelic is not null)
            {
                Destroy(_currentRelic.gameObject);
                _currentRelic = null;
            }
        }
    }
}