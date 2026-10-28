using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using VMGames.Core.SaveSystem;
using VMGames.SaveSystem.Unity;

namespace VMGames.SaveSystem.Tests.Integration
{
    [TestFixture]
    public class SaveServiceIntegrationTests
    {
        private SaveService _saveService;
        private string _testFilePath;

        // Classe de teste que espelha dados reais de um jogo
        [System.Serializable]
        public class PlayerData
        {
            public int Health;
            public Vector3 Position; // Dependência da Unity testada na prática
        }

        [SetUp]
        public void Setup()
        {
            // Injetamos as dependências REAIS
            _saveService = new SaveService(new FileSystemStorageStrategy(), new UnityJsonSerializer());
            
            // Garantimos que o arquivo será gravado num local seguro e temporário do SO
            _testFilePath = Path.Combine(Application.persistentDataPath, "test_save.json");
        }

        [TearDown]
        public void TearDown()
        {
            // Limpamos o lixo físico do HD após cada teste para não poluir o computador do desenvolvedor
            if (File.Exists(_testFilePath))
                File.Delete(_testFilePath);
        }

        [Test]
        public async Task SaveAndLoad_RealDisk_PersistsAndRestoresData()
        {
            // Arrange
            var originalData = new PlayerData 
            { 
                Health = 100, 
                Position = new Vector3(1.5f, 2.0f, 3.5f) 
            };

            // Act: Grava fisicamente no disco
            await _saveService.SaveAsync(_testFilePath, originalData);

            // Assert intermediário: O sistema operacional realmente criou o arquivo?
            Assert.IsTrue(File.Exists(_testFilePath), "O arquivo físico deveria existir no disco.");

            // Act: Carrega do disco
            var loadedData = await _saveService.LoadAsync<PlayerData>(_testFilePath);

            // Assert final: O contrato se manteve ao passar pelo HD e pela Unity?
            Assert.IsNotNull(loadedData);
            Assert.AreEqual(100, loadedData.Health);
            Assert.AreEqual(originalData.Position, loadedData.Position);
        }
    }
}