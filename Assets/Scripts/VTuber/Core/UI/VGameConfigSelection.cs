using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.Reincarnation;
using VTuber.Relic;

namespace VTuber.BattleSystem.Core.UI
{
    public class VGameConfigSelection : VUIBehaviour
    {
        private List<VScriptConfiguration> configurations = new List<VScriptConfiguration>();
        private VScriptConfiguration currentConfiguration;
        private List<VCharacterConfiguration> characters = new List<VCharacterConfiguration>();
        private VCharacterConfiguration currentCharacter;
        private List<VAccount> accounts = new List<VAccount>();
        private List<VAccount> currentAccount;
        
        [SerializeField] private VScriptSelectionMenu _scriptSelectionMenu;

        public void Begin(List<VScriptConfiguration> scripts)
        {
            _scriptSelectionMenu.Initialize(scripts);
            _scriptSelectionMenu.Show();
        }
    }
}