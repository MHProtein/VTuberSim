using System;
using CsvHelper;

namespace VTuber.BattleSystem.Effect
{      
    public enum VCardPileType
    {
        DrawPile,
        Discard,
        Exhaust
    }
    
    public class VPickCardFromPileEffectConfiguration : VEffectConfiguration
    {
        public VCardPileType cardPileType;
        public int cardCount;
        public VPickCardFromPileEffectConfiguration(CsvReader csv) : base(csv)
        {
            cardPileType = Enum.Parse<VCardPileType>(csv.GetField<string>("CardPileType"));
            cardCount = csv.GetField<int>("CardCount");
        }

        public override VEffect CreateEffect()
        {
            return new VPickCardFromPileEffect(this);
        }
    }
}