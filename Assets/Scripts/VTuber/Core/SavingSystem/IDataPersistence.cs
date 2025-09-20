namespace SlayTheSpire.System.SavingSystem
{
    public interface IDataPersistence
    {
        public void Load(GameData data);
        public void Save(GameData data);
    }
}