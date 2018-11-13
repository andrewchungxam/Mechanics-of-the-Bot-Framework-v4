using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class RootWaterfallDialog : WaterfallDialog
    {
        public static string DialogId { get; } = "rootDialog";

        public static RootWaterfallDialog BotInstance { get; } = new RootWaterfallDialog(DialogId, null);
        
        // YOU CAN DEFINE AS ARRAY AND THEN USE WHEN CALLING DIALOG-- BUT THIS ADDS SOME USAGE COMPLEXITY
        // ADDING 'ADD STEPS' IN CONSTRUCTOR LIMITS USAGE COMPLEXITY WHEN CALLING BOT
        //public WaterfallStep[] RootDialogWaterfallSteps { get; } = new WaterfallStep[]
        //{
        //    FirstStepAsync,
        //    NameStepAsync
        //};

        public RootWaterfallDialog(string dialogId, IEnumerable<WaterfallStep> steps)
            : base (dialogId, steps)
        {
            AddStep(FirstStepAsync);
            AddStep(PromptDialogChoiceStepAsync);
            AddStep(LaunchDialogStepAsync);
            AddStep(LoopDialogStepAsync);
        }

        private static async Task<DialogTurnResult> FirstStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 1: This is the first step.  You can put your code in each of these steps."), cancellationToken);
            return await stepContext.NextAsync("Data from First Step", cancellationToken);
        }

        private static async Task<DialogTurnResult> PromptDialogChoiceStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string stringFromFirstStep = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"WATERFALL STEP 2: You can pass objects/strings step-to-step like this: {stringFromFirstStep}"), cancellationToken);

            return await stepContext.PromptAsync("dialogChoice", 
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What would you like to talk about today?"),
                    Choices = new[]
                    {
                        new Choice { Value = "Favorite Food" },
                        new Choice { Value = "Favorite Color" },
                    },
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> LaunchDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var chosenDialogResponse = (stepContext.Result as FoundChoice)?.Value;

            if (chosenDialogResponse == "Favorite Food")
            {
                return await stepContext.BeginDialogAsync(FoodWaterfallDialog.DialogId);
            }

            if (chosenDialogResponse == "Favorite Color")
            {
                return await stepContext.BeginDialogAsync(ColorWaterfallDialog.DialogId);
            }

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> LoopDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await Task.Delay(3000);
            return await stepContext.ReplaceDialogAsync(RootWaterfallDialog.DialogId);
        }
    }
}
