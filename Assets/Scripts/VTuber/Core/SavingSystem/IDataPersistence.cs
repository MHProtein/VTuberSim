namespace SlayTheSpire.System.SavingSystem
{
    public interface IDataPersistence
    {
        public void Load(SaveData data);
        public void Save(SaveData data);
    }
}