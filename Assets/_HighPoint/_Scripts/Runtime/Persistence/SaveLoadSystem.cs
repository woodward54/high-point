using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.Persistence
{
    [Serializable]
    public class GameData
    {
        public PlayerData playerData;

        public GameData()
        {
            playerData = new();
        }
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }

    public interface IBind<TData> where TData : ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }

    public class SaveLoadSystem : SingletonPersistent<SaveLoadSystem>
    {
        [SerializeReference] GameData gameData;

        IDataService dataService;

        protected override void OnAwake()
        {
            dataService = new FileDataService(new JsonSerializer());

            LoadGame();
        }

        // void Start() => LoadGame();

        // void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        // void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        // {
        //     // if (scene.name == "Menu") return;

        //     Bind();
        // }

        void Bind()
        {
            Bind<Player, PlayerData>(gameData.playerData);
            // Bind<Inventory.Inventory, InventoryData>(gameData.inventoryData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null)
            {
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }

        void Bind<T, TData>(List<TData> datas) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new()
        {
            var entities = FindObjectsByType<T>(FindObjectsSortMode.None);

            foreach (var entity in entities)
            {
                var data = datas.FirstOrDefault(d => d.Id == entity.Id);
                if (data == null)
                {
                    data = new TData { Id = entity.Id };
                    datas.Add(data);
                }
                entity.Bind(data);
            }
        }

        public void SaveGame() => dataService.Save(gameData);

        public void LoadGame()
        {
            var loadedData = dataService.Load();

            if (loadedData != null)
            {
                gameData = loadedData;
            }
            else
            {
                gameData = new();
            }

            Bind();
        }

        public void DeleteGame() => dataService.Delete();
    }
}