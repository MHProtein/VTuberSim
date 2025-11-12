namespace VTuber.BattleSystem.Effect
{
    public class VUpgradableValue<T>
    {
        private readonly T _baseValue;

        public VUpgradableValue(T baseValue, T upgradedValue)
        {
            _baseValue = baseValue;
            UpgradedValue = upgradedValue;
            IsUpgraded = false;
        }

        public bool IsUpgraded { get; private set; }

        public T Value => IsUpgraded ? UpgradedValue : _baseValue;
        public T UpgradedValue { get; }

        public void Upgrade()
        {
            IsUpgraded = true;
        }

        public void Downgrade()
        {
            IsUpgraded = false;
        }
    }
}