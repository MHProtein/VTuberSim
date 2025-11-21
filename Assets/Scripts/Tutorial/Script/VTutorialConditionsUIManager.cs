using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace Tutorial.Script
{
    public class VTutorialConditionsUIManager : VSingletonMonobehaviour<VTutorialConditionsUIManager>
    {
        [SerializeField] private VTutorialConditionUI conditionPrefab;
        [SerializeField] private Transform parent;
        private List<VTutorialConditionUI> _conditions;

        public void SetConditions(List<VTutorialWeekCondition> conditions, VCharacter character)
        {
            if (_conditions is not null)
            {
                foreach (var condition in _conditions)
                {
                    Destroy(condition.gameObject);
                }
                _conditions.Clear();
            }
            _conditions = new List<VTutorialConditionUI>();
            foreach (var condition in conditions)
            {
                var ui = Instantiate(conditionPrefab, parent);
                ui.SetCondition(condition, character);
                _conditions.Add(ui);
            }
        }

        public void Clear()
        {
            if (_conditions is null)
                return;
            foreach (var condition in _conditions)
            {
                Destroy(condition.gameObject);
            }
            _conditions.Clear();
        }
    }
}