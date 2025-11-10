using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core.UI.VAccountSelection;
using VTuber.BattleSystem.Core.UI.VCharacterSelection;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.Core.SE;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VGameConfigSelection : VUIBehaviour
    {
        [SerializeField] private VScriptSelectionMenu _scriptSelectionMenu;
        [SerializeField] private VCharacterSelectionMenu _characterSelectionMenu;
        [SerializeField] private VAccountSelectionMenu _accountSelectionMenu;
        private Action _returnAction;

        public VScriptConfiguration SelectedScript { get; private set; }

        public VCharacterConfiguration SelectedCharacter { get; private set; }

        public List<VAccount> SelectedAccounts { get; private set; }

        public void Begin(List<VScriptConfiguration> scripts, List<VCharacterConfiguration> characters,
            List<VAccount> accounts)
        {
            _scriptSelectionMenu.Initialize(scripts, ScriptSelectionMenuConfirm, ScriptSelectionMenuReturn);
            _characterSelectionMenu.Initialize(characters, CharacterSelectionMenuConfirm, CharacterSelectionMenuReturn);
            _accountSelectionMenu.Initialize(this, accounts, AccountSelectionMenuConfirm, AccountSelectionMenuReturn);
            _scriptSelectionMenu.Show();
        }

        public void ScriptSelectionMenuConfirm(VScriptConfiguration script)
        {
            SelectedScript = script;

            _scriptSelectionMenu.Hide();
            _characterSelectionMenu.Show();
        }

        public void ScriptSelectionMenuReturn()
        {
            _scriptSelectionMenu.Clear();
            _accountSelectionMenu.Clear();
            _scriptSelectionMenu.Hide();
        }

        public void CharacterSelectionMenuConfirm(VCharacterConfiguration character)
        {
            SelectedCharacter = character;
            _characterSelectionMenu.Hide();
            _accountSelectionMenu.Show();
        }

        public void CharacterSelectionMenuReturn()
        {
            _characterSelectionMenu.Hide();
            _scriptSelectionMenu.Show();
        }

        public void AccountSelectionMenuConfirm(List<VAccount> selectedAccounts)
        {
            SelectedAccounts = selectedAccounts;

            _accountSelectionMenu.Hide();
            VGameManager.Instance.NewGame(SelectedCharacter, SelectedScript, selectedAccounts);
            VAudioPlayer.Instance.StopBGM();
            _scriptSelectionMenu.Clear();
            _accountSelectionMenu.Clear();
        }

        public void AccountSelectionMenuReturn()
        {
            _accountSelectionMenu.Hide();
            _characterSelectionMenu.Show();
        }
    }
}