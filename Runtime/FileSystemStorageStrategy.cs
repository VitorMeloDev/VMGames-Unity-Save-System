using System.IO;
using System.Threading.Tasks;
using VMGames.Core.SaveSystem;

namespace VMGames.SaveSystem.Unity
{
    // Adaptador de IStorageStrategy para usar sistema de archivos de Unity
    public class FileSystemStorageStrategy : IStorageStrategy
    {
        public async Task WriteTextAsync(string path, string data)
        {
            // O task.Run é usado para executar a operação de escrita em um thread separado, evitando bloquear o thread principal.
            await Task.Run(() => File.WriteAllText(path, data));
        }

        public async Task<string> ReadTextAsync(string path)
        {
            return await Task.Run(() => File.ReadAllText(path));
        }

        public bool Exists(string path) => File.Exists(path);

        public void Delete(string path)
        {
            if (Exists(path))
                File.Delete(path);
        }
    }

    public class UnityJsonSerializer : ISerializer
    {
        public string Serialize<T>(T obj) => UnityEngine.JsonUtility.ToJson(obj);
        public T Deserialize<T>(string data) => UnityEngine.JsonUtility.FromJson<T>(data);
    }
}