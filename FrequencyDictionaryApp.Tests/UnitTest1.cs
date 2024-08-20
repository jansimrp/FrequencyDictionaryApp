using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrequencyDictionaryApp.Tests
{
    [TestClass]
    public class FileoperationServiceTests
    {
        private const string TestFilePath = "testfile.txt";
        private const string OutputFilePath = "output.txt";

        [TestInitialize]
        public void Setup()
        {
            // Set up any necessary resources before each test
            File.WriteAllText(TestFilePath, "Hello World\nHello");
        }

        [TestCleanup]
        public void TearDown()
        {
            // Clean up resources after each test
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);

            if (File.Exists(OutputFilePath))
                File.Delete(OutputFilePath);
        }

        [TestMethod]
        public void ReadFile_FileExists_ReturnsContent()
        {
            // Arrange
            var processor = new FileOperationService();

            // Act
            string content = processor.ReadFile(TestFilePath);

            // Assert
            Assert.IsNotNull(content);
            Assert.AreEqual("Hello World\nHello", content);
        }

        [Test]
        public void ReadFile_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var processor = new FileProcessor();

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => processor.ReadFile("nonexistentfile.txt"));
        }

        [Test]
        public void WriteFrequencyDictionary_WritesFileSuccessfully()
        {
            // Arrange
            var processor = new FileProcessor();
            var dictionary = new Dictionary<string, int> { { "hello", 2 }, { "world", 1 } };

            // Act
            processor.WriteFrequencyDictionary(OutputFilePath, dictionary);

            // Assert
            Assert.IsTrue(File.Exists(OutputFilePath));
            var writtenContent = File.ReadAllText(OutputFilePath);
            Assert.AreEqual("hello,2\nworld,1\n", writtenContent);
        }
    }
}
