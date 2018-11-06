// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using DialogWithAccessorBotV4;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class WelcomeMessageWithAccessorBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;
        public WelcomeMessageWithAccessorBot(DialogBotConversationStateAndUserStateAccessor accessor)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);
            _dialogSet.Add(new TextPrompt("name"));
        }
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var didBotWelcomeUser = await _dialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState());

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Your bot should proactively send a welcome message to a personal chat the first time
                // (and only the first time) a user initiates a personal chat with your bot.
                if (didBotWelcomeUser.DidBotWelcomeUser == false)
                {
                    didBotWelcomeUser.DidBotWelcomeUser = true;
                    // Update user state flag to reflect bot handled first user interaction.
                    await _dialogBotConversationStateAndUserStateAccessor.WelcomeUserState.SetAsync(turnContext, didBotWelcomeUser);
                    await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext);

                    // the channel should sends the user name in the 'From' object
                    var userName = turnContext.Activity.From.Name;

                    await turnContext.SendActivityAsync($"You are seeing this message because this was your first message ever to this bot. Notice you will not see this message again.", cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync($"This is good place to welcome the user. For example, 'Welcome {userName}.' In your future bots -- this is also a good place to let them know what the bot can do.", cancellationToken: cancellationToken);
                }

                //    // Run the DialogSet - let the framework identify the current state of the dialog from
                //    // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                var dialogTurnResult = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (dialogTurnResult.Status == DialogTurnStatus.Empty)
                {
                    // A prompt dialog can be started directly on the DialogContext. The prompt text is given in the PromptOptions.
                    await dialogContext.PromptAsync(
                        "name",
                        new PromptOptions { Prompt = MessageFactory.Text(
                            "A DialogContext has been created (with TurnContext as an parameter) and ContinueDialog is being " +
                            "called on the DialogContext.  " +
                            "\n Each future IBot/OnTurn will do checks against the dialogTurnResult to understand where in the dialog it should resume." +
                            "\n If you are seeing this message, the dialog has just been kicked off and therefore the DialogTurnStatus is empty.  " +
                            "\n We will begin by calling DialogContext.PromptAsync: " +
                            "\n This is an example of a TextPrompt Dialog: " +
                            "\n *** Please Enter your name") },
                        cancellationToken);
                }
                // We had a dialog run (it was the prompt). Now it is Complete.
                else if (dialogTurnResult.Status == DialogTurnStatus.Complete)
                {
                    // Check for a result.
                    if (dialogTurnResult.Result != null)
                    {
                        // Finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                        await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you, I have your name as: '{dialogTurnResult.Result}'."));
                    }
                }
            }
            //Processes ConversationUpdate Activities to welcome the user.
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        string ConversationUpdateMessage = @"ActivityTypes.ConversationUpdate was triggered!";

                        string WelcomeMessage = @"This is a simple Welcome Bot sample.This bot will introduce you
                                                to welcoming and greeting users.  You are seeing this message because the bot received at least one
                                            'ConversationUpdate' event, indicating you (and possibly others including the bot) joined the conversation. 
                                            If you are using the emulator, pressing the 'Start Over' button to trigger this event again.";

                        //UNCOMMENT THIS SECTION AND THE ACTIVITY WILL NOT BE SENT WHEN THE BOT JOINS THE CHANNEL
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        //if (member.Id != turnContext.Activity.Recipient.Id) //IE. DON'T SEND THIS TO THE BOT
                        //{
                        await turnContext.SendActivityAsync($"Hi there - {member.Name}. \n {ConversationUpdateMessage}", cancellationToken: cancellationToken);
                        await turnContext.SendActivityAsync($"{WelcomeMessage} \n *** Type anything to continue.", cancellationToken: cancellationToken);
                        //}
                    }
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
            // Save the new turn count into the conversation state.
            await _dialogBotConversationStateAndUserStateAccessor.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}