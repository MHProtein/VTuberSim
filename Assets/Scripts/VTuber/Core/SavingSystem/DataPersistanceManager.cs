using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace SlayTheSpire.System.SavingSystem
{
    public class DataPersistenceManager : VSingleton<DataPersistenceManager>
    {
        public GameData GameData =>_gameData;
        private GameData _gameData;
        
        public List<IDataPersistence> DataPersistences =>_dataPersistences;
        private List<IDataPersistence> _dataPersistences;

        private FileDataHandler _dataHandler;
        
        public void Register(IDataPersistence data)
        {
            _dataPersistences.Add(data);
        }

        public void Initialize()
        {
            _dataPersistences = new List<IDataPersistence>();
            _dataHandler = new FileDataHandler(Application.persistentDataPath, "player.vtb");
            
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, EventSaveGame);
        }
        
        private void Start()
        {
            LoadGame();
        }

        public void NewGame()
        {
            _gameData = new GameData();
            //GameManager.Instance.newGame = true;
        }

        public bool SaveExists()
        {
            return _dataHandler.SaveExists();
        }

        public void LoadGame()
        {
            _gameData = _dataHandler.Load();
            if (_gameData is null)
            {
                Debug.Log("No data was found. Initializing data to defaults");
                NewGame();
            }

            foreach (var dataPersistence in _dataPersistences)
            {
                dataPersistence.Load(_gameData);
            }
        }

        public void EventSaveGame(Dictionary<string, object> message)
        {
            SaveGame();
        }
        
        public void SaveGame()
        {
            SavePersistences().Wait();
            _dataHandler.Save(_gameData);
        }

        public async Task SavePersistences()
        {
            foreach (var dataPersistence in _dataPersistences)
            {
                dataPersistence.Save(_gameData);
                await Task.CompletedTask;
            }
        }
    }
}