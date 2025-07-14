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

            CardDataLoader loader = new CardDataLoader("Assets\\Resources\\Configurations");
            
            _cardLibrary = new VCardLibrary();
            _cardLibrary.AddCards(loader.Load());
            _battle.InitializeBattle(null, _battleConfiguration, _cardLibrary);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
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