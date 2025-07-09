using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.BattleSystem.Buff;
using VTuber.Character;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Core
{
    public class VBattleAttributeManager
    {
         // //from stamina
         // public int MaxStamina { get; private set; } 
         //
         // //from followers
         // public int InitViewerCount { get; private set; } 
         //
         // //from streaming stats
         // public float SingingScoringMultiplier { get; private set; }
         // public float GamingScoringMultiplier { get; private set; }
         // public float ChattingScoringMultiplier { get; private set; }
         //
         // //from memberships
         // public int MembershipCount { get; private set; }

        private Dictionary<string, VBattleAttribute> _battleAttributes;
        
        public VBattleAttributeManager(VCharacterAttributeManager characterAttributeManager)
        {
            _battleAttributes = new Dictionary<string, VBattleAttribute>();
        }
        
        public void OnEnable()
        {
            foreach (var attribute in _battleAttributes)
            {
                attribute.Value.OnEnable();
            }
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnResolution, OnTurnResolution);
        }

        private void OnTurnResolution(Dictionary<string, object> messagedict)
        {
            var parameter = _battleAttributes["BAParameter"].Value;
            float multiplier = _battleAttributes["BASingingMultiplier"].Value / 100f;
            _battleAttributes["BAPopularity"].AddTo((int)(parameter * multiplier));

        }

        public bool TryGetAttribute(string name, out VBattleAttribute attribute)
        {
            return _battleAttributes.TryGetValue(name, out attribute);
        }
        
        public void OnDisable()
        {
            foreach (var attribute in _battleAttributes)
            {
                attribute.Value.OnDisable();
            }
        }

        public void AddAttribute(string name, VBattleAttribute attribute)
        {
            _battleAttributes.Add(name, attribute);
            attribute.OnEnable();
        }
        
        public void RemoveAttribute(string name)
        {
            if (_battleAttributes.TryGetValue(name, out var attribute))
            {
                attribute.OnDisable();
                _battleAttributes.Remove(name);
            }
        }
    }
}