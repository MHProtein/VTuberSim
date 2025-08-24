using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Core.SaveSystem;
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

        [SerializeField] private List<VScriptConfiguration> scripts;
        [SerializeField] private List<VCharacterConfiguration> characters;
        [SerializeField] private VReincarnationConfiguration reincarnationConfiguration;
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

            List<VAccount> accounts = new List<VAccount>();

            for (int i = 0; i < 50; i++)
            { 
                accounts.Add(VAccountCreator.CreateAccount(reincarnationConfiguration, "S", gameManager.Character));
                
            }
            
            gameConfigSelection.Begin(scripts, characters, accounts);
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