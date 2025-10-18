using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.BattleAttribute
{
    public class VColorSaveData
    {
        public float r;
        public float g;
        public float b;
        public float a;

        public VColorSaveData() { }

        public VColorSaveData(Color color)
        {
            r = color.r;
            g = color.g;
            b = color.b;
            a = color.a;
        }

        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }
    public class VBattleMultiplierAttribute : VBattleAttribute
    {
        public readonly Color color;
        public VBattleMultiplierAttribute(int value, Color color) : base(value, true, VBattleEventKey.OnMultiplierChange)
        {
            this.color = color;
        }

        public VBattleMultiplierAttribute(VBattleAttributeSaveData saveData) : base(saveData)
        {
            color = saveData.color.ToColor();
        }

        public override VBattleAttributeSaveData Save()
        {
            var data = base.Save();
            data.color = new VColorSaveData(color);
            return data;
        }

        protected override void InitSetValue(int value, bool isFromCard, bool shouldPlayTwice = false)
        {
            SetValue(value, isFromCard, shouldPlayTwice);
        }

        public override void SendEvent(int newValue, int delta, bool isFromCard, bool shouldPlayTwice = false)
        {
            var messageDict = new Dictionary<string, object>
            {
                { "Name", AttributeName },
                { "NewValue", newValue },
                { "Delta", delta },
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice },
                { "MaxValue", _maxValue }
            };
            VBattleRootEventCenter.Instance.Raise(_eventKey, messageDict);
            
            messageDict.Add("AttributeName", AttributeName);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnAttributeValueChange, messageDict);
        }

    }
}