using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlayTheSpire.System.SavingSystem;
using Tutorial.Script;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core.SaveSystem;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.BattleSystem.Core.UI;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.CoopSystem;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.ScriptSystem;
using VTuber.Core.StateMachine;
using VTuber.EventSystem;
using VTuber.Reincarnation;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;
using VTuber.Store;
using VTuber.Store.UI;

namespace VTuber.BattleSystem.Core
{
    public class VGameManager : VSingletonMonobehaviour<VGameManager>, IDataPersistence
    {
        [SerializeField] private bool useDevData;
        
        public VReincarnationConfiguration ReincarnationConfiguration => reincarnationConfiguration;
        [SerializeField] private VReincarnationConfiguration reincarnationConfiguration;
        
        [FormerlySerializedAs("schedule")]
        [Header("Schedule")] 
        [SerializeField] private VScheduleUI scheduleUI;
        [SerializeField] private VScheduleCreator scheduleCreator;

        private VWeeklySchedule _weeklySchedule; 
        
        [Space(5)]
        [Header("Battle")]
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private VBattle battle;
        [SerializeField] private VBattleConfiguration _battleConfiguration;
        [SerializeField] private VCharacterConfiguration _characterConfiguration;
        
        [Space(5)]
        [Header("EventSystem")]
        [SerializeField] private GameObject eventSystemRoot;
        [FormerlySerializedAs("eventSystemSystem")] [SerializeField] private VEventSystem eventSystem;

        [SerializeField] private VStoreUI _storeUI;
        [SerializeField] private VStoreConfiguration storeConfiguration;

        [Space(5)] [Header("MainMenu")]
        [SerializeField] private AssetLabelReference scriptLabel;
        [SerializeField] private AssetLabelReference characterLabel;
        
        [FormerlySerializedAs("_mainMenu")]
        [Space(5)] [Header("MainMenu")]
        [SerializeField] private VMainMenu mainMenu;
        
        private List<VScriptConfiguration> _scripts;
        private List<VCharacterConfiguration> _characterConfigs;
        
        public VCharacter Character => _character;
        private VCharacter _character;
        private VStateMachine _stateMachine;
        private VScript _script;

        private List<VAccount> _accounts;

        private bool _newGame = false;
        
        private DateTime _startGameTime;

        public bool IsTutorial => _isTutorial;
        private bool _isTutorial = false;
        
        public VTutorialScript TutorialScript => _tutorialScript;
        private VTutorialScript _tutorialScript;
        
        protected override void Awake()
        {
            base.Awake();
            VDataLoader loader;
            _accounts = new List<VAccount>();
            if (useDevData)
            {
                loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Cards.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Raising.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Relics.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Coop.xlsx"));
            }
            else
            {
                loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/Cards.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Raising.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Relics.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Coop.xlsx"));
            }

            _scripts = new List<VScriptConfiguration>();
            _characterConfigs = new List<VCharacterConfiguration>();
            Addressables.LoadAssetsAsync<VScriptConfiguration>(scriptLabel, scriptConfig =>
            {
                _scripts.Add(scriptConfig);
            }).Completed += handle => { };
            
            Addressables.LoadAssetsAsync<VCharacterConfiguration>(characterLabel, characterConfig =>
            {
                _characterConfigs.Add(characterConfig);
            }).Completed += handle => { };
            
            VResourcesManager.Instance.LoadSprites();
            loader.Load();
            VResourcesManager.Instance.LoadDialogs();
            DataPersistenceManager.Instance.Initialize();
            VSave save = VSaveSystem.Load();
            if(save != null)
            {
                _accounts = save.LoadAccounts();
            }
            
            DataPersistenceManager.Instance.Initialize();
            DataPersistenceManager.Instance.Register(this);

            var saveData = DataPersistenceManager.Instance.LoadSave();
            
            eventSystem.Initialize();
            
            _newGame = saveData == null;
            ReturnToMainMenu(saveData);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            if(_stateMachine is not null)
                _stateMachine.OnDisable();
            if(_character is not null)
                _character.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
            mainMenu.gameObject.SetActive(true);
        }

        public void LoadGame()
        {
            _startGameTime = DateTime.UtcNow;
            DataPersistenceManager.Instance.LoadGame();
            
            List<VScheduleEventConfiguration> eventConfigs = new List<VScheduleEventConfiguration>();
            eventConfigs.AddRange(_script.EventList.Select((id => VDataManager.Instance.GetDialogueEventConfigurationByID(id))));
            eventConfigs.AddRange(_script.StreamEventList.Select((id => VDataManager.Instance.GetStreamEventConfigurationByID(id))));

            scheduleCreator.InitializeCreator(_script);
            
            mainMenu.gameObject.SetActive(false);
        }

        public void NewGame(VCharacterConfiguration characterConfiguration, VScriptConfiguration scriptConfig, List<VAccount> accounts)
        {
            _startGameTime = DateTime.UtcNow;
            DataPersistenceManager.Instance.NewGame();

            if (scriptConfig is VTutorialScriptConfiguration)
            {
                _isTutorial = true;
                _tutorialScript = new VTutorialScript((VTutorialScriptConfiguration)scriptConfig);
                _script = _tutorialScript;
            }
            else
            {
                _script = new VScript(scriptConfig);
                scheduleCreator.InitializeCreator(_script);
            }
            
            _character = new VCharacter(characterConfiguration);
            _character.Initialize(false);

            foreach (var account in accounts)
            {
                foreach (var effect in account.Effects)
                {
                    effect.ApplyEffect(_character, null);
                }
            }
        
            _weeklySchedule = new VWeeklySchedule();
        
            // foreach (var config in VDataManager.Instance.GetAllCardConfigurations())
            // {
            //     if((config.liveType == "F" || config.liveType == _character.LiveType) && config.rarity == VCardRarity.Basic)
            //         _character.CardLibrary.AddCard(config.CreateCard());
            // }

            if (scriptConfig.cardIDs.TryGetValue(_character.LiveType, out var cardIDs))
            {
                foreach (var cardID in cardIDs)
                {
                    _character.CardLibrary.AddCard(VDataManager.Instance.GetCardConfigurationByID(cardID).CreateCard());
                }
            }
            else
            {
                VDebug.LogError("liveType not found in scriptConfig.cardIDs");
            }
        
            InitializeStateMachine();
        
            _stateMachine.OnEnable();
            _character.OnEnable();
        
            foreach (var configuration in scriptConfig.coops)
            {
                _character.CooperatorManager.AddCooperator(configuration);
            }
            
            scheduleUI.Initialize(_character, _script);
            _stateMachine.SwitchState(VStateType.PhaseStart, _script.BeginScript());
            
            mainMenu.gameObject.SetActive(false);
        }

        public void AddAccount(VAccount account)
        {
            _accounts.Add(account);
            var save = new VSave(_accounts);
            VSaveSystem.Save(save);
        }
        
        public void ModifySchedule()
        {
            _stateMachine.SwitchState(VStateType.ScheduleModify);
        }

        public void PauseSchedule()
        {
            _stateMachine.PauseSchedule();
        }

        public void ContinueSchedule()
        {
            _stateMachine.ContinueSchedule();
        }

        public void InitializeCardLibraryUI()
        {
            VRaisingUI.Instance.InitializeCardLibraryUI(_character.CardLibrary.GetCards());
        }
        
        public void InitializeStateMachine()
        {
            _stateMachine = new VStateMachine(scheduleUI, _weeklySchedule,
                battleRoot, eventSystemRoot, eventSystem,
                _character, _script, reincarnationConfiguration);
            _stateMachine.RegisterState(new VScheduleCreationState());
            _stateMachine.RegisterState(new VExecutionState());
            _stateMachine.RegisterState(new VPauseState());
            _stateMachine.RegisterState(new VScheduleModifyState());
            _stateMachine.RegisterState(new VPhaseStartState());
        }
        
        public void CloseCardLibraryUI()
        {
            VRaisingUI.Instance.CloseCardLibraryUI();
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ConvertToSchedule()
        {
            scheduleUI.CompleteSchedule(_character.FillingEventIDDuration1,
                _character.FillingEventIDDuration2,
                _character.FillingEventIDDuration3);
            
            var slots = scheduleUI.Slots;
            _weeklySchedule.Reset(false);
            for (int x = 0; x < slots.GetLength(1); x++)
            {
                for (int y = 0; y < slots.GetLength(0);)
                {
                    var slot = slots[y, x];
                    if (slot.Item is not null)
                    {
                        var evt = slot.Item.Event;
                        slot.Item.SetInteractive(false);
                        slot.TestSchedulingCondition(true);
                        _weeklySchedule.SetEvent(x, (TimeOfDay)y, evt);
                        y += evt.Duration;
                    }
                    else
                    {
                        ++y;
                    }
                }
            }
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnFinishScheduleCreationOrModification, new Dictionary<string, object>());

            if (_stateMachine.CurrentState.StateType == VStateType.ScheduleCreation)
            {
                _stateMachine.SwitchState(VStateType.Execution);
                
            }
            else if (_stateMachine.CurrentState.StateType == VStateType.ScheduleModify)
            {
                _stateMachine.SwitchState(VStateType.Pause);
            }
        }

        public void ReturnToMainMenu(SaveData data)
        {
            if (_stateMachine is not null)
            {
                _stateMachine.CurrentState.Exit(null);
                _stateMachine.OnDisable();
            } 
            if(_character is not null)
                _character.OnDisable();
            if(scheduleUI is not null)
                scheduleUI.Clear();
            
            mainMenu.gameObject.SetActive(true);
            mainMenu.Initialize(false, _scripts, _characterConfigs, _accounts);
        }

        public void ReturnToMainMenu()
        {
            if (_stateMachine is not null)
            {
                _stateMachine.CurrentState.Exit(null);
                _stateMachine.OnDisable();
            } 
            if(_character is not null)
                _character.OnDisable();
            if(scheduleUI is not null)
                scheduleUI.Clear();
            
            mainMenu.gameObject.SetActive(true);
            mainMenu.Initialize(true, _scripts, _characterConfigs, _accounts);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSwitchToMainMenu, new Dictionary<string, object>());
        }
        
        public void Load(SaveData data)
        {
            VCardConfiguration.LoadIDDistributor(data.cardIDDistributor);
            VConsumableConfiguration.LoadIDDistributor(data.consumableIDDistributor);
            _accounts = new List<VAccount>();
            foreach (var saveData in data.accounts)
            {
                _accounts.Add(new VAccount(saveData));
            }

            _character = new VCharacter(null);
            _character.Load(data, _characterConfigs.Find(config
                => config.name == data.characterSaveData.characterConfigurationName));
            
            var scriptConfig = GetScriptConfig(data.script.scriptConfigurationName);
            
            _script = VScript.Load(data.script, scriptConfig);
            
            // if (scriptConfig is VTutorialScriptConfiguration)
            // {
            //     _isTutorial = true;
            //     _tutorialScript = new VTutorialScript((VTutorialScriptConfiguration)scriptConfig);
            //     
            //     _script = _tutorialScript;
            // }
            // else
            // {
            //     _script = VScript.Load(data.script, scriptConfig);
            // }
            
            _weeklySchedule = VWeeklySchedule.Load(data.weeklySchedule, _script);
            
            scheduleUI.Initialize(_character, _script);
            scheduleUI.Load(data);
            scheduleUI.LoadEvents(_weeklySchedule);
            
            eventSystem.Load(data);
            
            InitializeStateMachine();
            _stateMachine.Load(data.stateMachine);
            
            _stateMachine.OnEnable();
            _character.OnEnable();
        }

        public void Save(SaveData data)
        {
            data.lastPlayTime = DateTime.UtcNow - _startGameTime;
            data.accounts = new List<VAccountSaveData>();
            foreach (var account in _accounts)
            {
                data.accounts.Add(account.Save());
            }
            
            _character.Save(data);
            data.weeklySchedule = _weeklySchedule.Save(_script);
            data.stateMachine = _stateMachine.Save();
            data.script = _script.Save();
            scheduleUI.Save(data);
            eventSystem.Save(data);
            
            data.battleSaveData = battle.Save();

            data.cardIDDistributor = VCardConfiguration.IDDistributor;
            data.consumableIDDistributor = VConsumableConfiguration.IDDistributor;
            
        }

        public VScriptConfiguration GetScriptConfig(string scriptScriptConfigurationName)
        {
            return _scripts.Find(config => config.name == scriptScriptConfigurationName);
        }

        public VCharacterConfiguration GetCharacterConfig(string characterConfigurationName)
        {
            return _characterConfigs.Find(config => config.name == characterConfigurationName);
        }
    }
}