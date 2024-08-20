using FrequencyDictionaryApp.Configuration;
using FrequencyDictionaryApp.Services;
using System.Text;

namespace FrequencyDictionaryAppTest
{
    [TestFixture]
    public class FileOperationTest
    {

        private const string TestFilePath = "./testfile.txt";
        private const string OutputFilePath = "./output.txt";

        private IConfigurationProvider configurationProvider;
        private IFileOperationService textFileOperationService;

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
            File.WriteAllText(TestFilePath, "Hello World\nHello World");

            configurationProvider = new ConfigurationProvider();
            textFileOperationService = new TextFileOperationService(configurationProvider);

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
        public async Task ReadFileAsync_FileExists_ReturnsContent()
        {

            // Act
            var items = new List<string>();
            await foreach (var item in textFileOperationService.ReadAsync(TestFilePath))
            {
                items.Add(item);
            }

            // Assert
            Assert.IsNotNull(items);
            Assert.AreEqual(2, items.Count);
        }

        [Test]
        public void ReadFileAsync_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Act & Assert
            Assert.ThrowsAsync<FileNotFoundException>(async () => 
            {
                await foreach (var item in textFileOperationService.ReadAsync("nonexistentfile.txt"))
                {

                }
            });
        }

        [Test]
        public async Task WriteAsync_WritesFileSuccessfully_And_SortsInDescendingOrder()
        {
            // Arrange
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var dictionary = new Dictionary<string, int> { { "hello", 2 }, { "world", 2 } };

            // Act
            await textFileOperationService.WriteAsync(OutputFilePath, dictionary);

            // Assert
            Assert.IsTrue(File.Exists(OutputFilePath));
            var writtenContent = File.ReadAllText(OutputFilePath);
            Assert.AreEqual("hello,2\r\nworld,2\r\n", writtenContent);

            File.WriteAllText(TestFilePath, "Hello World\nHello");
            var updateddictionary = new Dictionary<string, int> { { "hello", 2 }, { "world", 1 } };

            await textFileOperationService.WriteAsync(OutputFilePath, updateddictionary);

            // Assert
            Assert.IsTrue(File.Exists(OutputFilePath));
            writtenContent = File.ReadAllText(OutputFilePath);
            Assert.AreEqual("hello,2\r\nworld,1\r\n", writtenContent);
        }
    }
}

