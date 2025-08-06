using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine;
using VTuber.BattleSystem.Card;

namespace VTuber.BattleSystem.Buff
{
    public class VRelicConfiguration
    {
        public uint id;
        public string relicName;
        public Sprite icon;
        public bool isUnique;
        public List<VEffectItem> effectItems;

        public VRelicConfiguration(CellRange row)
        {
            id = Convert.ToUInt32(row.Columns[0].Value);
            relicName = row.Columns[1].Value;
            isUnique = Convert.ToInt32(row.Columns[2].Value) == 1;

            effectItems = new List<VEffectItem>();
            for (int i = 3; i < row.Columns.Length; i += 2)
            {
                var effectStr = row.Columns[i].Value;
                if (string.IsNullOrWhiteSpace(effectStr)) continue;

                uint effectId = Convert.ToUInt32(effectStr);
                effectItems.Add(new VEffectItem
                {
                    id = effectId,
                    parameter = row.Columns[i + 1].Value,
                    upgradedParameter = row.Columns[i + 1].Value
                });
            }
        }

        public VRelic CreateRelic()
        {
            return new VRelic(this, effectItems.Select(e => e.CreateEffect()).ToList());
        }
    }

}