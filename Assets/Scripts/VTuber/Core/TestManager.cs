using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.StateMachine;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;

namespace VTuber.BattleSystem.Core
{
    public class TestManager : VMonoBehaviour
    {
        [FormerlySerializedAs("schedule")]
        [Header("Schedule")] 
        [SerializeField] private VScheduleUI scheduleUI;

        private VWeeklySchedule _weeklySchedule; 
        
        [Space(5)]
        [Header("Battle")]
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private VBattle battle;
        [SerializeField] private VBattleConfiguration _battleConfiguration;
        [SerializeField] private VCharacterConfiguration _characterConfiguration;
        private VCharacter _character;
        private VStateMachine _stateMachine;
        
        protected override void Awake()
        {
            base.Awake();
            VBattleResourcesLoader loader = new VBattleResourcesLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
            _character = new VCharacter(_characterConfiguration);
            _weeklySchedule = new VWeeklySchedule(_character);
            var cardConfigs = loader.Load();
            List<VCard> cards = new List<VCard>();

            foreach (var cardConfig in cardConfigs)
            {
                for (int i = 0; i < 2; i++)
                {
                    var card = cardConfig.CreateCard();
                    if(card is not null)
                        cards.Add(card);
                }
            }
            _character.CardLibrary.AddCards(cards);
            _stateMachine = new VStateMachine(scheduleUI, _weeklySchedule, battleRoot, battle, _character);
            _stateMachine.RegisterState(new VScheduleCreationState());
            _stateMachine.RegisterState(new VExecutionState());
            _stateMachine.RegisterState(new VPauseState());
            _stateMachine.RegisterState(new VScheduleModifyState());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _stateMachine.OnEnable();
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            _stateMachine.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
            _stateMachine.SwitchState(VStateType.ScheduleCreation);
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

        public void ConvertToSchedule()
        {
            var slots = scheduleUI.Slots;
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

            if (_stateMachine.CurrentState.StateType == VStateType.ScheduleCreation)
            {
                _stateMachine.SwitchState(VStateType.Execution);
                
            }
            else if (_stateMachine.CurrentState.StateType == VStateType.ScheduleModify)
            {
                _stateMachine.SwitchState(VStateType.Pause);
            }
        }
    }
}