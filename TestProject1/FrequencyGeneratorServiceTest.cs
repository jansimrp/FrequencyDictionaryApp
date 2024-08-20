using FrequencyDictionaryApp.Configuration;
using FrequencyDictionaryApp.Services;

namespace FrequencyDictionaryAppTest
{
    [TestFixture]
    public class FrequencyGeneratorServiceTest
    {
        private IFileOperationService _fileOperationService;
        private FrequencyGeneratorService frqGeneratorService;
        private const string TestFilePath = "./testfile.txt";
        private const string OutputFilePath = "./output.txt";

        private string[] args = new[] { TestFilePath, OutputFilePath };
        private IConfigurationProvider configurationProvider;
        [SetUp]
        public void Setup()
        {
            if (!File.Exists(TestFilePath))
            {
                var stream = File.Create(TestFilePath);
                stream.Dispose();

            }

            if (!File.Exists(OutputFilePath))
            {
                var stream1 = File.Create(OutputFilePath);
                stream1.Dispose();
            }

            configurationProvider = new ConfigurationProvider();
            _fileOperationService = new TextFileOperationService(configurationProvider);
            frqGeneratorService = new FrequencyGeneratorService(_fileOperationService);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(TestFilePath))
            {
                File.Delete(TestFilePath);
            }

            if (File.Exists(OutputFilePath))
            {
                File.Delete(OutputFilePath);
            }
        }

        [Test]
        public async Task GenerateFrequency_ValidText_ReturnsCorrectFrequency()
        {
            // Arrange

            File.WriteAllText(TestFilePath, "Hello world hello");

            // Act
            var result = await frqGeneratorService.GenerateWordFrequencyAsync(TestFilePath, OutputFilePath);

            var words = File.ReadAllLines(OutputFilePath);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, words.Count());
            Assert.AreEqual(2, int.Parse(words[0].Split(',').Last()));
            Assert.AreEqual("hello", words[0].Split(',').First());
            Assert.AreEqual(1, int.Parse(words[1].Split(',').Last()));
            Assert.AreEqual("world", words[1].Split(',').First());
        }

        [Test]
        public async Task GenerateFrequency_EmptyText_ReturnsEmptyDictionary()
        {
         
            // Act
            var result = await frqGeneratorService.GenerateWordFrequencyAsync(TestFilePath, OutputFilePath);
            var words = File.ReadAllLines(OutputFilePath);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(0, words.Count());
        }

        [Test]
        public async Task GenerateFrequency_MixedCaseWords_ReturnsCaseInsensitiveFrequency()
        {
            // Arrange
            File.WriteAllText(TestFilePath, "Hello hEllo HELLO World");

            // Act
            var result = await frqGeneratorService.GenerateWordFrequencyAsync(TestFilePath, OutputFilePath);
            var words = File.ReadAllLines(OutputFilePath);
            
            // Assert

            Assert.IsTrue(result);
            Assert.AreEqual(2, words.Count());
            Assert.AreEqual(3, int.Parse(words[0].Split(',').Last()));
            Assert.AreEqual("hello", words[0].Split(',').First());
            Assert.AreEqual(1, int.Parse(words[1].Split(',').Last()));
            Assert.AreEqual("world", words[1].Split(',').First());
        }

        [Test]
        public async Task GenerateFrequency_MultipleSpacesAndNewLines_IgnoresEmptyEntries()
        {
            // Arrange
            File.WriteAllText(TestFilePath, "Hello   \n\n World \n Hello");

            // Act
            var result = await frqGeneratorService.GenerateWordFrequencyAsync(TestFilePath, OutputFilePath);
            var words = File.ReadAllLines(OutputFilePath);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, words.Count());
            Assert.AreEqual(2, int.Parse(words[0].Split(',').Last()));
            Assert.AreEqual("hello", words[0].Split(',').First());
            Assert.AreEqual(1, int.Parse(words[1].Split(',').Last()));
            Assert.AreEqual("world", words[1].Split(',').First());
        }
    }
}
