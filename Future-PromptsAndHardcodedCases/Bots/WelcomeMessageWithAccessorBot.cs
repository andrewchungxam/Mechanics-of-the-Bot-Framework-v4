﻿// Licensed under the MIT License.

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

        // The bot state accessor object. Use this to access specific state properties.
        //private readonly DialogBotUserStateAccessor _dialogBotUserStateAccessor;

        private const string WelcomeMessageWithAccessors = @"Welcome to the Welcome bot.  This is the Welcome message!  
        This bot will introduce you to welcoming and greeting users. If you are running this bot in the Bot Framework
        Emulator, press the 'Start Over' button to simulate user joining a bot or a channel. 
        Behind the scenes, we're looking for an Activity Type of 'Conversation Update'.";

        // Messages sent to the user.
        private const string WelcomeMessage = @"This is a simple Welcome Bot sample.This bot will introduce you
                                                to welcoming and greeting users. You can say 'intro' to see the
                                                introduction card. If you are running this bot in the Bot Framework
                                                Emulator, press the 'Start Over' button to simulate user joining
                                                a bot or a channel";

        private const string PatternMessage = @"It is a good pattern to use this event to send general greeting
                                              to user, explaining what your bot can do. In this example, the bot
                                              handles 'hello', 'hi', 'help' and 'intro. Try it now, type 'hi'";

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

                    await turnContext.SendActivityAsync($"You are seeing this message because this was your first message ever to this bot.", cancellationToken: cancellationToken);
                    await turnContext.SendActivityAsync($"It is a good practice to welcome the user and provide personal greeting. For example, welcome {userName}.", cancellationToken: cancellationToken);
                }
                else
                {
                    // This example hardcodes specific utterances. You should use LUIS or QnA for more advance language understanding.
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "hello":
                            await turnContext.SendActivityAsync($"Case 'hello': You said {text}.", cancellationToken: cancellationToken);
                            break;
                        case "hi":
                            await turnContext.SendActivityAsync($"Case 'hi': You said {text}.", cancellationToken: cancellationToken);
                            break;
                        case "intro":
                            await turnContext.SendActivityAsync($"Case 'intro': You said {text}.", cancellationToken: cancellationToken);
                            break;
                        case "help":
                            await turnContext.SendActivityAsync($"Case 'help': You said {text}.", cancellationToken: cancellationToken);
                            break;
                        default:
                            await turnContext.SendActivityAsync($"Case 'default': You said {text}.", cancellationToken: cancellationToken);
                            break;
                    }
                }

                //    // Run the DialogSet - let the framework identify the current state of the dialog from
                //    // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    // A prompt dialog can be started directly on the DialogContext. The prompt text is given in the PromptOptions.
                    await dialogContext.PromptAsync(
                        "name",
                        new PromptOptions { Prompt = MessageFactory.Text("STEP 4: This is the TextPrompt Dialog ::: PLEASE ENTER YOUR NAME.") },
                        cancellationToken);
                }

                // We had a dialog run (it was the prompt). Now it is Complete.
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    // Check for a result.
                    if (results.Result != null)
                    {
                        // Finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                        await turnContext.SendActivityAsync(MessageFactory.Text($"THANK YOU, I HAVE YOUR NAME AS: '{results.Result}'."));
                    }
                }
            }

            //Processes ConversationUpdate Activities to welcome the user.
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    //await SendWelcomeMessageAsync(turnContext, cancellationToken);

                    // Iterate over all new members added to the conversation
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        string ConversationUpdateMessage = @"ActivityTypes.ConversationUpdate was triggered!";

                        string InfoMessage = @"You are seeing this message because the bot received at least one
                                            'ConversationUpdate' event, indicating you (and possibly others)
                                            joined the conversation. If you are using the emulator, pressing
                                            the 'Start Over' button to trigger this event again.";

                        //UNCOMMENT THIS SECTION AND THE ACTIVITY WILL NOT BE SENT WHEN THE BOT JOINS THE CHANNEL
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        //if (member.Id != turnContext.Activity.Recipient.Id) //IE. DON'T SEND THIS TO THE BOT
                        //{
                        await turnContext.SendActivityAsync($"Hi there - {member.Name}. \n {ConversationUpdateMessage}", cancellationToken: cancellationToken);
                        await turnContext.SendActivityAsync($"{InfoMessage}", cancellationToken: cancellationToken);
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