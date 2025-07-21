using System;
using Spire.Xls;

namespace VTuber.BattleSystem.Effect
{      
    public enum VCardPileType
    {
        DrawPile,
        Discard,
        Exhaust,
        Deck
    }
    
    public class VPickCardFromPileEffectConfiguration : VEffectConfiguration
    {
        public VCardPileType cardPileType;
        public int cardCount;
        public VPickCardFromPileEffectConfiguration(CellRange row) : base(row)
        {
            cardPileType = Enum.Parse<VCardPileType>(row.Columns[VEffectHeaderIndex.Parameter].Value);
        }

        public override VEffect CreateEffect(string parameter, string upgradedParameter)
        {
            upgradable = parameter != upgradedParameter;
            return new VPickCardFromPileEffect(this, parameter, upgradedParameter);
        }
    }
}