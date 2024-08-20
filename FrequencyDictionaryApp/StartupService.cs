using FluentValidation;
using FrequencyDictionaryApp.Configuration;
using FrequencyDictionaryApp.Services;
using FrequencyDictionaryApp.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrequencyDictionaryApp
{
    public class StartupService
    {
        static async Task Main(string[] args)
        {
            IHost host = InitialseServices();

            try
            {
                var userInputValidator = host.Services.GetService<AbstractValidator<string[]>>();

                Console.Write("Please Enter the input text file path: ");
                string inputFilePath = Console.ReadLine();

                // Prompt the user for the second input
                Console.Write("Please Enter the output text file path: ");
                string OutputFilePath = Console.ReadLine();

                args = new string[] { inputFilePath, OutputFilePath };

                if (userInputValidator.Validate(args) is var result && !result.IsValid)
                {
                    throw new Exception(string.Join("\n", result.Errors));
                }

                var frequencyGeneratorService = host.Services.GetService<FrequencyGeneratorService>();
                await frequencyGeneratorService.GenerateWordFrequencyAsync(inputFilePath, OutputFilePath);

                Console.WriteLine("Frequency dictionary generated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


        }
        public static IHost InitialseServices()
        {
            //initialise the services
            var hostBuilder = new HostBuilder()
           .ConfigureServices((context, services) =>
           {
               services.AddSingleton<AbstractValidator<string[]>, UserInputValidator>();
               services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
               services.AddSingleton<IFileOperationService, TextFileOperationService>(provider =>
               {
                   var configurationProvider = provider.GetRequiredService<IConfigurationProvider>();
                   return new TextFileOperationService(configurationProvider);
               });

               services.AddSingleton<FrequencyGeneratorService>(provider =>
               {
                   var fileservice = provider.GetRequiredService<IFileOperationService>();
                   return new FrequencyGeneratorService(fileservice);
               });
           });

            // Build the host
            var host = hostBuilder.Build();
            return host;
        }
    }

}
