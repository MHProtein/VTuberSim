using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Core.ScriptSystem
{
    public struct VScoreResult
    {
        public int score;
        public string scoreLevelName;
    }

    public struct VScriptSaveData
    {
        public string scriptConfigurationName;
        public int currentPhaseIndex;
    }
    
    public class VScript
    {
        private List<VPhase> Phases => _configuration.phases;
        private VScriptConfiguration _configuration;
        
        public VPhase CurrentPhase => _currentPhase;
        private VPhase _currentPhase;

        public List<uint> EventList => _configuration.eventIDs;
        public List<uint> StreamEventList => _configuration.streamEventIDs;
        public List<VKPI> kpis;

        public VScript(VScriptConfiguration configuration)
        {
            _configuration = configuration;
            kpis = new List<VKPI>();
            foreach (var kpi in configuration.kpis)
            {
                kpis.Add(new VKPI(kpi.EventType, kpi.RequiredAmount, kpi.AbilityIndex, true));
            }
        }

        public VScriptSaveData Save()
        {
            return new VScriptSaveData
            {
                scriptConfigurationName = _configuration.name,
                currentPhaseIndex = Phases.IndexOf(_currentPhase)
            };
        }

        public static VScript Load(VScriptSaveData data, VScriptConfiguration scriptConfig)
        {
            VScript script = new VScript(scriptConfig);
            script._currentPhase = script.Phases[data.currentPhaseIndex];
            return script;
        }

        public VScheduleEvent BeginScript()
        {
            for (int i = 0; i < Phases.Count - 1; i++)
            {
                Phases[i].nextPhase = Phases[i + 1];
            }
            _currentPhase = Phases[0];
            return _currentPhase.GetStartEvent();
        }
        
        public List<VSpecialEventData> GetSpecialEvents(int weekIndex)
        {
            List<VSpecialEventData> events = new List<VSpecialEventData>();
            foreach (var phase in Phases)
            {
                if(phase.IsInPhase(weekIndex))
                    events.AddRange(phase.GetSpecialEventData(weekIndex));
            }
            return events;
        }

        public VScheduleEvent NextWeek(int weekIndex)
        {
            if (_currentPhase.nextPhase is null)
                return null;
            if (_currentPhase.nextPhase.IsInPhase(weekIndex))
            {
                _currentPhase = _currentPhase.nextPhase;
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseBegin, new Dictionary<string, object>());
                return _currentPhase.GetStartEvent();
            }

            return null;
        }

        public VScoreResult CalculateScore(VCharacter character, int popularity, bool success)
        {
            int singingAbility = character.AttributeManager.Attributes["CASingingAbility"].Value;
            int gamingAbility = character.AttributeManager.Attributes["CAGamingAbility"].Value;
            int chattingAbility = character.AttributeManager.Attributes["CAChattingAbility"].Value;
            int follower = character.AttributeManager.Attributes["CAFollowerCount"].Value;
            int highestMembershipCount = (character.AttributeManager.Attributes["CAMembershipCount"] as VMembershipCountAttribute).highestValue;

            float popularityCoefficient = 0;
            foreach (var range in _configuration.popularityCoefficient)
            {
                if (range.IsInRange(popularity))
                {
                    popularityCoefficient = range.value;
                }
            }
            
            int score = Mathf.CeilToInt((singingAbility + gamingAbility + chattingAbility) * _configuration.abilityCoefficient +
                        follower * _configuration.followerCoefficient
                        + highestMembershipCount * _configuration.membershipCoefficient
                        + popularityCoefficient * popularity);
            if(success)
                score += _configuration.successBonus;
            
            var scoreLevel = _configuration.scoreLevels.Find(level=> level.InLevel(score));
            VDebug.Log("歌力： " + singingAbility);
            VDebug.Log("游戏力： " + gamingAbility);
            VDebug.Log("杂谈力： " + chattingAbility);
            VDebug.Log("关注人数： " + follower);
            VDebug.Log("最高舰长数： " + highestMembershipCount);
            VDebug.Log("直播热度： " + popularity);
            VDebug.Log("分数： " + score);
            VDebug.Log("等级： " + scoreLevel);
            return new VScoreResult
            {
                score = score,
                scoreLevelName = scoreLevel.name
            };
        }

        public int GetPhaseIndex(VPhase phase)
        {
            return Phases.IndexOf(phase);
        }

        public VPhase GetPhase(int index)
        {
            return Phases[index];
        }
    }
}