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
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnParameterChange, OnParameterChange);
        }

        private void OnParameterChange(Dictionary<string, object> messagedict)
        {
            if (_battleAttributes.TryGetValue("BAParameter", out var parameter))
            {
                float multiplier = _battleAttributes["BASingingMultiplier"].Value / 100f;
                int delta = (int)messagedict["Delta"];
                _battleAttributes["BAPopularity"].AddTo((int)(delta * multiplier), false);
            }
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
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnParameterChange, OnParameterChange);
        }

        public void AddAttribute(string name, VBattleAttribute attribute)
        {
            _battleAttributes.Add(name, attribute);
            attribute.AttributeName = name;
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

        public void ApplyCost(int cost)
        {
            var stamina = _battleAttributes["BAStamina"] as VBattleStaminaAttribute;
            var shield = _battleAttributes["BAShield"];

            int costAfterShield = cost - shield.Value;

            shield.AddTo(-cost, false);
            if (costAfterShield <= 0)
                return;
            
            stamina.AddTo(-costAfterShield, false);
        }

        public bool TestCost(int cost)
        {
            var stamina = _battleAttributes["BAStamina"] as VBattleStaminaAttribute;
            var shield = _battleAttributes["BAShield"];
            
            int costAfterShield = cost - shield.Value;
            
            if (costAfterShield <= 0)
                return true;
            
            return stamina.TestCost(-costAfterShield);
        }
    }
}