using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.Core.Foundation;

namespace VTuber.Store
{
    public class VStoreConfiguration : VScriptableObject
    {
        [Header("升级/删除卡牌")] [LabelText("升级价格")]
        public int upgradePrice;

        [FormerlySerializedAs("deleteCardPrice")] [LabelText("删卡价格")]
        public int discardCardPrice;

        [LabelText("升级价格涨幅")] public int upgradePriceIncrease;

        [FormerlySerializedAs("deletePriceIncrease")] [LabelText("删卡价格涨幅")]
        public int discardCardPriceIncrease;

        [LabelText("刷新次数")] public int defaultRefreshCount;

        [LabelText("最小折扣")] public float minDiscount;
        [LabelText("最大折扣")] public float maxDiscount;

        [LabelText("稀有度出现概率")] public List<float> cardRarityProbabilities;
        [LabelText("稀有度升级概率")] public List<float> cardRarityUpgradeProbabilities;

        [LabelText("稀有度出现概率")] public List<float> consumableRarityProbabilities;

        [LabelText("卡牌价格")] public List<int> cardPrices;
        [LabelText("消耗品价格")] public List<int> consumablePrices;

        public int cardCount;
        public int consumableCount;
    }
}