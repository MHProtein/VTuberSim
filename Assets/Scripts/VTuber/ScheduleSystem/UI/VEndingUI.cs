using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core.UI.VAccountSelection;
using VTuber.BattleSystem.UI;
using VTuber.Core.Foundation;
using VTuber.Core.RaisingEffect;
using VTuber.Reincarnation;
using VTuber.Relic;
using VTuber.Relic.UI;

namespace VTuber.ScheduleSystem.UI
{
    public class VEndingUI : VUIBehaviour
    {
        [SerializeField] private TMP_Text ratingLevelText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text accountNameText;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject relicPrefab;
        [SerializeField] private GameObject attributeEffectPrefab;
        [SerializeField] private Transform relicGrid; 
        [SerializeField] private Transform cardGrid; 
        [SerializeField] private Transform attributeEffectGrid; 
        
        private List<VCardUI> cards;
        private List<VRelicSlotUI> relics;
        private List<VAttributeEffectUI> attributeEffects;
        
        
        public void Initialize(string ratingLevel, int score, VAccount account)
        {
            ratingLevelText.text = ratingLevel;
            scoreText.text = score.ToString();
            
            cards = new List<VCardUI>();
            relics = new List<VRelicSlotUI>();
            attributeEffects = new List<VAttributeEffectUI>();
            
            foreach (var card in account.Cards)
            {
                SpawnCard(card);
            }
            foreach (var relic in account.Relics)
            {
                SpawnRelic(relic);
            }

            for (int i = 0; i < account.Effects.Count; i++)
            {
                SpawnAttributeEffect(account.Effects[i], account.EffectItems[i].level);
            }
        }

        private void SpawnCard(VCard card)
        {
            var go = Instantiate(cardPrefab, cardGrid);
            var ui = go.GetComponent<VCardUI>();
            ui.SetCard(card);
            cards.Add(ui);
        }

        private void SpawnRelic(VRelic relic)
        {
            var go = Instantiate(relicPrefab, relicGrid);
            var ui = go.GetComponent<VRelicSlotUI>();
            ui.Initialize(relic, false);
            relics.Add(ui);
        }

        private void SpawnAttributeEffect(VRaisingEffect attributeEffect, int level)
        {
            var go = Instantiate(attributeEffectPrefab, attributeEffectGrid);
            var ui = go.GetComponent<VAttributeEffectUI>();
            ui.Initialize(attributeEffect, level);
            attributeEffects.Add(ui);
        }
    }
}