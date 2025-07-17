namespace VTuber.BattleSystem.Effect
{
    public class VUpgradableValue<T>
    {
        T _baseValue;
        T _upgradedValue;
        bool _isUpgraded;
        
        public T Value => _isUpgraded ? _upgradedValue : _baseValue;
        
        public VUpgradableValue(T baseValue, T upgradedValue)
        {
            _baseValue = baseValue;
            _upgradedValue = upgradedValue;
            _isUpgraded = false;
        }
        
        public void Upgrade()
        {
            _isUpgraded = true;
        }
        
        public void Downgrade()
        {
            _isUpgraded = false;
        }
        
    }
}