using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.StateMachine;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
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
        private VCharacter character;
        
        private VStreamEvent _currentEvent;
        protected override void Awake()
        {
            base.Awake();
            VBattleResourcesLoader loader = new VBattleResourcesLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
            character = new VCharacter(_characterConfiguration);
            _weeklySchedule = new VWeeklySchedule(character);
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
            character.CardLibrary.AddCards(cards);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEndNotify, OnBattleEnd);
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(0.75f).OnComplete(() => battleRoot.SetActive(false));
            Tween.Delay(2.0f, () =>
            {
                _currentEvent.NextEvent();
            });
        }
        
        public void InitializeBattle()
        {
            battleRoot.SetActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(1.0f);
            battle.InitializeBattle(character.AttributeManager,
                character.CardLibrary,
                _currentEvent.InitialTurnCount);
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VStreamEvent;
            InitializeBattle();
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetBattleUIScale(1.0f);
        }      
        
        protected override void Start()
        {
            base.Start();
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToCreation();
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
                        var eventData = slot.Item.EventData;
                        slot.Item.SetInteractive(false);
                        _weeklySchedule.SetEvent(x, (TimeOfDay)y, eventData.CreateEvent());
                        y += eventData.Duration;
                    }
                    else
                    {
                        ++y;
                    }
                }
            }

            VSingletonMonobehaviour<VRaisingUI>.Instance.SetCreationUIActive(false);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetExecutionUIActive(true);
            VSingletonMonobehaviour<VRaisingUI>.Instance.SetScheduleUIPositionToExecution().OnComplete(() =>
            {
                scheduleUI.ResetIndicatorPosition().OnComplete(() => _weeklySchedule.BeginExecution());
            });
        }
    }
}