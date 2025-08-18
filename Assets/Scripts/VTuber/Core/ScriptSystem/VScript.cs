using System.Collections.Generic;
using UnityEngine;
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
    
    public class VScript
    {
        private List<VPhase> Phases => _configuration.phases;
        private VScriptConfiguration _configuration;
        
        public VPhase CurrentPhase => _currentPhase;
        private VPhase _currentPhase;

        public List<uint> EventList => _configuration.eventIDs;
        public List<uint> StreamEventList => _configuration.streamEventIDs;

        public VScript(VScriptConfiguration configuration)
        {
            _configuration = configuration;
        }

        public VScheduleEvent BeginScript()
        {
            _currentPhase = Phases[0];
            _currentPhase.nextPhase = Phases.Count > 1 ? Phases[1] : null;
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
            if (_currentPhase.nextPhase.IsInPhase(weekIndex))
            {
                _currentPhase = _currentPhase.nextPhase;
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseBegin, new Dictionary<string, object>());
                return _currentPhase.GetStartEvent();
            }

            return null;
        }

        public VScoreResult CalculateScore(VCharacter character)
        {
            int singingAbility = character.AttributeManager.Attributes["CASingingAbility"].Value;
            int gamingAbility = character.AttributeManager.Attributes["CAGamingAbility"].Value;
            int chattingAbility = character.AttributeManager.Attributes["CAChattingAbility"].Value;
            int follower = character.AttributeManager.Attributes["CAFollowerCount"].Value;
            int highestMembershipCount = (character.AttributeManager.Attributes["CAMembershipCount"] as VMembershipCountAttribute).highestValue;

            int score = Mathf.CeilToInt((singingAbility + gamingAbility + chattingAbility) * _configuration.abilityCoefficient +
                        follower * _configuration.followerCoefficient
                        + highestMembershipCount * _configuration.membershipCoefficient);
            
            var scoreLevel = _configuration.scoreLevels.Find(level => score >= level.low && score <= level.high);
            VDebug.Log("歌力： " + singingAbility);
            VDebug.Log("游戏力： " + gamingAbility);
            VDebug.Log("杂谈力： " + chattingAbility);
            VDebug.Log("关注人数： " + follower);
            VDebug.Log("最高舰长数： " + highestMembershipCount);
            VDebug.Log("分数： " + score);
            VDebug.Log("等级： " + scoreLevel);
            return new VScoreResult
            {
                score = score,
                scoreLevelName = scoreLevel.name
            };
        }
    }
}