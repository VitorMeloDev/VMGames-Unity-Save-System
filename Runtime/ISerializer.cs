using System.Threading.Tasks;

namespace VMGames.Core.SaveSystem
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
    }

    public interface IStorageStrategy
    {
        Task WriteTextAsync(string path, string data);
        Task<string> ReadTextAsync(string path);
        bool Exists(string path);
        void Delete(string path);
    } 
}