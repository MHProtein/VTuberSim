using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SlayTheSpire.System.SavingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;
using VTuber.Core.SE;
using VTuber.Reincarnation;

namespace VTuber.BattleSystem.Core.UI
{
    public class VMainMenu : VUIBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private TMP_Text _startButtonText;
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _optionButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private VGameConfigSelection gameConfigSelection;
        [SerializeField] private VGameManager gameManager;
        [SerializeField] private VLoadSaveMenu loadSaveMenu;
        [SerializeField] private VConfirmationMenu confirmationMenu;
        
        [Space(5)]
        [Header("消息")]
        [TextArea][LabelText("删除存档确认")][SerializeField] private string deleteSaveConfirmationText;
        [TextArea][LabelText("新游戏确认")][SerializeField] private string newGameConfirmationText;
        [TextArea][LabelText("新游戏二次确认")][SerializeField] private string newGameConfirmationTwiceText;
        
        private List<VScriptConfiguration> _scripts;
        private List<VCharacterConfiguration> _characters;
        private List<VAccount> _accounts;

        protected override void Awake()
        {
            base.Awake();
            
            _startButton.onClick.AddListener(NewGame);
            _loadGameButton.onClick.AddListener(OpenLoadMenu);
            _optionButton.onClick.AddListener(OpenOptionMenu);
            _exitButton.onClick.AddListener(ExitGame);
            
            loadSaveMenu.confirmButton.onClick.AddListener(ConfirmLoad);
            loadSaveMenu.returnButton.onClick.AddListener(CloseLoadMenu);
            loadSaveMenu.deleteSaveButton.onClick.AddListener(DeleteSave);
        }

        private void DeleteSave()
        {
            confirmationMenu.Show("删除存档", new List<string> { deleteSaveConfirmationText }, () =>
            {
                VDataPersistenceManager.Instance.DeleteSave();
                CloseLoadMenu();
                _loadGameButton.interactable = false;
            });
        }

        private void OpenLoadMenu()
        {
            loadSaveMenu.gameObject.SetActive(true);
            loadSaveMenu.SetDetails(VDataPersistenceManager.Instance.SaveData);
        }

        private void CloseLoadMenu()
        {
            loadSaveMenu.gameObject.SetActive(false);
        }

        private void ConfirmLoad()
        {
            loadSaveMenu.gameObject.SetActive(false);
            gameManager.LoadGame();
            AudioManager.Instance.StopSoundsByChannel(SoundChannel.Music);
        }

        private void NewGame()
        {
            //VSave save = VSaveSystem.Load();
            if (VDataPersistenceManager.Instance.SaveData is null)
            {
                VDataPersistenceManager.Instance.NewGame();
                gameConfigSelection.Begin(_scripts, _characters, _accounts);
                return;
            }
            
            confirmationMenu.Show("新游戏", new List<string> { newGameConfirmationText }, () =>
            {
                confirmationMenu.Show("新游戏", new List<string> { newGameConfirmationTwiceText }, () =>
                {
                    VDataPersistenceManager.Instance.NewGame();
                    gameConfigSelection.Begin(_scripts, _characters, _accounts);
                });
            });
        }

        public void Initialize(bool isReturn, List<VScriptConfiguration> scriptConfig, List<VCharacterConfiguration> characterConfiguration,
            List<VAccount> accounts)
        {
            _scripts = scriptConfig;
            _characters = characterConfiguration;
            
            _accounts = accounts;

            _loadGameButton.interactable = VDataPersistenceManager.Instance.SaveData is not null;
            VAudioPlayer.Instance.PlayBGM(VBGMType.MainMenu);
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