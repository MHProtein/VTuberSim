using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI.VAccountSelection
{
    public class VAccountSelectionMenu : VUIBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private GameObject accountPrefab;
        [SerializeField] private Transform accountGrids;
        
        List<VAccount> _accounts;

        public void Initialize(List<VAccount> accounts)
        {
            _accounts = accounts;
            foreach (var account in _accounts)
            {
                SpawnAccount(account);
            }
        }

        public async void Spawn()
        {
            try
            {
            }
            catch (Exception e)
            {
                VDebug.LogError(e.Message);
            }
        }

        private void SpawnAccount(VAccount account)
        {
            var accountUI = Instantiate(accountPrefab, accountGrids).GetComponent<VAccountUI>();
            accountUI.Initialize(this, account);
        }

        public void Show()
        {
            ui.SetActive(true);
        }

        public void Hide()
        {
            ui.SetActive(false);
        }
    }
}