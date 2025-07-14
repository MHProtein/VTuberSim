using System.Collections.Generic;
using CsvHelper;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;

namespace VTuber.BattleSystem.Effect
{
    public class VEffectConfiguration
    {
        public int id;
        public string effectName;
        [TextArea] public string description;
        public bool multiplyByLayer = false;
        
        public VEffectCondition condition;

        public VEffectConfiguration(CsvReader csv)
        {
            id = csv.GetField<int>("ID");
            effectName = csv.GetField<string>("Name");
            description = csv.GetField<string>("Description");
            multiplyByLayer = csv.GetField<int>("MultiplyByLayer") == 1;
            
            //todo: add condition parsing
            //condition = ;
        }
        
        public virtual VEffect CreateEffect()
        {
            return new VEffect(this);
        }
    }
}