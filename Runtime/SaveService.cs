using System;
using System.Threading.Tasks;

namespace VMGames.Core.SaveSystem
{
    public class SaveService
    {
        private readonly ISerializer _serializer;
        private readonly IStorageStrategy _storageStrategy;

        public SaveService(IStorageStrategy storageStrategy, ISerializer serializer)
        {
            _storageStrategy = storageStrategy;
            _serializer = serializer;
        }

        public async Task SaveAsync<T>(string path, T data)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            string content = _serializer.Serialize(data);
            await _storageStrategy.WriteTextAsync(path, content);
        }

        public async Task<T> LoadAsync<T>(string path, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));

            if (!_storageStrategy.Exists(path))
                return defaultValue;

            string content = await _storageStrategy.ReadTextAsync(path);

            if (string.IsNullOrEmpty(content))
                return defaultValue;
            
            return _serializer.Deserialize<T>(content);
        }
    }
}