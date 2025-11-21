using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core.UI;
using VTuber.BattleSystem.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Relic.UI;
using VTuber.ScheduleSystem.UI;

namespace VTuber.Core.UI.VCharacterSelection
{
    public class VCharacterDetailsUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text liveTypeText;
        [SerializeField] private VCardUI card;
        [SerializeField] private TMP_Text staminaText;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text followerText;
        [SerializeField] private VAbilityDetails abilityDetails;
        [SerializeField] private Transform pressureEffectTableGrid;
        [SerializeField] private GameObject pressureEffectEntryPrefab;
        [SerializeField] private VEventDataUI eventDataUI;
        [SerializeField] private VRelicSlotUI relicSlotUI;

        private readonly Dictionary<uint, VCard> _cardsCreated = new();
        private List<VPressureEffectTableEntry> _pressureEffects;
        
        public string GetLiveTypeText(string liveType)
        {
            switch (liveType)
            {
                case "A":
                    return "情感系";
                case "I":
                    return "知性系";
                case "G":
                    return "重力系";
                case "C":
                    return "混沌系";
                case "P":
                    return "精神系";
            }

            return null;
        }

        public void SetDetails(VCharacterConfiguration characterConfig)
        {
            nameText.text = characterConfig.characterName;
            descriptionText.text = characterConfig.description;
            liveTypeText.text = GetLiveTypeText(characterConfig.liveType);
            if (_cardsCreated.TryGetValue(characterConfig.initialCardId, out var value))
            {
                card.SetCard(value);
            }
            else
            {
                var c = VDataManager.Instance.CreateCardByID(characterConfig.initialCardId);
                _cardsCreated.Add(characterConfig.initialCardId, c);
                card.SetCard(c);
            }

            staminaText.text = $"{characterConfig.staminaInitialValue}/{characterConfig.staminaMaxValue}";
            moneyText.text = characterConfig.moneyInitialValue.ToString();
            followerText.text = characterConfig.followerCountInitialValue.ToString();

            abilityDetails.SetDetails(characterConfig);

            if(_pressureEffects is not null)
                foreach (var entry in _pressureEffects)
                {
                    Destroy(entry.gameObject);
                }
            
            _pressureEffects = new List<VPressureEffectTableEntry>();
            for (int i = 0; i < characterConfig.pressureEffects.Count; i++)
            {
                var entry = Instantiate(pressureEffectEntryPrefab, pressureEffectTableGrid);
                var pressureLevelInfo = VUIUtils.Instance.GetPressureIcon(i + 1);
                if (i == 2)
                {
                    
                    entry.GetComponent<VPressureEffectTableEntry>().SetEffect(pressureLevelInfo.Value, 
                        pressureLevelInfo.Key, "无效果");
                }
                else
                {
                    entry.GetComponent<VPressureEffectTableEntry>().SetEffect(pressureLevelInfo.Value, 
                        pressureLevelInfo.Key, "每天结束时, " + characterConfig.pressureEffects[i].CreateRaisingEffect().Description);
                }
                
                _pressureEffects.Add(entry.GetComponent<VPressureEffectTableEntry>());
            }
            if (characterConfig.isCharacterEventStream)
            {
                eventDataUI.Initialize(VDataManager.Instance.GetStreamEventConfigurationByID(characterConfig.characterEvent), false);
            }
            else
            {
                eventDataUI.Initialize(VDataManager.Instance.GetDialogueEventConfigurationByID(characterConfig.characterEvent), false);
            }
            
            relicSlotUI.Initialize(VDataManager.Instance.CreateRelicByID(characterConfig.initialRelicId), false);
        }
    }
}