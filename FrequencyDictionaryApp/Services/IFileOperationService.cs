namespace FrequencyDictionaryApp.Services
{
    public interface IFileOperationService
    {
        IAsyncEnumerable<string> ReadAsync(string filePath);

        Task<bool> WriteAsync(string filePath, IDictionary<string, int> frequencyDict);
    }
}
