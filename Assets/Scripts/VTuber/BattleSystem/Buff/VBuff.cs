using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuff
    {
        
        private VBuffConfiguration _configuration;
        
        // Buff对应的所有效果
        public List<VEffect> Effects => _effects;
        private List<VEffect> _effects;
        
        // 配置表中的唯一ID
        public uint ConfigId => _configuration.id;
        
        // 是否为永久Buff
        public bool IsPermanent => _configuration.IsBuffPermanent();
        
        // 延迟生效的回合数
        public int latency;
        
        public VBuff(VBuffConfiguration configuration, List<VEffect> effects)
        {
            _configuration = configuration;
            _effects = effects;
            latency = _configuration.latency;
        }

        // 是否可以叠加
        public virtual bool IsStackable()
        {
            return _configuration.stackable;
        }
        
        // 获取Buff名称
        public string GetBuffName()
        {
            return _configuration.buffName;
        }
    }
}