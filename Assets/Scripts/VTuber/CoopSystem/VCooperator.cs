using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using Random = UnityEngine.Random;

namespace VTuber.CoopSystem
{
    public class VCoopEventHeaderIndex
    {  
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Probability = 3;
        public const int UnlockLevel = 4;
        public const int EventTypes = 5;
        public const int Effect1 = 6;
        public const int E1Param = 7;
        public const int Effect2 = 8;
        public const int E2Param = 9;
        public const int Effect3 = 10;
        public const int E3Param = 11;
    }
    
    public class VCoopEvent
    {
        public struct VCoopEventType
        {
            public VEventType eventType;
            public int abilityIndex;
        }
        public uint id;
        public string eventName;
        public int unlockLevel;
        public float probability;
        public List<VRaisingEffect>  effects;
        public List<VCoopEventType> eventTypes;

        public VCoopEvent(CellRange row)
        {
            id = uint.Parse(row.Columns[VCoopEventHeaderIndex.Id].Value);
            eventName = row.Columns[VCoopEventHeaderIndex.Name].Value;
            unlockLevel = int.Parse(row.Columns[VCoopEventHeaderIndex.UnlockLevel].Value);
            probability = float.Parse(row.Columns[VCoopEventHeaderIndex.Probability].Value);
            
            effects = new List<VRaisingEffect>();
            for (int i = VCoopEventHeaderIndex.Effect1; i <= VCoopEventHeaderIndex.E3Param; i += 2)
            {               
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                effects.Add(VDataManager.Instance.CreateRaisingEffectByID(Convert.ToUInt32(effectIDStr),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }
            
            eventTypes = new List<VCoopEventType>();
            foreach (var type in row.Columns[VCoopEventHeaderIndex.EventTypes].Value.Split(','))
            {
                var typeStr = type.Trim();
                var t = new VCoopEventType();
                if (type.Contains("Stream") && type.Length != 6)
                {
                    t.abilityIndex = int.Parse(type.Substring(6));
                    typeStr = "Stream";
                }
                else
                    t.abilityIndex = -1;
                
                t.eventType = Enum.Parse<VEventType>(typeStr);
                eventTypes.Add(t);
            }
        }
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
        
        public int CoopValue => _coopValue;
        
        public int CurrentLevel => _currentLevelIndex;
        private int _currentLevelIndex;
        private int _coopValue;
        
        public List<VCoopEvent> CoopEvents => _coopEvents;
        private List<VCoopEvent> _coopEvents;

        private bool _hasExecutedUpgradeEventThisWeek;
        
        public VScheduleEvent UpgradeEvent => _upgradeEvent;
        private VScheduleEvent _upgradeEvent;
        
        public VCooperator(VCooperatorConfiguration configuration)
        {
            _currentLevelIndex = 0;
            this.configuration = configuration;
            _coopEvents = this.configuration.CoopEvents.Select(@event => VDataManager.Instance.GetCoopEventByID(@event)).ToList();
        }

        public void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnWeekEnd, OnWeekEnd);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToScheduleCreation, OnSwitchToScheduleCreation);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }

        public void OnDisable()
        {
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnWeekEnd, OnWeekEnd);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule, OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToScheduleCreation, OnSwitchToScheduleCreation);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }
        
        private void OnSwitchToScheduleCreation(Dictionary<string, object> messagedict)
        { 
            if (!_hasExecutedUpgradeEventThisWeek && _upgradeEvent != null)
            {
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSetCoopUpgradeEvent, new Dictionary<string, object>()
                {
                    {"Cooperator", this},
                });
            }
        }

        private void OnSwitchToModifySchedule(Dictionary<string, object> messagedict)
        {
            if (!_hasExecutedUpgradeEventThisWeek && _upgradeEvent != null)
            {      
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSetCoopUpgradeEvent, new Dictionary<string, object>()
                {
                    {"Cooperator", this},
                });
            }
        }
        
        private void OnWeekEnd(Dictionary<string, object> messagedict)
        {
            _hasExecutedUpgradeEventThisWeek = false;
        }
        
        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            if (messagedict["Event"] == _upgradeEvent)
            {
                _hasExecutedUpgradeEventThisWeek = true;
                _upgradeEvent = null;
            }
        }
        
        public void AddCoopValue(int value)
        {
            _coopValue += value;
            VDebug.Log("CoopValue: " + _coopValue);
            if (_coopValue - CurrentCoopLevel.to >= 0)
            {
                if (CurrentCoopLevel.eventType == VEventType.Stream)
                    _upgradeEvent = VDataManager.Instance.CreateStreamEventByID(CurrentCoopLevel.upgradeEventID);
                else
                    _upgradeEvent = VDataManager.Instance.CreateDialogueEventByID(CurrentCoopLevel.upgradeEventID);
            }
        }
        
        public void UpgradeLevel()
        {
            _currentLevelIndex = Mathf.Clamp(_currentLevelIndex + 1, 0, configuration.CoopLevels.Count - 1);
            
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorValueUpdated, new Dictionary<string, object>()
            {
                {"Cooperator", this},
                {"Level", _currentLevelIndex}
            });
            if (_coopValue - CurrentCoopLevel.to >= 0)
            {
                if (CurrentCoopLevel.eventType == VEventType.Stream)
                    _upgradeEvent = VDataManager.Instance.CreateStreamEventByID(CurrentCoopLevel.upgradeEventID);
                else
                    _upgradeEvent = VDataManager.Instance.CreateDialogueEventByID(CurrentCoopLevel.upgradeEventID);
            }
            _hasExecutedUpgradeEventThisWeek = true;
        }

        #region CoopEventGeneration
        
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

                VDebug.Log(position);

                VCoopEvent e = null;
                while (e is null)
                {
                    e = GetEvent();
                }
                events.Add(new VCoopEventItem()
                {
                    e = e,
                    position = position,
                    pfp = configuration.Icon
                });
            }

            return events;
        }

        public VCoopEvent GetEvent()
        {
            List<VCoopEvent> events = _coopEvents.Where(x => x.unlockLevel <= _currentLevelIndex).ToList();
            if (events.Count == 1)
                return events.FirstOrDefault();
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
        
        #endregion

    }
}