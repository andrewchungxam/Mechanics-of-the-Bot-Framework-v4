// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{ 
    public class SimplifiedEchoBot : IBot
    {
       
        private const string WelcomeMessageWithAccessors = @"Welcome to the Welcome bot.  This is the Welcome message!  
        This bot will introduce you to welcoming and greeting users. If you are running this bot in the Bot Framework
        Emulator, press the 'Start Over' button to simulate user joining a bot or a channel. 
        Behind the scenes, we're looking for an Activity Type of 'Conversation Update'.";

        private const string WelcomeMessageWithoutAccessors = @"Welcome to the Welcome bot.  
        Notice that even though a local bool 'DidBotWelcomeUser' flag is flipped - it always welcomes the user.
        This is because the OnTurn is called and recreated from scratch each time an activity needs to be processed.  
        Therefore, state cannot be saved on the OnTurn ... something needs to be persist between turns.  
        This is where the BotAccessors and Conversation + User State comes into play (in future samples).
        Notice also that the Activity.ConversationUpdate does not get triggered.";

        public bool DidBotWelcomeUser = false;

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (DidBotWelcomeUser == false)
            {
                await turnContext.SendActivityAsync($"{WelcomeMessageWithoutAccessors}", cancellationToken: cancellationToken);
                DidBotWelcomeUser = true;
            }


            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var responseMessage1 = $"Welcome Message Middleware Echo Bot :: STEP 4: Thanks for typing: {turnContext.Activity.Text}.  This message was sent to you from the Bot. \n";
                await turnContext.SendActivityAsync(responseMessage1, cancellationToken: cancellationToken);
            }

            //YOU NEED AN ACCESSOR + KEEP TRACK OF CONVERSATION STATE AND BOT STATE
            //THEN THIS WILL BE TRIGGERED WHEN SOMEBODY NEW JOINS THE CONVERSATION
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        string ConversationUpdateMessage = @"ActivityTypes.ConversationUpdate was triggered!";

                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        //if (member.Id != turnContext.Activity.Recipient.Id) //IE. DON'T SEND THIS TO THE BOT
                        //{
                            await turnContext.SendActivityAsync($"Hi there - {member.Name}. \n {ConversationUpdateMessage}", cancellationToken: cancellationToken);
                        //}
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
