using System.Collections.Concurrent;

namespace FrequencyDictionaryApp.Services
{
    public class FrequencyGeneratorService
    {
        private IFileOperationService _fileOperationService { get; set; }
        public FrequencyGeneratorService(IFileOperationService fileOperationService)
        {
            _fileOperationService = fileOperationService;
        }


        // Read the file
        // Split the file asynchronously
        // Generate the frequency of each word
        // write the file into the outputfile
        public async Task<bool> GenerateWordFrequencyAsync(string inputFilePath, string outputFilePath)
        {
            var wordsToProcess = new List<string>();
            await foreach (var item in _fileOperationService.ReadAsync(inputFilePath))
            {
                var words = item
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => word.ToLowerInvariant());

                wordsToProcess.AddRange(words);
            }

            var frequencyDictionary = await GetWordFrequencyAsync(wordsToProcess);
            var issuccess = await _fileOperationService.WriteAsync(outputFilePath, frequencyDictionary);
            return issuccess;
        }

        // Get the frequency of each word in the text
        private async Task<IDictionary<string, int>> GetWordFrequencyAsync(IEnumerable<string> words)
        {
            var processorCount = Environment.ProcessorCount;
            int wordChunkCount = (int)Math.Ceiling((double)words.Count() / processorCount);
            var tasks = new Task[processorCount];
            var frequencyDict = new ConcurrentDictionary<string, int>();
            for (int i = 0; i < processorCount; i++)
            {
                int taskIndex = i;
                tasks[i] = Task.Run(() =>
                {
                    var chunk = words.Skip(taskIndex * wordChunkCount).Take(wordChunkCount);
                    foreach (var word in chunk)
                    {
                        frequencyDict.AddOrUpdate(word, 1, (key, oldValue) => oldValue + 1);
                    }
                });
            }

            await Task.WhenAll(tasks);

            return frequencyDict
                .OrderByDescending(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

        }
    }
}

