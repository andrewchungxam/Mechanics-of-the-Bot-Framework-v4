// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using SimplifiedWaterfallDialogBotV4;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedWaterfallDialogBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;
        public SimplifiedWaterfallDialogBot(DialogBotConversationStateAndUserStateAccessor accessor)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);


            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                FirstStepAsync,
                NameStepAsync,
                NameConfirmStepAsync,
                AgeStepAsync,
                ConfirmStepAsync,
                SummaryStepAsync,
            };

            _dialogSet.Add(new WaterfallDialog("details", waterfallSteps));
            _dialogSet.Add(new TextPrompt("name"));
            _dialogSet.Add(new NumberPrompt<int>("age"));
            _dialogSet.Add(new ConfirmPrompt("confirm"));
            //_dialogSet.Add(new TextPrompt("name"));
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

                    //    // the channel should sends the user name in the 'From' object
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
                    if (dialogTurnResult.Status == DialogTurnStatus.Empty)
                    {
                        await dialogContext.BeginDialogAsync("details", null, cancellationToken);
                    }
                }
                //// We had a dialog run (it was the prompt). Now it is Complete.
                //else if (dialogTurnResult.Status == DialogTurnStatus.Complete)
                //{
                //    // Check for a result.
                //    if (dialogTurnResult.Result != null)
                //    {
                //        // Finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                //        await turnContext.SendActivityAsync(MessageFactory.Text($"Thank you, I have your name as: '{dialogTurnResult.Result}'."));
                //    }
                //}
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

            //Determine if needed 
            //await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);

            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            //return await stepContext.NextAsync(null, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Name = (string)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 3: Thanks {stepContext.Result}."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 3: Thanks {userProfile.Name }."), cancellationToken);

        
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            //return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //if ((bool)stepContext.Result)
            //{
                // User said "yes" so we will be prompting for the age.

                // Get the current profile object from user state.
                var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
                return await stepContext.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("WATERFALL STEP 4: Please enter your age.") }, cancellationToken);
            //}
            //else
            //{
            //    // User said "no" so we will skip the next step. Give -1 as the age.
            //    return await stepContext.NextAsync(-1, cancellationToken);
            //}
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Get the current profile object from user state.
            var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            // Update the profile.
            userProfile.Age = (int)stepContext.Result;

            // We can send messages to the user at any point in the WaterfallStep.
            //if (userProfile.Age == -1)
            //{
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"No age given."), cancellationToken);
            //}
            //else
            //{
                // We can send messages to the user at any point in the WaterfallStep.
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 5: I have your age as {userProfile.Age}."), cancellationToken);
            //}

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
            //return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        /// <summary>
        /// One of the functions that make up the <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //if ((bool)stepContext.Result)
            //{
                // Get the current profile object from user state.
                var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                // We can send messages to the user at any point in the WaterfallStep.
                //if (userProfile.Age == -1)
                //{
                //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name}."), cancellationToken);
                //}
                //else
                //{
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 6: I have your name as {userProfile.Name} and age as {userProfile.Age}.  \n Exercise: If your name didn't come out correctly, uncomment one line of code in the solution to fix problem."), cancellationToken);
                //}
            //}
            //else
            //{
            //    // We can send messages to the user at any point in the WaterfallStep.
            //    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks. Your profile will not be kept."), cancellationToken);
            //}

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

    }
}