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
        [SerializeField] private List<VCardConfiguration> _cardConfigurations;
        
        private VCardLibrary _cardLibrary;
        
        protected override void Awake()
        {
            base.Awake();
            _cardLibrary = new VCardLibrary();

            foreach (var config in _cardConfigurations)
            {
                for (int i = 0; i < 5; i++)
                {
                    _cardLibrary.AddCard(config);
                }
            }

            _battle.InitializeBattle(null, _battleConfiguration, _cardLibrary);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnBegin, OnTurnBegin);
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