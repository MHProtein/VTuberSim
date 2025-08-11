using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.Core;

namespace VTuber.CoopSystem
{
    public class VCoopEvent
    {
        public int unlockLevel;
        public float probability;
        public List<VRaisingEffect> effects;
        public List<VEventType> eventTypes;
    }

    public struct VCoopEventItem
    {
        public VCoopEvent e;
        public Sprite pfp;
        public Vector2Int position;
    }
    
    public class VCooperator
    {
        public uint Id => configuration.Id;
        
        public VCoopLevel CurrentCoopLevel => configuration.CoopLevels[_currentLevelIndex];
        
        public readonly VCooperatorConfiguration configuration;
        
        private int _currentLevelIndex;
        private int _coopValue;
        private List<VCoopEvent> _coopEvents;
        
        public VCooperator(VCooperatorConfiguration configuration)
        {
            _currentLevelIndex = 0;
            this.configuration = configuration;
        }
        
        public List<VCoopEventItem> GenerateCoopEventPositions(List<Vector2Int> occupiedPositions)
        {
            _coopEvents.Sort((x, y) => x.probability.CompareTo(y.probability));
            List<VCoopEventItem> events = new List<VCoopEventItem>();
            int positionCount = Random.Range(configuration.MinEvents, configuration.MaxEvents + 1);
            for (int i = 0; i < positionCount; i++)
            {
                Vector2Int position = new Vector2Int(GetDay(), GetTime());
                while (occupiedPositions.Contains(position) || events.Exists(x => x.position == position))
                {
                    position = new Vector2Int(GetDay(), GetTime());
                }

                var e = GetEvent();
                events.Add(new VCoopEventItem()
                {
                    e = e,
                    position = position
                });
            }

            return events;
        }

        public VCoopEvent GetEvent()
        {
            List<VCoopEvent> events = _coopEvents.Where(x => x.unlockLevel <= _currentLevelIndex).ToList();
            float probabilitySum = events.Sum(x => x.probability);
            if (probabilitySum <= 0)
            {
                return null;
            }

            float probability = Random.Range(0, 1f);
            float totalProbability = 0;
            foreach (var e in events)
            {
                totalProbability += e.probability / probabilitySum;
                if (probability <= totalProbability)
                {
                    return e;
                }
            }
            return null;
        }

        public int GetDay()
        {
            var dayProbabilities = configuration.DayProbabilities;
            float probabilitySum = dayProbabilities.Sum();
            float probability = Random.Range(0, 1f);
            float totalProbability = 0;
            for (int i = 0; i < dayProbabilities.Count; i++)
            {
                totalProbability += dayProbabilities[i] / probabilitySum;
                if (probability <= totalProbability)
                {
                    return i;
                }
            }
            return dayProbabilities.Count - 1;
        }
        
        public int GetTime()
        {
            var timeProbabilities = configuration.DayTimeProbabilities;
            float probabilitySum = timeProbabilities.Sum();
            float probability = Random.Range(0, 1f);
            float totalProbability = 0;
            for (int i = 0; i < timeProbabilities.Count; i++)
            {
                totalProbability += timeProbabilities[i] / probabilitySum;
                if (probability <= totalProbability)
                {
                    return i;
                }
            }
            return timeProbabilities.Count - 1;
        }
    }
}