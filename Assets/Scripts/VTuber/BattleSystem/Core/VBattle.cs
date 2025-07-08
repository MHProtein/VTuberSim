using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattle : VMonoBehaviour
    {
        private VBattleConfiguration _configuration;

        private VBattleAttributeManager _battleAttributeManager;
        private VCardPilesManager _cardPilesManager;

        private int _currentPlayCountLeft = 0;
        
        //Attributes
        private VBattleTurnAttribute _turnAttribute;
        private VBattleScoreAttribute _scoreAttribute;
        
        
        public int TurnLeft => _turnAttribute.Value;
        
        private int MaxTurnCount => _configuration.maxTurnCount;

        public void InitializeBattle(VCharacterAttributeManager characterAttributeManager, VBattleConfiguration configuration, VCardLibrary cardLibrary)
        {
            _configuration = configuration;

            _battleAttributeManager = new VBattleAttributeManager(characterAttributeManager);
            _cardPilesManager = new VCardPilesManager(_configuration.handSize, _configuration.maxHandSize, cardLibrary);

            _turnAttribute = new VBattleTurnAttribute(_configuration.maxTurnCount, false);
            _scoreAttribute = new VBattleScoreAttribute(0, false);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAScore", _scoreAttribute);
            
            InitializeTurn();
        }

        protected override void Awake()
        {
            base.Awake();
            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //_cardPilesManager.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //_cardPilesManager.OnDisable();
        }

        public void InitializeTurn()
        {
            VRootEventCenter.Instance.Raise(VRootEventKeys.OnTurnBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
                {"HandSize", _configuration.maxHandSize}
            });
        }

        public void EndTurn()
        {
            _turnAttribute.DecreaseTurn();
            if (TurnLeft <= 0)
            {
                // End battle
            }
            else
            {
                VRootEventCenter.Instance.Raise(VRootEventKeys.OnTurnEnd, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
            }
        }
    }
}