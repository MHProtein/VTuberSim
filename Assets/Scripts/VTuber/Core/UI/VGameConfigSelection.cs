using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core.UI.VAccountSelection;
using VTuber.BattleSystem.Core.UI.VCharacterSelection;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.Reincarnation;
using VTuber.Relic;

namespace VTuber.BattleSystem.Core.UI
{
    public class VGameConfigSelection : VUIBehaviour
    {
        [SerializeField] private VScriptSelectionMenu _scriptSelectionMenu;
        [SerializeField] private VCharacterSelectionMenu _characterSelectionMenu;
        [SerializeField] private VAccountSelectionMenu _accountSelectionMenu;

        public VScriptConfiguration SelectedScript => _selectedScript;
        private VScriptConfiguration _selectedScript;
        public VCharacterConfiguration SelectedCharacter => _selectedCharacter;
        private VCharacterConfiguration _selectedCharacter;
        
        public List<VAccount> SelectedAccounts => _selectedAccounts;
        private List<VAccount> _selectedAccounts;
        private Action _returnAction;
        
        private Action<VCharacterConfiguration, VScriptConfiguration, List<VAccount>> _startGame;

        public void Begin(List<VScriptConfiguration> scripts, List<VCharacterConfiguration> characters, List<VAccount> accounts,
            Action<VCharacterConfiguration, VScriptConfiguration, List<VAccount>> startGame)
        {
            _startGame = startGame;
            _scriptSelectionMenu.Initialize(scripts, ScriptSelectionMenuConfirm, ScriptSelectionMenuReturn);
            _characterSelectionMenu.Initialize(characters, CharacterSelectionMenuConfirm, CharacterSelectionMenuReturn);
            _accountSelectionMenu.Initialize(this, accounts, AccountSelectionMenuConfirm, AccountSelectionMenuReturn);
            _scriptSelectionMenu.Show();
        }

        public void ScriptSelectionMenuConfirm(VScriptConfiguration script)
        {
            _selectedScript = script;
            
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
            _selectedCharacter = character;
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
            _selectedAccounts = selectedAccounts;
            
            _accountSelectionMenu.Hide();
            _startGame?.Invoke(_selectedCharacter, _selectedScript, selectedAccounts);
        }

        public void AccountSelectionMenuReturn()
        {
            _accountSelectionMenu.Hide();
            _characterSelectionMenu.Show();
        }
    }
}