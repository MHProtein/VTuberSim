using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.Character;

namespace VTuber.BattleSystem.Core
{
    public class VBattleTest : VBattle
    {
        public virtual void InitializeBattle(VCharacterAttributeManager characterAttributeManager,
            VCardLibrary cardLibrary, int initialTurnCount, int targetPopularity, int initialViewers)
        {    
            _characterAttributeManager = characterAttributeManager;
            _battleAttributeManager = new VBattleAttributeManager();
            _cardPilesManager = new VCardPilesManager(configuration.handSize, configuration.maxHandSize, cardLibrary); 
            _buffManager = new VBuffManager(this);
        }

        protected override void Start()
        {
            base.Start();
            _battleAttributeManager.AttributesConversion(_characterAttributeManager);
            _turnAttribute = new VBattleTurnAttribute(10);
            _playLeftAttribute = new VBattlePlayLeftAttribute(configuration.defaultPlayPerTurn);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAPlayLeft", _playLeftAttribute);
            
            _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));
            
            _battleAttributeManager.AddAttribute("BAShield", new VBattleStaminaAttribute(0, VBattleEventKey.OnShieldChange));
            _battleAttributeManager.AddAttribute("BARevenue", new VBattleStaminaAttribute(0, VBattleEventKey.OnRevenueChange));

            _battleAttributeManager.InitializeInternalManagers();
            
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBattleBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
            });
            
            InitializeTurn();
        }

        protected override void OnEnable()
        {
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _battleAttributeManager.OnDisable();
            _cardPilesManager.OnDisable();
            _buffManager.OnDisable();
            base.OnDisable();
        }
    }
}