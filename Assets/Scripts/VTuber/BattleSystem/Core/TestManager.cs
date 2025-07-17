using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class TestManager : VMonoBehaviour
    {
        [SerializeField] private VBattle _battle;
        [SerializeField] private VBattleConfiguration _battleConfiguration;
        
        private VCardLibrary _cardLibrary;
        
        protected override void Awake()
        {
            base.Awake();

            BattleResourcesLoader loader = new BattleResourcesLoader("Assets\\Resources\\Configurations\\NewCards.xlsx");
            
            _cardLibrary = new VCardLibrary();
            var cardConfigs = loader.Load();
            List<VCard> cards = new List<VCard>();

            foreach (var cardConfig in cardConfigs)
            {
                for (int i = 0; i < 2; i++)
                {
                    cards.Add(cardConfig.CreateCard());
                }
            }

            _cardLibrary.AddCards(cards);
            _battle.InitializeBattle(null, _battleConfiguration, _cardLibrary);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
        }

        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            VDebug.Log("Turn Begin: " + messagedict["TurnLeft"]);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}