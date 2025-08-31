using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Character;
using VTuber.Consumable;
using VTuber.Core.Managers;

namespace VTuber.Core.RaisingEffect
{
    public abstract class VRaisingConsumableEffect : VRaisingEffect
    {
        public readonly List<float> rarityProbabilities;
        public VRaisingConsumableEffect(VRaisingConsumableEffectConfiguration configuration) : base(configuration)
        {
            rarityProbabilities = configuration.rarityProbabilities;
        }
        
        public List<VConsumable> GetRandomConsumables(int count, string liveType)
        {
            List<int> rarityCounts = new List<int>()
            {
                0, 0, 0
            };
            List<VConsumableConfiguration> consumables = VDataManager.Instance.
                GetAllConsumableConfigurations().Where(configuration => (configuration.liveType == liveType || configuration.liveType == "F")).ToList();
            
            if (consumables.Count == 0)
                return null;
            
            foreach (var consumable in consumables)
            {
                rarityCounts[(int) consumable.rarity]++;
            }
            
            float totalRarityProb = 0f;
            for (int r = 0; r < 3; r++)
                if (rarityCounts[r] > 0)
                    totalRarityProb += rarityProbabilities[r];

            float[] perConsumableProbabilityByRarity = new float[3];
            for (int r = 0; r < 3; r++)
            {
                if (rarityCounts[r] > 0)
                {
                    float adjustedRarityProb = rarityProbabilities[r] / totalRarityProb; // normalize
                    perConsumableProbabilityByRarity[r] = adjustedRarityProb / rarityCounts[r];
                }
                else
                {
                    perConsumableProbabilityByRarity[r] = 0f;
                }
            }
            
            List<VConsumable> selectedConsumables = new List<VConsumable>();

            int i = 0;
            while (i < count)
            {
                float probability = Random.Range(0, 1.0f);
                float totalProbability = 0;
                for (int j = 0; j < consumables.Count; j++)
                {
                    totalProbability += perConsumableProbabilityByRarity[(int)consumables[j].rarity];
                    if (probability <= totalProbability)
                    {
                        var consumable = consumables[j].CreateConsumable();
                        if (selectedConsumables.Find(vConsumable => vConsumable._configuration.id == consumable._configuration.id) != null)
                            break;
                        selectedConsumables.Add(consumable);
                        ++i;
                        break;
                    }
                }
            }
            return selectedConsumables;
        }
    }
}