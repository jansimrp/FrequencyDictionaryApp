using FluentValidation;
using System.Text;

namespace FrequencyDictionaryApp.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        private readonly AbstractValidator<string[]> validator;
        private readonly int codePage = 1252;

        public Encoding? Encoding { get; set; }

        public ConfigurationProvider()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ConfigureEncoding();
        }

        private void ConfigureEncoding()
        {
            Encoding = Encoding.GetEncoding(codePage);
        }
    }
}
