// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class WelcomeMessageMiddlewareEchoBot : IBot
    {
        //private const string WelcomeText = "Welcome to Custom Dialog Bot. This bot uses a custom dialog that executes a data driven flow.  Type anything to get started.";
        //private DialogSet _dialogs;

        private const string WelcomeMessage = @"Welcome to the Welcome bot.  This is the Welcome message!  This bot will introduce you
                                                to welcoming and greeting users. \n 
                                                If you are running this bot in the Bot Framework
                                                Emulator, press the 'Start Over' button to simulate user joining
                                                a bot or a channel. \n
                                                Behind the scenes, we're looking for an Activity Type of 'Conversation Update'. \n
                                                ";

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var responseMessage1 = $"help - STEP 4: Thanks for typing: {turnContext.Activity.Text}.  This message was sent to you from the Bot. \n";
                await turnContext.SendActivityAsync(responseMessage1);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        if (member.Id != turnContext.Activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync($"Hi there - {member.Name}. {WelcomeMessage}", cancellationToken: cancellationToken);
                        }
                    }
                }
            }
            else
            {
                // Default behavior for all other type of activities.
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected");
            }



        }
    }
}
