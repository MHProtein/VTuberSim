namespace VTuber.BattleSystem.Buff
{
    public class VAdditionBuff : VBuff
    {
        private VAdditionBuffConfiguration _configuration;
        
        private int _addAmount => _configuration.addAmount;
        private int _stackCount = 1;
        private int _turnsLeft;
        
        public VAdditionBuff(VBuffConfiguration configuration) : base(configuration)
        {
            _configuration = (VAdditionBuffConfiguration)configuration;
            _turnsLeft = configuration.duration;
        }

        public override void Stack(VBuff buff)
        {
            switch (_configuration.buffTemporalType)
            {
                
            }
        }
        
        
        public override bool IsStackable(VBuff buff)
        {
            if (base.IsStackable(buff))
            {
                if (((VAdditionBuff)buff)._addAmount == _addAmount)
                    return true;
            }

            return false;
        }

        public override int ApplyBuff(int value)
        {
            return value + _configuration.addAmount;
        }
    }
}