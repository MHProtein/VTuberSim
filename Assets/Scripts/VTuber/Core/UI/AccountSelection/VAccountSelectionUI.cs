using System;
using System.Collections.Generic;
using System.Linq;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Consumable;
using VTuber.Core.Foundation;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountSelectionMenu : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private GameObject accountPrefab;
        [SerializeField] private Transform accountGrids;
        [SerializeField] private Transform unpickPosition;
        [SerializeField] private List<VAccountSlot> accountSlots;
        [SerializeField] private List<VClickDetectionPanel> detectionPanels;
        [SerializeField] private VAccountSelectionCharacterUI characterUI;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button returnButton;

        private List<VAccount> _accounts;
        private List<VAccountUI> _accountUIs;

        private Action<List<VAccount>> _confirmAction;
        private VGameConfigSelection _gameConfigSelection;
        private Action _returnAction;
        private List<VAccountUI> _selectedAccounts;

        protected override void Awake()
        {
            base.Awake();
            foreach (var detectionPanel in detectionPanels) detectionPanel.onClick += UnselectAll;
            confirmButton.onClick.AddListener(Confirm);
            returnButton.onClick.AddListener(Return);
        }

        public bool IsFull()
        {
            return _selectedAccounts.Count == accountSlots.Count;
        }

        public void ActivatePanels(bool value)
        {
            foreach (var panel in detectionPanels) panel.gameObject.SetActive(value);
        }

        public void Initialize(VGameConfigSelection gameConfigSelection, List<VAccount> accounts,
            Action<List<VAccount>> confirmAction, Action returnAction)
        {
            _confirmAction = confirmAction;
            _returnAction = returnAction;
            _gameConfigSelection = gameConfigSelection;
            _accounts = accounts;
            _selectedAccounts = new List<VAccountUI>();
            Spawn();
        }

        public void Confirm()
        {
            _confirmAction?.Invoke(_selectedAccounts.Select(accountUI => accountUI.Account).ToList());
        }

        public void Return()
        {
            _returnAction?.Invoke();
        }

        public void Spawn()
        {
            _accountUIs = new List<VAccountUI>();
            foreach (var account in _accounts) SpawnAccount(account);
        }

        private void SpawnAccount(VAccount account)
        {
            var accountUI = Instantiate(accountPrefab, accountGrids).GetComponent<VAccountUI>();
            accountUI.Initialize(this, account);
            _accountUIs.Add(accountUI);
        }

        public void Show()
        {
            characterUI.SetCharacter(_gameConfigSelection.SelectedCharacter);
            ui.SetActive(true);
        }

        public void Hide()
        {
            ui.SetActive(false);
        }

        public void UnselectAll()
        {
            foreach (var accountUI in _accountUIs) accountUI.Deselect();
            ActivatePanels(false);
        }

        public void PickAccount(VAccountUI account)
        {
            VAccountSlot accountSlot = null;
            foreach (var slot in accountSlots)
                if (!slot.HasAccountUI())
                {
                    accountSlot = slot;
                    slot.SetAccountUI(account);
                    break;
                }

            _selectedAccounts.Add(account);

            if (IsFull())
                foreach (var accountUI in _accountUIs)
                    accountUI.SetSelectable(false);

            if (accountSlot == null)
                return;

            characterUI.SetAccounts(_selectedAccounts);
            account.transform.SetParent(accountSlot.transform);
            Tween.LocalPosition(account.transform, Vector3.zero, 0.4f);
            Tween.Scale(account.transform, Vector3.one * 0.57f, 0.4f);
        }

        public void OnSelected(VAccountUI accountUI)
        {
            foreach (var ui in _accountUIs)
            {
                if (ui == accountUI)
                    continue;
                ui.Deselect();
            }

            ActivatePanels(true);
        }

        public void UnpickAccount(VAccountUI accountUI)
        {
            foreach (var slot in accountSlots)
                if (slot.HasAccountUI() && slot.Account == accountUI)
                {
                    slot.RemoveAccountUI();
                    break;
                }

            _selectedAccounts.Remove(accountUI);

            foreach (var ui in _accountUIs) ui.SetSelectable(true);

            characterUI.SetAccounts(_selectedAccounts);

            accountUI.transform.localScale = Vector3.one;
            Tween.LocalPosition(accountUI.transform, unpickPosition.localPosition, 0.4f);
            Tween.Scale(accountUI.transform, Vector3.one, 0.35f)
                .OnComplete(() => accountUI.transform.SetParent(accountGrids));
        }

        public void Clear()
        {
            foreach (var accountSlot in accountSlots) accountSlot.RemoveAccountUI();
            foreach (var accountUI in _accountUIs) Destroy(accountUI.gameObject);

            foreach (var selectedAccount in _selectedAccounts) Destroy(selectedAccount.gameObject);
            _accountUIs.Clear();
            _selectedAccounts.Clear();
        }
    }
}