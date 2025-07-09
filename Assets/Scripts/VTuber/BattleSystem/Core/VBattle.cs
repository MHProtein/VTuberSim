using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattle : VMonoBehaviour
    {
        private VBattleConfiguration _configuration;

        #region Managers

        public VBattleAttributeManager BattleAttributeManager => _battleAttributeManager;
        private VBattleAttributeManager _battleAttributeManager;
        
        public VCardPilesManager CardPilesManager => _cardPilesManager;
        private VCardPilesManager _cardPilesManager;
        
        public VBuffManager BuffManager => _buffManager;
        private VBuffManager _buffManager;

        #endregion
        
        #region Attributes

        private VBattleTurnAttribute _turnAttribute;

        private VBattlePlayLeftAttribute _playLeftAttribute;
        // private VBattlePopularityAttribute _popularityAttribute;
        // private VBattleParameterAttribute _parameterAttribute;
        // private VBattleAttribute _shieldAttribute;

        #endregion

        private int _currentPlayCountLeft = 0;

        
        public int TurnLeft => _turnAttribute.Value;
        public int PlayLeft => _playLeftAttribute.Value;
        
        private int MaxTurnCount => _configuration.maxTurnCount;

        public void InitializeBattle(VCharacterAttributeManager characterAttributeManager, VBattleConfiguration configuration, VCardLibrary cardLibrary)
        {
            _configuration = configuration;

            _battleAttributeManager = new VBattleAttributeManager(characterAttributeManager);
            _cardPilesManager = new VCardPilesManager(_configuration.handSize, _configuration.maxHandSize, cardLibrary); 
            _buffManager = new VBuffManager(this);
            //_battleAttributeManager.AddAttribute("BAShield", new VBattleAttribute(0, false));
        }

        protected override void Awake()
        {
            base.Awake();
            
        }

 
        
        protected override void Start()
        {
            base.Start();
            
            _turnAttribute = new VBattleTurnAttribute(_configuration.maxTurnCount);
            _playLeftAttribute = new VBattlePlayLeftAttribute(_configuration.defaultPlayPerTurn);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAPlayLeft", _playLeftAttribute);
            _battleAttributeManager.AddAttribute("BAPopularity", new VBattlePopularityAttribute(0));
            _battleAttributeManager.AddAttribute("BAParameter", new VBattleParameterAttribute(0));
            _battleAttributeManager.AddAttribute("BASingingMultiplier", new VBattleMultiplierAttribute(500));
            
            InitializeTurn();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _battleAttributeManager.OnDisable();
            _cardPilesManager.OnDisable();
            _buffManager.OnDisable();
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
        }

        public void InitializeTurn()
        {
            VRootEventCenter.Instance.Raise(VRootEventKey.OnTurnBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
                {"HandSize", _configuration.maxHandSize}
            });
        }

        public void EndTurn()
        {
            _turnAttribute.AddTo(-1);
            if (TurnLeft <= 0)
            {
                // End battle
            }
            else
            {
                VRootEventCenter.Instance.Raise(VRootEventKey.OnTurnEndBuffApply, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VRootEventCenter.Instance.Raise(VRootEventKey.OnTurnResolution, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VRootEventCenter.Instance.Raise(VRootEventKey.OnTurnEnd, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                InitializeTurn();
            }
            
        }
        
        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            var buffs = messagedict["Buffs"] as List<VBuffConfiguration>;
            var effects = messagedict["Effects"] as List<VEffectConfiguration>;
            
            _buffManager.AddBuffs(buffs);
            if(effects is not null)
            {
                foreach (var effectConfig in effects)
                {
                    var effect = effectConfig.CreateEffect();
                    if (effect.AreConditionsMet(this, messagedict))
                    {
                        effect.ApplyEffect(this);
                    }
                }
            }
            
            _playLeftAttribute.AddTo(-1);
            VDebug.Log("Play Left: " + PlayLeft);
            
            if (PlayLeft <= 0)
            {
                EndTurn();
            }
        }
    }
}