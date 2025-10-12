using System.Collections.Generic;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattleLookUpTables : VSingleton<VBattleLookUpTables>
    {
        private Dictionary<int, VValueModifier<int>> _gainValueModifiers;
        private Dictionary<int, VValueModifier<float>> _gainRateModifiers;
        
        public int IDDistributor => _idDistributor;
        private int _idDistributor = 0;
        public void Initialize(VBattleSaveData saveData)
        {
            if (saveData is not null)
                _idDistributor = saveData.battleLookUpIDDistributor;
            _gainValueModifiers = new Dictionary<int, VValueModifier<int>>();
            _gainRateModifiers = new Dictionary<int, VValueModifier<float>>();
        }
        
        public VValueModifier<int> GetGainValueModifier(int id)
        {
            if (_gainValueModifiers.ContainsKey(id))
                return _gainValueModifiers[id];
            return null;
        }

        public VValueModifier<float> GetGainRateModifier(int id)
        {
            if (_gainRateModifiers.ContainsKey(id))
                return _gainRateModifiers[id];
            return null;
        }

        public void AddGainValueModifier(VValueModifier<int> modifier)
        {
            if (modifier.ID == -1)
            {
                modifier.SetID(_idDistributor);
                _gainValueModifiers.Add(_idDistributor++, modifier);
            }
            else
            {
                _gainValueModifiers.Add(modifier.ID, modifier);
            }
        }

        public void AddGainRateModifier(VValueModifier<float> modifier)
        {
            if (modifier.ID == -1)
            {
                modifier.SetID(_idDistributor);
                _gainRateModifiers.Add(_idDistributor++, modifier);
            }
            else
            {
                _gainRateModifiers.Add(modifier.ID, modifier);
            }
        }
    }
}