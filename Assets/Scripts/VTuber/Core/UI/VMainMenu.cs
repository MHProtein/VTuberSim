using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;

namespace VTuber.BattleSystem.Core.UI
{
    public class VMainMenu : VUIBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private VGameConfigSelection gameConfigSelection;

        [SerializeField] private List<VScriptConfiguration> scripts;
        protected override void Awake()
        {
            base.Awake();
            
            _startButton.onClick.AddListener(StartGame);
            _optionButton.onClick.AddListener(OpenOptionMenu);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void StartGame()
        {
            gameConfigSelection.Begin(scripts);
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