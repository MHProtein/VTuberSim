using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace SlayTheSpire.System.SavingSystem
{
    public enum VSavePointType
    {
        Dialog,
        Battle,
        ScheduleCreation,
        ListenerSystem,
        TutorialWeek
    }
    public class VDataPersistenceManager : VSingleton<VDataPersistenceManager>
    {
        private FileDataHandler _dataHandler;
        private FileDataHandler _tutorialBattleDataHandler;
        private FileDataHandler _tutorialWeekDataHandler;
        private SaveData _tutorialBattleSaveData;
        private SaveData _tutorialWeekSaveData;
        public SaveData SaveData { get; private set; }

        public SaveData TutorialBattleSaveData => _tutorialBattleSaveData;
        public SaveData TutorialWeekSaveData => _tutorialWeekSaveData;

        public List<IDataPersistence> DataPersistences { get; private set; }

        public void Register(IDataPersistence data)
        {
            DataPersistences.Add(data);
        }

        public void Initialize()
        {
            DataPersistences = new List<IDataPersistence>();
            _dataHandler = new FileDataHandler(Application.persistentDataPath, "player.vtb");
            _tutorialBattleDataHandler = new FileDataHandler(Application.persistentDataPath, "player_tutorial.vtb");
            _tutorialWeekDataHandler = new FileDataHandler(Application.persistentDataPath, "player_week.vtb");

            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEndRun, EventSaveGame);
        }

        private void Start()
        {
            LoadGame(false);
        }

        public void NewGame(bool isTutorial)
        {
            SaveData = new SaveData();
            if (!isTutorial)
                return;
            _tutorialBattleSaveData = new SaveData();
            _tutorialWeekSaveData = new SaveData();
            //GameManager.Instance.newGame = true;
        }

        public void DeleteSave()
        {
            SaveData = null;
            _dataHandler.Delete();
        }

        public bool SaveExists()
        {
            return _dataHandler.SaveExists();
        }

        public void LoadGame(bool isTutorial)
        {
            if (SaveData is null)
                SaveData = _dataHandler.Load();
            if (SaveData is null)
            {
                Debug.Log("No data was found. Initializing data to defaults");
                NewGame(isTutorial);
            }

            foreach (var dataPersistence in DataPersistences) dataPersistence.Load(SaveData);
        }

        public SaveData LoadSave()
        {
            SaveData = _dataHandler.Load();
            return SaveData;
        }

        public void EventSaveGame(Dictionary<string, object> message)
        {
            SaveGame(VSavePointType.ListenerSystem);
        }

        public void SaveGame(VSavePointType savePointType)
        {
            SavePersistences(SaveData, savePointType);
            _dataHandler.Save(SaveData);
        }

        public void SavePersistences(SaveData saveData, VSavePointType savePointType)
        {
            saveData.savePointType = savePointType;
            foreach (var dataPersistence in DataPersistences)
            {
                dataPersistence.Save(saveData);
            }
            saveData.saved = true;
        }

        public void SaveGameTutorialBattle()
        {            
            if (_tutorialBattleSaveData is null)
                return;
            SavePersistences(_tutorialBattleSaveData, VSavePointType.Battle);
            _tutorialBattleDataHandler.Save(_tutorialBattleSaveData);
        }

        public SaveData LoadTutorialBattleSave()
        {
            _tutorialBattleSaveData = _tutorialBattleDataHandler.Load();
            return _tutorialBattleSaveData;
        }

        public void LoadTutorialBattleGame()
        {
            if (_tutorialBattleSaveData is null)
                _tutorialBattleSaveData = _tutorialBattleDataHandler.Load();
            if (_tutorialBattleSaveData is null) VDebug.LogError("No tutorial save data was found.");

            foreach (var dataPersistence in DataPersistences) dataPersistence.Load(_tutorialBattleSaveData);
        }
        
        public void SaveGameTutorialWeek()
        {
            if (_tutorialWeekSaveData is null)
                return;
            SavePersistences(_tutorialWeekSaveData, VSavePointType.TutorialWeek);
            _tutorialWeekDataHandler.Save(_tutorialWeekSaveData);
        }

        public SaveData LoadTutorialWeekSave()
        {
            _tutorialWeekSaveData = _tutorialWeekDataHandler.Load();
            return _tutorialWeekSaveData;
        }

        public void LoadTutorialWeekGame()
        {
            if (_tutorialWeekSaveData is null)
                _tutorialWeekSaveData = _tutorialWeekDataHandler.Load();
            if (_tutorialWeekSaveData is null) VDebug.LogError("No tutorial save data was found.");

            foreach (var dataPersistence in DataPersistences) dataPersistence.Load(_tutorialWeekSaveData);
        }
    }
}