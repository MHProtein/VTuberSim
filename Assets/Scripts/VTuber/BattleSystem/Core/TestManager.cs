using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.BattleSystem.Core
{
    public class TestManager : VMonoBehaviour
    {
        [Header("Schedule")]
        
        [Space(5)]
        [Header("Battle")]
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private VBattle _battle;
        [SerializeField] private VBattleConfiguration _battleConfiguration;
        [SerializeField] private VCharacterConfiguration _characterConfiguration;
        private VCharacter character;
        private VStreamEvent _currentEvent;
        
        protected override void Awake()
        {
            base.Awake();
            
            VBattleResourcesLoader loader = new VBattleResourcesLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
            character = new VCharacter(_characterConfiguration);
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


            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnStreamEventStart, OnStreamEventStart);
            
            
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }
        
        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            battleRoot.SetActive(false);
            Tween.Delay(2.0f, () =>
            {
                _currentEvent.NextEvent();
                _currentEvent = null;
            });
        }
        
        private void OnStreamEventStart(Dictionary<string, object> messagedict)
        {
            _currentEvent = messagedict["Event"] as VStreamEvent;
            InitializeBattle();
        }

        public void InitializeBattle()
        {
            battleRoot.SetActive(true);
            _battle.InitializeBattle(character.AttributeManager, _battleConfiguration, character.CardLibrary, _currentEvent.InitialTurnCount);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}