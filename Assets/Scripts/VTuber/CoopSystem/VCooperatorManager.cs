using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.Core.EventCenter;

namespace VTuber.CoopSystem
{
    public class VCooperatorManager
    {
        List<VCooperator> _cooperators = new List<VCooperator>();

        public void AddCooperator(VCooperatorConfiguration configuration)
        {
            var cooperator = new VCooperator(configuration);
            _cooperators.Add(cooperator);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorAdded, new Dictionary<string, object> { { "Cooperator", cooperator } });
        }

        public List<VCoopEventItem> GetCoopEvents(List<Vector2Int> occupiedPositions)
        {
            List<VCoopEventItem> events = new List<VCoopEventItem>();
            foreach (var cooperator in _cooperators)
            {
                events.AddRange(cooperator.GenerateCoopEventPositions(occupiedPositions));
            }
            occupiedPositions.AddRange(events.Select(x => x.position).ToList());
            return events;
        }
    }
}