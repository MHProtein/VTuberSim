using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.SE;
using VTuber.Relic;
using VTuber.Relic.UI;

namespace VTuber.BattleSystem.UI
{
    public class VPickRelicMenu : VUIBehaviour
    {
        [SerializeField] private TMP_Text SelectRelicText;
        [SerializeField] private Button ConfirmButton;
        [SerializeField] private GameObject relicSlotUI;
        [SerializeField] private Transform content;
        private List<VRelicSlotUI> _relicUIs;

        private int _maxPickCount = 3;
        private List<VRelic> _pickedRelics;
        private bool _shouldPlayTwice;

        private Action<List<VRelic>> _onRelicsPicked;

        protected override void Awake()
        {
            base.Awake();
            ConfirmButton.onClick.AddListener(ConfirmSelection);
        }

        public void BeginPickRelic(List<VRelic> relicsToSpawn, int maxPickCount, Action<List<VRelic>> onRelicsPicked)
        {
            _relicUIs = new List<VRelicSlotUI>();
            _pickedRelics = new List<VRelic>();
            _onRelicsPicked = onRelicsPicked;
            ConfirmButton.interactable = true;
            _maxPickCount = maxPickCount;
            SelectRelicText.text = "Remaining picks: " + maxPickCount;

            foreach (var relic in relicsToSpawn)
            {
                var relicSlot = Instantiate(relicSlotUI, content).GetComponent<VRelicSlotUI>();
                relicSlot.Initialize(relic, false);
                var pickableRelicCompoent = relicSlot.gameObject.AddComponent<VPickableRelicComponent>();
                pickableRelicCompoent.Initialize(relicSlot, this);
                relicSlot.ShowDescriptionPermenant();
                _relicUIs.Add(relicSlot);
            }
        }

        public bool SelectCard(VRelic pickCard)
        {
            VAudioPlayer.Instance.PlayStaticSFX(VSFXType.Selection);
            if (_pickedRelics.Count >= _maxPickCount)
                return false;

            if (pickCard != null) _pickedRelics.Add(pickCard);
            SelectRelicText.text = "Remaining picks: " + (_maxPickCount - _pickedRelics.Count);

            return true;
        }

        public void RemoveCard(VRelic pickCard)
        {
            if (_pickedRelics.Contains(pickCard))
            {
                _pickedRelics.Remove(pickCard);
                SelectRelicText.text = "Remaining picks: " + (_maxPickCount - _pickedRelics.Count);
            }
        }

        public void ConfirmSelection()
        {
            SelectRelicText.text = $"Selected {_pickedRelics.Count} relics.";
            ConfirmButton.interactable = false;

            _onRelicsPicked?.Invoke(_pickedRelics);

            foreach (var cardUI in _relicUIs) Destroy(cardUI.gameObject);
            _relicUIs.Clear();
            _pickedRelics.Clear();
        }
    }
}