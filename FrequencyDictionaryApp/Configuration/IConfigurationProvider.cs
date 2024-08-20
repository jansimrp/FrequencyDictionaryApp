using System.Text;

namespace FrequencyDictionaryApp.Configuration
{
    public interface IConfigurationProvider
    {
        Encoding Encoding { get; set; }
    }
}
