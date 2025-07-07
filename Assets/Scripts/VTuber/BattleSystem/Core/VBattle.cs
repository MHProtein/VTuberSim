using VTuber.Character;

namespace VTuber.BattleSystem.Core
{
    public class VBattle
    {
        private VBattleConfiguration _configuration;

        VCharacterBattleAttributes characterBattleAttributes;

        int currentTurn = 1;
        int maxTurnCount => _configuration.maxTurnCount;
        int currentPlayCountLeft = 0;
        
        public VBattle(VCharacterAttributeManager characterAttributeManager, VBattleConfiguration configuration)
        {
            this._configuration = configuration;
            this.characterBattleAttributes = new VCharacterBattleAttributes(characterAttributeManager);
        }

        public void InitializeTurn()
        {
            
        }
        
        public void EndTurn()
        {
            currentTurn++;
            if (currentTurn > maxTurnCount)
            {
                // End battle
            }
            else
            {
                InitializeTurn();
            }
        }
    }
}