using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Relic;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Reincarnation
{
    public class VAccountCreator
    {
        public static VAccount CreateAccount(VReincarnationConfiguration config, string ratingLevel, VCharacter character)
        {
            VAccount account = new VAccount(GetCards(config, ratingLevel, character),
                GetRelics(config, ratingLevel, character),
                GetEffects(config, ratingLevel, character));
            return account;
        }

        public static List<VCard> GetCards(VReincarnationConfiguration config, string ratingLevel, VCharacter character)
        {
            List<VCard> cardLibrary = character.CardLibrary.GetCards();
            
            List<VCard> cards = new List<VCard>();
            
            int currentCapacity = 0;
            int maxCapacity = config.cardLevels[ratingLevel].cardTotalCapacity;
            int maxCount = config.cardLevels[ratingLevel].cardCount;

            foreach (var requirement in config.cardLevels[ratingLevel].cardRarityRequirements)
            {
                cards.AddRange(cardLibrary.Where(card => (int)card.Rarity >= (int)requirement.rarity)
                    .OrderBy(r => Random.Range(0f, 1f))
                    .Take(requirement.count));
            }

            // calculate current capacity from required cards
            currentCapacity = 0;
            foreach (var card in cards)
            {
                foreach (var cardCapacityInfo in config.cardCapacities)
                {
                    if (cardCapacityInfo.rarity == card.Rarity)
                    {
                        currentCapacity += card.IsUpgraded ? cardCapacityInfo.upgradeCapacity : cardCapacityInfo.capacity;
                        break;
                    }
                }
            }

            // get remaining capacity
            int remainingCapacity = maxCapacity - currentCapacity;

            // try to fill optimally
            List<VCard> shuffledPool = cardLibrary
                .OrderBy(r => Random.Range(0f, 1f)) // shuffle card pool
                .ToList();

            foreach (var card in shuffledPool)
            {
                if (cards.Count >= maxCount) break;

                int capacity = 0;
                foreach (var cardCapacityInfo in config.cardCapacities)
                {
                    if (cardCapacityInfo.rarity == card.Rarity)
                    {
                        capacity = card.IsUpgraded ? cardCapacityInfo.upgradeCapacity : cardCapacityInfo.capacity;
                        break;
                    }
                }

                if (capacity <= remainingCapacity) // only add if it fits
                {
                    cards.Add(card);
                    remainingCapacity -= capacity;

                    if (remainingCapacity == 0) // perfect fit, stop early
                        break;
                }
            }
            return cards;
        }
        
        public static List<VRelic> GetRelics(VReincarnationConfiguration config, string ratingLevel, VCharacter character)
        {
            if (character is null)
                return new List<VRelic>();
            int relicCount = config.relicCount[ratingLevel];
            List<uint> relicIDs = new List<uint>();
            var streamEvents = character.succeededStreams.Select(e => e.EventID).ToList();
            foreach (var relicInfo in config.relicRewards)
            {
                if (streamEvents.Contains(relicInfo.eventID))
                {
                    relicIDs.Add(relicInfo.relicID);
                }
            }
            
            if(relicCount < relicIDs.Count)
            {
                relicIDs = relicIDs.OrderBy(r => Random.Range(0f, 1f)).Take(relicCount).ToList();
            }
            
            List<VRelic> relics = new List<VRelic>();
            foreach (var relicID in relicIDs)
            {
                var relic = VDataManager.Instance.CreateRelicByID(relicID);
                relics.Add(relic);
            }
            
            return relics;
        }

        public static List<VEffectItem> GetEffects(VReincarnationConfiguration config, string ratingLevel,
            VCharacter character)
        {
            int count = config.attributeLevels[ratingLevel].count;
            int capacity = 0;
            int maxCapacity = config.attributeLevels[ratingLevel].capacity;
            var ratingLevelInfo = config.attributeLevels[ratingLevel];

            // StreamAttributes
            List<VAbilityEffectInfo> streamEffectInfos = new List<VAbilityEffectInfo>();
            List<VEffectItem> streamEffects = new List<VEffectItem>();

            foreach (var requirement in ratingLevelInfo.streamEffectsRequirements)
            {
                for (int i = 0; i < requirement.count; i++)
                {
                    var effectInfo = config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];
                    while (streamEffectInfos.Find(info => info.ability == effectInfo.ability) != null)
                    {
                        effectInfo = config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];
                    }

                    var level = Random.Range(requirement.level, config.effectCapacities.Count - 1);
                    var param = effectInfo.levelInfos[level].levelParam;
                
                    streamEffects.Add(new VEffectItem()
                    {
                        id = effectInfo.effect,
                        parameter = param,
                        upgradedParameter = param,
                        level = level
                    });
                
                    streamEffectInfos.Add(effectInfo);
                    capacity += config.effectCapacities[requirement.level];

                    if (streamEffects.Count + 0 >= count) // already at max effects
                        return streamEffects; 
                }
            }

            while (streamEffects.Count < 2 && streamEffects.Count < count)
            {
                var effectInfo = config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];
                if (streamEffectInfos.Find(info => info.ability == effectInfo.ability) != null)
                    continue;

                var level = Random.Range(0, effectInfo.levelInfos.Count);
                var levelInfo = effectInfo.levelInfos[level];

                if (capacity + config.effectCapacities[level] <= maxCapacity)
                {
                    streamEffects.Add(new VEffectItem()
                    {
                        id = effectInfo.effect,
                        parameter = levelInfo.levelParam,
                        upgradedParameter = levelInfo.levelParam,
                        level = level
                    });;
                    streamEffectInfos.Add(effectInfo);
                    capacity += config.effectCapacities[level];

                    if (streamEffects.Count >= count)
                        return streamEffects; 
                }
            }

            // OtherAttributes
            List<VEffectItem> otherEffects = new List<VEffectItem>();
            foreach (var requirement in ratingLevelInfo.attributeEffectsRequirements)
            {
                for (int i = 0; i < requirement.count; i++)
                {
                    var effectInfos = config.attributeEffects.OrderBy(r => Random.Range(0f, 1f))
                        .Take(requirement.count);
                    foreach (var effectInfo in effectInfos)
                    {
                        if (streamEffects.Count + otherEffects.Count >= count)
                            return otherEffects.Union(streamEffects).ToList();
                        var level = Random.Range(0, effectInfo.levelInfos.Count - 1);
                        var param = effectInfo.levelInfos[level].levelParam;

                        otherEffects.Add(new VEffectItem()
                        {
                            id = effectInfo.effect,
                            parameter = param,
                            upgradedParameter = param,
                            level = level
                        });
                        ;
                        capacity += config.effectCapacities[requirement.level];
                    }
                }
            }

            // Remaining pool (allEffects)
            var allEffects = config.attributeEffects.Where(e =>
                !otherEffects.Exists(eft => eft.id == e.effect))
                .OrderBy(r => Random.Range(0f, 1f)).ToList();// shuffle.ToList();

            foreach (var streamAttributeEffect in config.streamAttributeEffects)
            {
                if (!streamEffectInfos.Exists(info => info.ability == streamAttributeEffect.ability) &&
                    !allEffects.Exists(info =>
                    {
                        if (info is VAbilityEffectInfo abilityEffectInfo)
                            return abilityEffectInfo.ability == streamAttributeEffect.ability;
                        return false;
                    }))
                {
                    allEffects.Add(streamAttributeEffect);   
                }
            }

            // Fill with random remaining effects without exceeding capacity and count
            foreach (var effectInfo in allEffects)
            {
                if (streamEffects.Count + otherEffects.Count >= count)
                    break;

                int level = Random.Range(0, effectInfo.levelInfos.Count);
                var levelInfo = effectInfo.levelInfos[level];

                int cost = config.effectCapacities[level];
                if (capacity + cost > maxCapacity)
                    continue;
                
                otherEffects.Add(new VEffectItem()
                {
                    id = effectInfo.effect,
                    parameter = levelInfo.levelParam,
                    upgradedParameter = levelInfo.levelParam,
                    level = level
                });;
                capacity += cost;

                if (capacity == maxCapacity || streamEffects.Count + otherEffects.Count >= count)
                    break;
            }

            return streamEffects.Union(otherEffects).ToList();
        }
    }
}