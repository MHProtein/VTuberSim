using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.EventCenter;

namespace VTuber.CoopSystem
{
    public class VCooperatorManager
    {
        private readonly List<VCooperator> _cooperators = new();

        public void AddCooperator(VCooperatorConfiguration configuration)
        {
            var cooperator = new VCooperator(configuration);
            AddCooperator(cooperator);
        }

        public void AddCooperator(VCooperator cooperator)
        {
            _cooperators.Add(cooperator);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorAdded,
                new Dictionary<string, object> { { "Cooperator", cooperator } });
            cooperator.OnEnable();
        }

        public List<VCoopEventItem> GetCoopEvents(List<Vector2Int> occupiedPositions)
        {
            var events = new List<VCoopEventItem>();
            foreach (var cooperator in _cooperators)
            {
                events.AddRange(cooperator.GenerateCoopEventPositions(occupiedPositions));
                occupiedPositions.AddRange(events.Select(x => x.position).ToList());
            }

            return events;
        }

        public VCooperator GetCooperator(uint id)
        {
            return _cooperators.Find(x => x.Id == id);
        }

        public List<VCooperator> GetCooperators()
        {
            return _cooperators;
        }

        public void Clear()
        {
            foreach (var cooperator in _cooperators)
            {
                cooperator.OnDisable();
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorRemoved,
                    new Dictionary<string, object> { { "Cooperator", cooperator } });
            }

            _cooperators.Clear();
        }
    }
}