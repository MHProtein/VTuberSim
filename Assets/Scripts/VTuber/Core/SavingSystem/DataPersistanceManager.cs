using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace SlayTheSpire.System.SavingSystem
{
    public class VDataPersistenceManager : VSingleton<VDataPersistenceManager>
    {
        public SaveData SaveData => _saveData;
        private SaveData _saveData;
        
        public SaveData TutorialSaveData => _saveData;
        private SaveData _tutorialSaveData;
        
        public List<IDataPersistence> DataPersistences =>_dataPersistences;
        private List<IDataPersistence> _dataPersistences;

        private FileDataHandler _dataHandler;
        private FileDataHandler _tutorialDataHandler;
        
        public void Register(IDataPersistence data)
        {
            _dataPersistences.Add(data);
        }

        public void Initialize()
        {
            _dataPersistences = new List<IDataPersistence>();
            _dataHandler = new FileDataHandler(Application.persistentDataPath, "player.vtb");
            _tutorialDataHandler = new FileDataHandler(Application.persistentDataPath, "player_tutorial.vtb");
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEndSave, EventSaveGame);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEndRun, EventSaveGame);
        }
        
        private void Start()
        {
            LoadGame();
        }

        public void NewGame()
        {
            _saveData = new SaveData();
            //GameManager.Instance.newGame = true;
        }

        public void DeleteSave()
        {
            _saveData = null;
            _dataHandler.Delete();
        }

        public bool SaveExists()
        {
            return _dataHandler.SaveExists();
        }

        public void LoadGame()
        {
            if(_saveData is null) 
                _saveData = _dataHandler.Load();
            if (_saveData is null)
            {
                Debug.Log("No data was found. Initializing data to defaults");
                NewGame();
            }

            foreach (var dataPersistence in _dataPersistences)
            {
                dataPersistence.Load(_saveData);
            }
        }

        public SaveData LoadSave()
        {
            _saveData = _dataHandler.Load();
            return _saveData;
        }

        public void EventSaveGame(Dictionary<string, object> message)
        {
            SaveGame();
        }
        
        public void SaveGame()
        {
            SavePersistences().Wait();
            _dataHandler.Save(_saveData);
        }

        public async Task SavePersistences()
        {
            foreach (var dataPersistence in _dataPersistences)
            {
                dataPersistence.Save(_saveData);
                await Task.CompletedTask;
            }
        }

        public void SaveGameTutorial()
        {
            SavePersistences().Wait();
            _tutorialDataHandler.Save(_saveData);
        }

        public SaveData LoadTutorialSave()
        {
            _tutorialSaveData = _tutorialDataHandler.Load();
            return _tutorialSaveData;
        }
        
        public void LoadTutorialGame()
        {
            if(_tutorialSaveData is null) 
                _tutorialSaveData = _tutorialDataHandler.Load();
            if (_tutorialSaveData is null)
            {
                VDebug.LogError("No tutorial save data was found.");
            }

            foreach (var dataPersistence in _dataPersistences)
            {
                dataPersistence.Load(_tutorialSaveData);
            }
        }
    }
}