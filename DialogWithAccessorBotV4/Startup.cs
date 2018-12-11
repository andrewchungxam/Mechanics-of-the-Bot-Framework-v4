// Licensed under the MIT License.

using System;
using System.Linq;
using DialogWithAccessorBotV4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class Startup
    {
        private bool _isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<DialogueBotWithAccessor>(options =>
            {
                var secretKey = Configuration.GetSection("botFileSecret")?.Value;
                var botFilePath = Configuration.GetSection("botFilePath")?.Value;

                // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
                var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
                services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

                // Retrieve current endpoint.
                var environment = _isProduction ? "production" : "development";
                var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
                }
                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                options.Middleware.Add(new SimplifiedEchoBotMiddleware1());
                options.Middleware.Add(new SimplifiedEchoBotMiddleware2());
                options.Middleware.Add(new SimplifiedEchoBotMiddleware3());

                // Memory Storage is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.  This is where you'd want to use Cosmos or Blob Storage to have persistance beyond application's life.
                IStorage dataStore = new MemoryStorage();

                // For production bots use the Azure Blob or
                // Azure CosmosDB storage providers. For the Azure
                // based storage providers, add the Microsoft.Bot.Builder.Azure
                // Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/

                //Look at CosmosDB storage setup here:
                //https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-v4-storage?view=azure-bot-service-4.0&tabs=csharp#using-cosmos-db

                // Uncomment the following lines to use Azure Blob Storage
                // //Storage configuration name or ID from the .bot file.
                // const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
                // var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
                // if (!(blobConfig is BlobStorageService blobStorageConfig))
                // {
                //    throw new InvalidOperationException($"The .bot file does not contain an blob storage with name '{StorageConfigurationId}'.");
                // }
                // // Default container name.
                // const string DefaultBotContainer = "<DEFAULT-CONTAINER>";
                // var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
                // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);


                // Create and add conversation state.
                // The Conversation State object is where we persist anything at the conversation-scope.

                //Each state management object automates the reading and writing of associated state information to storage.
                //The storage layer connects to the backing storage for state, such as in-memory(for testing), or Azure Cosmos DB Storage(for production).
            
                var conversationState = new ConversationState(dataStore);
                options.State.Add(conversationState);
            });

            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton(sp =>
            {
                // We need to grab the conversationState we added on the options in the previous step.
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                // The dialogs will need a state store accessor. Creating it here once (on-demand) allows the dependency injection
                // to hand it to our IBot class that is create per-request.
                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new DialogueBotConversationStateAccessor(conversationState)
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                };
                return accessors;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
