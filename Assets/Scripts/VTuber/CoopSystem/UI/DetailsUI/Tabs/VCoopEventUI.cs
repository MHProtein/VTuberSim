using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Core.UI;

namespace VTuber.CoopSystem.UI.DetailsUI
{
    public class VCoopEventUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text eventName;
        [SerializeField] private TMP_Text unlockLevel;
        [SerializeField] private GameObject checkmark;
        [SerializeField] private Transform conditionGrid;
        [SerializeField] private Transform effectGrid;
        [SerializeField] private GameObject conditionPrefab;
        [SerializeField] private GameObject effectPrefab;
        
        private VCoopEvent _coopEvent;
        private int _coopLevel;
        private List<GameObject> _conditions;
        private List<GameObject> _effects;
        
        public void Initialize(VCoopEvent coopEvent, int coopLevel, string levelName)
        {
            _conditions = new List<GameObject>();
            _effects = new List<GameObject>();
                
            eventName.text = coopEvent.eventName;
            unlockLevel.text = levelName;
            if (coopEvent.unlockLevel >= coopLevel)
            {
                checkmark.gameObject.SetActive(true);
            }

            foreach (var eventType in coopEvent.eventTypes)
            {
                var condition = Instantiate(conditionPrefab, conditionGrid);
                var x = eventType.eventType.ToString();
                condition.GetComponentInChildren<TMP_Text>().text = VUIUtils.Instance.GetEventName(eventType.eventType);
                condition.GetComponentInChildren<Image>().sprite = VResourcesManager.Instance.TryGetSprite(x);
                _conditions.Add(condition);
            }

            foreach (var effect in coopEvent.effects)
            {
                var effectUI = Instantiate(effectPrefab, effectGrid);
                if (effect is IAttributeEffect attributeEffect)
                {
                    effectUI.GetComponentInChildren<TMP_Text>().text = "+" + effect.GetParameter();
                    effectUI.GetComponentInChildren<Image>().sprite =
                        VUIUtils.Instance.GetAttributeIcon(attributeEffect.AttributeName);
                    _effects.Add(effectUI);
                    return;
                }

                if (effect is VRaisingAddCoopValueEffect raisingAddCoopValueEffect)
                {
                    effectUI.GetComponentInChildren<TMP_Text>().text = "+" + effect.GetParameter();
                    effectUI.GetComponentInChildren<Image>().sprite =
                        VUIUtils.Instance.GetCoopIcon();
                    _effects.Add(effectUI);
                }
            }
        }
    }
}