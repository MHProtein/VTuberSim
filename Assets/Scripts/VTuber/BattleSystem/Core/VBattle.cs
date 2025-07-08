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

        VBattleAttributeManager _battleAttributeManager;

        private int _currentPlayCountLeft = 0;
        
        //Attributes
        private VBattleTurnAttribute _turnAttribute;
        private VBattleScoreAttribute _scoreAttribute;
        
        
        public int TurnLeft => _turnAttribute.Value;
        
        private int MaxTurnCount => _configuration.maxTurnCount;

        public void InitializeBattle(VCharacterAttributeManager characterAttributeManager, VBattleConfiguration configuration)
        {
            this._configuration = configuration;

            _battleAttributeManager = new VBattleAttributeManager(characterAttributeManager);

            _turnAttribute = new VBattleTurnAttribute(_configuration.maxTurnCount, false);
            _scoreAttribute = new VBattleScoreAttribute(0, false);
            
            _battleAttributeManager.AddAttribute("BATurn", _turnAttribute);
            _battleAttributeManager.AddAttribute("BAScore", _scoreAttribute);
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