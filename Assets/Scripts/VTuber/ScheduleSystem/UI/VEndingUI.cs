using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
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
        [SerializeField] private InputField inputField;
        [SerializeField] private Button continueButton;
        
        private List<VCardUI> _cards;
        private List<VRelicSlotUI> _relics;
        private List<VAttributeEffectUI> _attributeEffects;
        private VAccount _account;

        protected override void Awake()
        {
            base.Awake();
            continueButton.onClick.AddListener(OnNextButtonClicked);
        }

        public void Initialize(string ratingLevel, int score, VAccount account)
        {
            _account = account;
            inputField.characterLimit = 10;
            ratingLevelText.text = ratingLevel;
            scoreText.text = score.ToString();
            
            _cards = new List<VCardUI>();
            _relics = new List<VRelicSlotUI>();
            _attributeEffects = new List<VAttributeEffectUI>();
            
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
            _cards.Add(ui);
        }

        private void SpawnRelic(VRelic relic)
        {
            var go = Instantiate(relicPrefab, relicGrid);
            var ui = go.GetComponent<VRelicSlotUI>();
            ui.Initialize(relic, false);
            _relics.Add(ui);
        }

        private void SpawnAttributeEffect(VRaisingEffect attributeEffect, int level)
        {
            var go = Instantiate(attributeEffectPrefab, attributeEffectGrid);
            var ui = go.GetComponent<VAttributeEffectUI>();
            ui.Initialize(attributeEffect, level);
            _attributeEffects.Add(ui);
        }

        public void OnNextButtonClicked()
        {
            foreach (var card in _cards)
            {
                Destroy(card.gameObject);
            }

            foreach (var relic in _relics)
            {
                Destroy(relic.gameObject);
            }

            foreach (var attributeEffect in _attributeEffects)
            {
                Destroy(attributeEffect.gameObject);
            }
            
            _cards.Clear();
            _relics.Clear();
            _attributeEffects.Clear();
            
            _account.accountName = inputField.text;
            VGameManager.Instance.AddAccount(_account);
            VGameManager.Instance.ChangeToMainMenu();
        }
    }
}