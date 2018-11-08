using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using SimplifiedWaterfallDialogBotV4.BotAccessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class ColorWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "colorDialog";

        public static ColorWaterfallDialog BotInstance { get; } = new ColorWaterfallDialog(DialogId, null);


        // YOU CAN DEFINE AS ARRAY AND THEN USE WHEN CALLING DIALOG-- BUT THIS ADDS SOME USAGE COMPLEXITY
        // ADDING 'ADD STEPS' IN CONSTRUCTOR LIMITS USAGE COMPLEXITY WHEN CALLING BOT
        //public WaterfallStep[] RootDialogWaterfallSteps { get; } = new WaterfallStep[]
        //{
        //    FirstStepAsync,
        //    NameStepAsync
        //};

        public ColorWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base(dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(NameStepAsync);
            AddStep(NameConfirmStepAsync);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            //return await stepContext.PromptAsync("name", new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") }, cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            // Running a prompt here means the next WaterfallStep will be run when the users response is received.
            return await stepContext.PromptAsync("colorName", new PromptOptions { Prompt = MessageFactory.Text("What is your favorite color?") }, cancellationToken);
            //return await stepContext.NextAsync(null, cancellationToken);
            //return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            // We can send messages to the user at any point in the WaterfallStep.
            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"COLOR WATERFALL STEP 3: I like the color {stepContext.Result} too!"), cancellationToken);
            //END-WITHOUT SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'


            //WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            var botState = await (stepContext.Context.TurnState["DialogBotConversationStateAndUserStateAccessor"] as DialogBotConversationStateAndUserStateAccessor).TheUserProfile.GetAsync(stepContext.Context);
            botState.Color = stepContext.Result.ToString();

            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"FOOD WATERFALL STEP 3: I like {botState.Color} {botState.Food} as well! "), cancellationToken);
            //END-WITH SAVING STATE WITH ACCESSOR TO 'THEUSERSTATE'
            
            //// Get the current profile object from user state.
            //var userProfile = await _dialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            //// Update the profile.
            //userProfile.Name = (string)stepContext.Result;

            //await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 3: Thanks {userProfile.Name }."), cancellationToken);

            // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
            //return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("Would you like to give your age?") }, cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }

    }
}
