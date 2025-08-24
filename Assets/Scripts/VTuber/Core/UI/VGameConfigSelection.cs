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
        
        private List<VScriptConfiguration> configurations = new List<VScriptConfiguration>();
        private VScriptConfiguration currentConfiguration;
        private List<VCharacterConfiguration> characters = new List<VCharacterConfiguration>();
        private VCharacterConfiguration currentCharacter;
        private List<VAccount> accounts = new List<VAccount>();
        private List<VAccount> currentAccount;

        private VScriptConfiguration selectedScript;
        private VCharacterConfiguration selectedCharacter;
        private Action _returnAction;

        public void Begin(List<VScriptConfiguration> scripts, List<VCharacterConfiguration> characters, List<VAccount> accounts)
        {
            _scriptSelectionMenu.Initialize(scripts, ScriptSelectionMenuConfirm, ScriptSelectionMenuReturn);
            _characterSelectionMenu.Initialize(characters, CharacterSelectionMenuConfirm, CharacterSelectionMenuReturn);
            _accountSelectionMenu.Initialize(accounts);
            _scriptSelectionMenu.Show();
        }

        public void ScriptSelectionMenuConfirm(VScriptConfiguration script)
        {
            selectedScript = script;
            
            _scriptSelectionMenu.Hide();
            _characterSelectionMenu.Show();
        }

        public void ScriptSelectionMenuReturn()
        {
            _scriptSelectionMenu.Hide();
        }

        public void CharacterSelectionMenuConfirm(VCharacterConfiguration character)
        {
            selectedCharacter = character;
            _characterSelectionMenu.Hide();
            _accountSelectionMenu.Show();
        }

        public void CharacterSelectionMenuReturn()
        {
            _characterSelectionMenu.Hide();
            _scriptSelectionMenu.Show();
        }
    }
}