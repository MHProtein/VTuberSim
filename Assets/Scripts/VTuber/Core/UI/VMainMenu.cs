using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VMainMenu : VUIBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private VGameConfigSelection gameConfigSelection;
        [SerializeField] private VGameManager gameManager;

        [SerializeField] private VReincarnationConfiguration reincarnationConfiguration;
        
        private List<VScriptConfiguration> _scripts;
        private List<VCharacterConfiguration> _characters;
        private List<VAccount> _accounts;
        private Action<VCharacterConfiguration, VScriptConfiguration, List<VAccount>> _startGame;

        protected override void Awake()
        {
            base.Awake();
            
            _startButton.onClick.AddListener(StartGame);
            _optionButton.onClick.AddListener(OpenOptionMenu);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void StartGame()
        {
            //VSave save = VSaveSystem.Load();
            
            gameConfigSelection.Begin(_scripts, _characters, _accounts, _startGame);
        }

        public void Initialize(List<VScriptConfiguration> scriptConfig, List<VCharacterConfiguration> characterConfiguration,
            List<VAccount> accounts, Action<VCharacterConfiguration, VScriptConfiguration, List<VAccount>> startGame)
        {
            _scripts = scriptConfig;
            _characters = characterConfiguration;
            _startGame = startGame;
            
            _accounts = accounts;
        }

        private void OpenOptionMenu()
        {
            
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}