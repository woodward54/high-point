using System.Collections.Generic;

namespace Systems.Persistence
{
    public interface IDataService
    {
        void Save(GameData data, bool overwrite = true);
        GameData Load();
        void Delete();
        // void DeleteAll();
    }
}