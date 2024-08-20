using FrequencyDictionaryApp.Configuration;

namespace FrequencyDictionaryApp.Services
{
    public class TextFileOperationService : IFileOperationService
    {

        public TextFileOperationService(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        //make the encoding DI
        private IConfigurationProvider _configurationProvider { get; set; }


        // Write the frequency dictionary to the output file
        // Handle exception and extract write file as an interface method 
        public async Task<bool> WriteAsync(string filePath, IDictionary<string, int> frequencyDict)
        {
            var content = frequencyDict.Select(x => $"{x.Key},{x.Value}").ToArray();
            await File.WriteAllLinesAsync(filePath, content, _configurationProvider.Encoding);
            return await Task.FromResult(true);
        }

        //public async Task<string> ReadAsync(string filePath)
        //{
        //    if (!File.Exists(filePath))
        //    {
        //        throw new FileNotFoundException("The input file does not exist.");
        //    }

        //    return await File.ReadAllTextAsync(filePath, _configurationProvider.Encoding);
        //}

        async IAsyncEnumerable<string> IFileOperationService.ReadAsync(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                using (StreamReader sr = new StreamReader(fileStream, _configurationProvider.Encoding))
                {
                    string? line = default;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }
    }
}
