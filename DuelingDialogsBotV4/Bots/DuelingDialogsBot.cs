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
    public class DuelingDialogsBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;
        public DuelingDialogsBot(DialogBotConversationStateAndUserStateAccessor accessor)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);
            _dialogSet.Add(RootWaterfallDialog.BotInstance);
            _dialogSet.Add(new TextPrompt("name"));
            _dialogSet.Add(new TextPrompt("colorName"));
            _dialogSet.Add(new TextPrompt("foodName"));
            _dialogSet.Add(FoodWaterfallDialog.BotInstance);
            _dialogSet.Add(ColorWaterfallDialog.BotInstance);
            _dialogSet.Add(new ChoicePrompt("dialogChoice"));

        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                //var dialogTurnResult = await dialogContext.ContinueDialogAsync(cancellationToken);
                
                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (dialogContext.ActiveDialog == null)
                {
                    await dialogContext.BeginDialogAsync(RootWaterfallDialog.DialogId, null, cancellationToken);
                }
                else
                {
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                }

                await _dialogBotConversationStateAndUserStateAccessor.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
        }
    }
}

    //    private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
    //        // Running a prompt here means the next WaterfallStep will be run when the users response is received.
    //        //return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);

    //        return await stepContext.NextAsync("Data from First Step", cancellationToken);
    //    }

    //    /// <summary>
    //    /// One of the functions that make up the <see cref="WaterfallDialog"/>.
    //    /// </summary>
    //    /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
    //    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    //    /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
    //    private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        string stringFromFirstStep = (string)stepContext.Result;
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);

    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
    //        // Running a prompt here means the next WaterfallStep will be run when the users response is received.
    //        return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
    //        //return await stepContext.NextAsync(null, cancellationToken);
    //    }

    //    /// <summary>
    //    /// One of the functions that make up the <see cref="WaterfallDialog"/>.
    //    /// </summary>
    //    /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
    //    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    //    /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
    //    private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        // Get the current profile object from user state.
    //        var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

    //        // Update the profile.
    //        userProfile.Name = (string)stepContext.Result;

    //        // We can send messages to the user at any point in the WaterfallStep.
    //        //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 3: Thanks {stepContext.Result}."), cancellationToken);
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 3: Thanks {userProfile.Name }."), cancellationToken);


    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
    //        //return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
    //        return await stepContext.NextAsync(null, cancellationToken);
    //    }

    //    /// <summary>
    //    /// One of the functions that make up the <see cref="WaterfallDialog"/>.
    //    /// </summary>
    //    /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
    //    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    //    /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
    //    private async Task<DialogTurnResult> AgeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        //if ((bool)stepContext.Result)
    //        //{
    //        // User said "yes" so we will be prompting for the age.

    //        // Get the current profile object from user state.
    //        var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
    //        return await stepContext.PromptAsync("age", new PromptOptions { Prompt = MessageFactory.Text("WATERFALL STEP 4: Please enter your age.") }, cancellationToken);
    //        //}
    //        //else
    //        //{
    //        //    // User said "no" so we will skip the next step. Give -1 as the age.
    //        //    return await stepContext.NextAsync(-1, cancellationToken);
    //        //}
    //    }

    //    /// <summary>
    //    /// One of the functions that make up the <see cref="WaterfallDialog"/>.
    //    /// </summary>
    //    /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
    //    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    //    /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
    //    private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        // Get the current profile object from user state.
    //        var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

    //        // Update the profile.
    //        userProfile.Age = (int)stepContext.Result;

    //        // We can send messages to the user at any point in the WaterfallStep.
    //        //if (userProfile.Age == -1)
    //        //{
    //        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"No age given."), cancellationToken);
    //        //}
    //        //else
    //        //{
    //        // We can send messages to the user at any point in the WaterfallStep.
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 5: I have your age as {userProfile.Age}."), cancellationToken);
    //        //}

    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is a Prompt Dialog.
    //        //return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Is this ok?") }, cancellationToken);

    //        return await stepContext.NextAsync(null, cancellationToken);
    //    }

    //    /// <summary>
    //    /// One of the functions that make up the <see cref="WaterfallDialog"/>.
    //    /// </summary>
    //    /// <param name="stepContext">The <see cref="WaterfallStepContext"/> gives access to the executing dialog runtime.</param>
    //    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    //    /// <returns>A <see cref="DialogTurnResult"/> to communicate some flow control back to the containing WaterfallDialog.</returns>
    //    private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    //    {
    //        //if ((bool)stepContext.Result)
    //        //{
    //        // Get the current profile object from user state.
    //        var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

    //        // We can send messages to the user at any point in the WaterfallStep.
    //        //if (userProfile.Age == -1)
    //        //{
    //        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I have your name as {userProfile.Name}."), cancellationToken);
    //        //}
    //        //else
    //        //{
    //        await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 6: I have your name as {userProfile.Name} and age as {userProfile.Age}.  \n Exercise: If your name didn't come out correctly, uncomment one line of code in the solution to fix problem."), cancellationToken);
    //        //}
    //        //}
    //        //else
    //        //{
    //        //    // We can send messages to the user at any point in the WaterfallStep.
    //        //    await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thanks. Your profile will not be kept."), cancellationToken);
    //        //}

    //        // WaterfallStep always finishes with the end of the Waterfall or with another dialog, here it is the end.
    //        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
    //    }

    //}
//}