using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattle : VSingleton<VBattle>
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

        private bool shouldNextCardPlayTwice = false;
        private bool shouldRedraw = false;
        
        private List<VBuffConfiguration> _playTwiceBuffConfigurations;
        private List<VEffectConfiguration> _playTwiceEffectConfigurations;
        private Dictionary<string, object> _playTwiceMessageDict;

        public void NextCardPlayTwice()
        {
            shouldNextCardPlayTwice = true;
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnEffectAnimationFinished, new Dictionary<string ,object>() { });
        }
        
        public void RedrawRest()
        {
            shouldRedraw = true;
        }
        
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
            
            _battleAttributeManager.AddAttribute("BAStamina", new VBattleStaminaAttribute(100, 100));
            _battleAttributeManager.AddAttribute("BAShield", new VBattleAttribute(0, false, VRootEventKey.OnShieldChange));
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBattleBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
            });
            
            InitializeTurn();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _battleAttributeManager.OnEnable();
            _cardPilesManager.OnEnable();
            _buffManager.OnEnable();
            
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnNotifyTurnBeginDelay, OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnCardMovedToPlayPosition, OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _battleAttributeManager.OnDisable();
            _cardPilesManager.OnDisable();
            _buffManager.OnDisable();
            
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardPlayed, OnCardPlayed);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnNotifyTurnBeginDelay, OnNotifyTurnBeginDelay);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardUsed, OnCardUsed);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnCardMovedToPlayPosition, OnCardMovedToPlayPosition);
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnPlayTheSecondTime, OnPlayTheSecondTime);
        }
        
        private void OnPlayTheSecondTime(Dictionary<string, object> messagedict)
        {
            shouldNextCardPlayTwice = false;
            
            if(_playTwiceBuffConfigurations is not null &&
               _playTwiceEffectConfigurations is not null &&
               _playTwiceMessageDict is not null)
                ApplyCardEffectsAndBuffs(_playTwiceBuffConfigurations, _playTwiceEffectConfigurations, _playTwiceMessageDict);
            
            _playTwiceBuffConfigurations = null;
            _playTwiceEffectConfigurations = null;
            _playTwiceMessageDict = null;
        }
        
        private void OnCardMovedToPlayPosition(Dictionary<string, object> messagedict)
        {
            var card = messagedict["Card"] as VCard;
            if (card is null)
                return;
            var buffs = card.Buffs;
            var effects = card.Effects;
            
            ApplyCardEffectsAndBuffs(buffs, effects, messagedict);
        }
        
        private void OnCardUsed(Dictionary<string, object> messagedict)
        {
            _playLeftAttribute.AddTo(-1, false);
            VDebug.Log("Play Left: " + PlayLeft);
            if (PlayLeft <= 0)
            {
                EndTurn();
                if (shouldRedraw) shouldRedraw = false;
                return;
            }
        }
        
        private void OnNotifyTurnBeginDelay(Dictionary<string, object> messagedict)
        {
            StartCoroutine(DelayInitializeTurn((float)messagedict["DelaySeconds"]));
        }
        
        IEnumerator DelayInitializeTurn(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);

            InitializeTurn();
        }

        public void InitializeTurn()
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnTurnBegin, new Dictionary<string, object>
            {
                {"TurnLeft", TurnLeft},
                {"HandSize", _configuration.maxHandSize}
            });
        }

        public void EndTurn()
        {
            Debug.Log("End Turn: " + TurnLeft);
            _turnAttribute.AddTo(-1, false);
            if (TurnLeft <= 0)
            {
                // End battle
            }
            else
            {
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnTurnEndBuffApply, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnTurnResolution, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
                
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnTurnEnd, new Dictionary<string, object>
                {
                    {"TurnLeft", TurnLeft}
                });
            }
        }
        
        private void OnCardPlayed(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnPreCardApply, messagedict);
            
            _battleAttributeManager.ApplyCost((int)messagedict["Cost"]);
        }

        private void Redraw()
        {
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnRedrawCards, new Dictionary<string, object>
            {
                {"ShouldPlayTwice", shouldNextCardPlayTwice},
            });
            if (shouldNextCardPlayTwice)
                shouldNextCardPlayTwice = false;
        }

        private void ApplyCardEffectsAndBuffs(List<VBuffConfiguration> buffs, List<VEffectConfiguration> effects,
            Dictionary<string, object> messagedict)
        {
            if(effects is null)
                return;
            
            if (shouldNextCardPlayTwice)
            {
                _playTwiceBuffConfigurations = buffs;
                _playTwiceEffectConfigurations = effects;
                _playTwiceMessageDict = messagedict;
            }
            
            _buffManager.AddBuffs(buffs, true, shouldNextCardPlayTwice);
            
            foreach (var effectConfig in effects)
            {
                var effect = effectConfig.CreateEffect();
                
                if (!effect.AreConditionsMet(this, messagedict))
                    continue;
                effect.ApplyEffect(this, 1, true, shouldNextCardPlayTwice);
            }
            
            if (shouldRedraw)
            {
                shouldRedraw = false;
                if (PlayLeft == 0)
                    return;
                Redraw();
            }
        }
    }
}