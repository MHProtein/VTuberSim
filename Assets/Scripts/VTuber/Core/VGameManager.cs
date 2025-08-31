using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Core.SaveSystem;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.BattleSystem.Core.UI;
using VTuber.Character;
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
    public class VGameManager : VSingletonMonobehaviour<VGameManager>
    {
        [SerializeField] private bool dev;
        [SerializeField] private List<VCooperatorConfiguration> cooperatorConfigurations;
        [FormerlySerializedAs("script")] [SerializeField] private VScriptConfiguration scriptConfiguration;
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
        [FormerlySerializedAs("eventSystem")] [SerializeField] private VEventSystem eventSystemSystem;

        [SerializeField] private VStoreUI _storeUI;
        [SerializeField] private VStoreConfiguration storeConfiguration;

        [Space(5)] [Header("MainMenu")] [SerializeField]
        private VMainMenu _mainMenu;
        [SerializeField] private List<VScriptConfiguration> scripts;
        [SerializeField] private List<VCharacterConfiguration> characters;
        
        
        public VCharacter Character => _character;
        private VCharacter _character;
        private VStateMachine _stateMachine;
        private VScript _script;

        private List<VAccount> _accounts;
        
        protected override void Awake()
        {
            base.Awake();
            VDataLoader loader;
            _accounts = new List<VAccount>();
            if (dev)
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
            
            VResourcesManager.Instance.LoadSprites();
            loader.Load();
            VResourcesManager.Instance.LoadDialogs();
            
            VSave save = VSaveSystem.Load();
            if(save != null)
            {
                _accounts = save.LoadAccounts();
            }
            
            ChangeToMainMenu();
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
            _mainMenu.gameObject.SetActive(true);
        }

        public void InitializeGame(VCharacterConfiguration characterConfiguration, VScriptConfiguration scriptConfig, List<VAccount> accounts)
        {
            _script = new VScript(scriptConfig);
            _character = new VCharacter(characterConfiguration);
            _character.Initialize();

            foreach (var account in accounts)
            {
                foreach (var effect in account.Effects)
                {
                    effect.ApplyEffect(_character, null);
                }
            }
            
            _weeklySchedule = new VWeeklySchedule(_character);

            foreach (var config in VDataManager.Instance.GetAllCardConfigurations())
            {
                if((config.liveType == "F" || config.liveType == _character.LiveType) && config.rarity == VCardRarity.Basic)
                    _character.CardLibrary.AddCard(config.CreateCard());
            }
            
            _stateMachine = new VStateMachine(scheduleUI, _weeklySchedule,
                battleRoot, battle, eventSystemRoot, eventSystemSystem,
                _character, _script, reincarnationConfiguration);
            _stateMachine.RegisterState(new VScheduleCreationState());
            _stateMachine.RegisterState(new VExecutionState());
            _stateMachine.RegisterState(new VPauseState());
            _stateMachine.RegisterState(new VScheduleModifyState());
            _stateMachine.RegisterState(new VPhaseStartState());
            
            _stateMachine.OnEnable();
            _character.OnEnable();
            
            scheduleUI.Initialize(_character, _script);
            List<VScheduleEventConfiguration> eventConfigs = new List<VScheduleEventConfiguration>();
            eventConfigs.AddRange(_script.EventList.Select((id => VDataManager.Instance.GetDialogueEventConfigurationByID(id))));
            eventConfigs.AddRange(_script.StreamEventList.Select((id => VDataManager.Instance.GetStreamEventConfigurationByID(id))));
            
            scheduleCreator.InitializeCreator(eventConfigs);
            
            _stateMachine.SwitchState(VStateType.PhaseStart, _script.BeginScript());
            
            foreach (var configuration in cooperatorConfigurations)
            {
                _character.CooperatorManager.AddCooperator(configuration);
            }
            
            _mainMenu.gameObject.SetActive(false);
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

        public void ChangeToMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
            _mainMenu.Initialize(scripts, characters, _accounts, InitializeGame);
        }
    }
}