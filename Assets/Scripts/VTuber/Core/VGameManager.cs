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
using VTuber.BattleSystem.Core.UI;
using VTuber.Character;
using VTuber.Consumable;
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
        [SerializeField] private VReincarnationConfiguration reincarnationConfiguration;

        [FormerlySerializedAs("schedule")] [Header("Schedule")] [SerializeField]
        private VScheduleUI scheduleUI;

        [SerializeField] private VScheduleCreator scheduleCreator;

        private VWeeklySchedule _weeklySchedule;

        [Space(5)] [Header("Battle")] [SerializeField]
        private GameObject battleRoot;

        [SerializeField] private VBattle battle;
        [SerializeField] private VBattleConfiguration _battleConfiguration;
        [SerializeField] private VCharacterConfiguration _characterConfiguration;

        [Space(5)] [Header("EventSystem")] [SerializeField]
        private GameObject eventSystemRoot;

        [FormerlySerializedAs("eventSystemSystem")] [SerializeField]
        private VEventSystem eventSystem;

        [SerializeField] private VStoreUI _storeUI;
        [SerializeField] private VStoreConfiguration storeConfiguration;

        [Space(5)] [Header("MainMenu")] [SerializeField]
        private AssetLabelReference scriptLabel;

        [SerializeField] private AssetLabelReference characterLabel;

        [FormerlySerializedAs("_mainMenu")] [Space(5)] [Header("MainMenu")] [SerializeField]
        private VMainMenu mainMenu;

        private List<VScriptConfiguration> _scripts;
        private List<VCharacterConfiguration> _characterConfigs;
        private VStateMachine _stateMachine;
        private VScript _script;

        private List<VAccount> _accounts;

        private bool _newGame;

        private DateTime _startGameTime;

        public VReincarnationConfiguration ReincarnationConfiguration => reincarnationConfiguration;

        public VCharacter Character { get; private set; }

        public bool IsTutorial { get; private set; }

        public VTutorialScript TutorialScript { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            VDataLoader loader;
            _accounts = new List<VAccount>();
            if (useDevData)
                loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Cards.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Raising.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Relics.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/dev/Coop.xlsx"));
            else
                loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/Cards.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Raising.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Relics.xlsx"),
                    Path.Combine(Application.streamingAssetsPath, "Configurations/Coop.xlsx"));

            _scripts = new List<VScriptConfiguration>();
            _characterConfigs = new List<VCharacterConfiguration>();
            Addressables
                .LoadAssetsAsync<VScriptConfiguration>(scriptLabel, scriptConfig => { _scripts.Add(scriptConfig); })
                .Completed += handle => { };

            Addressables.LoadAssetsAsync<VCharacterConfiguration>(characterLabel,
                characterConfig => { _characterConfigs.Add(characterConfig); }).Completed += handle => { };

            VResourcesManager.Instance.LoadSprites();
            loader.Load();
            VResourcesManager.Instance.LoadDialogs();
            VDataPersistenceManager.Instance.Initialize();

            VDataPersistenceManager.Instance.Initialize();
            VDataPersistenceManager.Instance.Register(this);

            var saveData = VDataPersistenceManager.Instance.LoadSave();

            eventSystem.Initialize();

            _newGame = saveData == null;
            OpenMainMenu();
        }

        protected override void Start()
        {
            base.Start();
            mainMenu.gameObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_stateMachine is not null)
                _stateMachine.OnDisable();
            if (Character is not null)
                Character.OnDisable();
        }

        public void Load(SaveData data)
        {
            VCardConfiguration.LoadIDDistributor(data.cardIDDistributor);
            VConsumableConfiguration.LoadIDDistributor(data.consumableIDDistributor);
            _accounts = new List<VAccount>();
            foreach (var saveData in data.accounts) _accounts.Add(new VAccount(saveData));

            Character = new VCharacter(null);
            Character.Load(data, _characterConfigs.Find(config
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

            scheduleUI.Initialize(Character, _script);
            scheduleUI.Load(data);
            scheduleUI.LoadEvents(_weeklySchedule);

            eventSystem.Load(data);

            InitializeStateMachine();
            _stateMachine.Load(data.stateMachine);

            _stateMachine.OnEnable();
            Character.OnEnable();
        }

        public void Save(SaveData data)
        {
            data.lastPlayTime = DateTime.UtcNow - _startGameTime;
            data.accounts = new List<VAccountSaveData>();
            foreach (var account in _accounts) data.accounts.Add(account.Save());

            Character.Save(data);
            data.weeklySchedule = _weeklySchedule.Save(_script);
            data.stateMachine = _stateMachine.Save();
            data.script = _script.Save();
            scheduleUI.Save(data);
            eventSystem.Save(data);

            data.battleSaveData = battle.Save();

            data.cardIDDistributor = VCardConfiguration.IDDistributor;
            data.consumableIDDistributor = VConsumableConfiguration.IDDistributor;
        }

        public void LoadGame(bool isTutorialSave = false)
        {
            scheduleCreator.gameObject.SetActive(true);
            _startGameTime = DateTime.UtcNow;

            if (isTutorialSave)
                VDataPersistenceManager.Instance.LoadTutorialWeekGame();
            else
                VDataPersistenceManager.Instance.LoadGame(IsTutorial);

            var eventConfigs = new List<VScheduleEventConfiguration>();
            eventConfigs.AddRange(_script.EventList.Select(id =>
                VDataManager.Instance.GetDialogueEventConfigurationByID(id)));
            eventConfigs.AddRange(
                _script.StreamEventList.Select(id => VDataManager.Instance.GetStreamEventConfigurationByID(id)));

            scheduleCreator.InitializeCreator(_script);

            mainMenu.gameObject.SetActive(false);
        }

        public void NewGame(VCharacterConfiguration characterConfiguration, VScriptConfiguration scriptConfig,
            List<VAccount> accounts)
        {
            scheduleCreator.gameObject.SetActive(true);
            _startGameTime = DateTime.UtcNow;

            if (scriptConfig is VTutorialScriptConfiguration)
            {
                IsTutorial = true;
                TutorialScript = new VTutorialScript((VTutorialScriptConfiguration)scriptConfig);
                _script = TutorialScript;
                scheduleCreator.InitializeTutorialCreator(TutorialScript);
            }
            else
            {
                _script = new VScript(scriptConfig);
                scheduleCreator.InitializeCreator(_script);
            }
            VDataPersistenceManager.Instance.NewGame(IsTutorial);

            Character = new VCharacter(characterConfiguration);
            Character.Initialize(false);

            foreach (var account in accounts)
            foreach (var effect in account.Effects)
                effect.ApplyEffect(Character, null);

            _weeklySchedule = new VWeeklySchedule();

            // foreach (var config in VDataManager.Instance.GetAllCardConfigurations())
            // {
            //     if((config.liveType == "F" || config.liveType == _character.LiveType) && config.rarity == VCardRarity.Basic)
            //         _character.CardLibrary.AddCard(config.CreateCard());
            // }

            if (scriptConfig.cardIDs.TryGetValue(Character.LiveType, out var cardIDs))
                foreach (var cardID in cardIDs)
                    Character.CardLibrary.AddCard(VDataManager.Instance.GetCardConfigurationByID(cardID).CreateCard());
            else
                VDebug.LogError("liveType not found in scriptConfig.cardIDs");

            InitializeStateMachine();

            _stateMachine.OnEnable();
            Character.OnEnable();

            foreach (var configuration in scriptConfig.coops) Character.CooperatorManager.AddCooperator(configuration);

            scheduleUI.Initialize(Character, _script);
            _stateMachine.SwitchState(VStateType.PhaseStart, _script.BeginScript());

            mainMenu.gameObject.SetActive(false);
        }

        public void AddAccount(VAccount account)
        {
            _accounts.Add(account);
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
            VRaisingUI.Instance.InitializeCardLibraryUI(Character.CardLibrary.GetCards());
        }

        public void InitializeStateMachine()
        {
            _stateMachine = new VStateMachine(IsTutorial, scheduleUI, _weeklySchedule,
                battleRoot, eventSystemRoot, eventSystem,
                Character, _script, reincarnationConfiguration);
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
            scheduleUI.CompleteSchedule(Character.FillingEventIDDuration1,
                Character.FillingEventIDDuration2,
                Character.FillingEventIDDuration3);

            var slots = scheduleUI.Slots;
            _weeklySchedule.Reset(false);
            for (var x = 0; x < slots.GetLength(1); x++)
            for (var y = 0; y < slots.GetLength(0);)
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

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnFinishScheduleCreationOrModification,
                new Dictionary<string, object>());

            if (_stateMachine.CurrentState.StateType == VStateType.ScheduleCreation)
                _stateMachine.SwitchState(VStateType.Execution);
            else if (_stateMachine.CurrentState.StateType == VStateType.ScheduleModify)
                _stateMachine.SwitchState(VStateType.Pause);
        }

        public void ReturnToMainMenu()
        {
            ResetGame();
            mainMenu.gameObject.SetActive(true);
            mainMenu.Initialize(true, _scripts, _characterConfigs, _accounts);
        }

        public void OpenMainMenu()
        {
            mainMenu.gameObject.SetActive(true);
            mainMenu.Initialize(true, _scripts, _characterConfigs, _accounts);
        }

        public void TutorialRestartWeek()
        {
            ResetGame();
            LoadGame(true);
        }

        private void ResetGame()
        {
            if (_stateMachine is not null)
            {
                _stateMachine.CurrentState.Exit(null);
                _stateMachine.OnDisable();
            }

            if (Character is not null)
                Character.OnDisable();
            if (scheduleUI is not null)
                scheduleUI.Clear();

            eventSystem.CloseUI();
            scheduleCreator.gameObject.SetActive(false);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnReset, new Dictionary<string, object>());
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