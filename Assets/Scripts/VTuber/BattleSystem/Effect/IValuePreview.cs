using VTuber.BattleSystem.Core;

namespace VTuber.BattleSystem.Effect
{
    public interface IVValuePreview
    {
        public string AttributeName { get; }
        public int GetValue(VBattle battle);
    }
}