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
    public class MultiDialogWithAccessorBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;

        public DialogBotConversationStateAndUserStateAccessor DialogBotConversationStateAndUserStateAccessor { get; set; }

        public MultiDialogWithAccessorBot(DialogBotConversationStateAndUserStateAccessor accessor)
        {
            _dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            _dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);
            _dialogSet.Add(RootWaterfallDialog.BotInstance);
            _dialogSet.Add(new TextPrompt("name"));
            _dialogSet.Add(new TextPrompt("colorName"));
            _dialogSet.Add(new TextPrompt("foodName"));
            _dialogSet.Add(FoodWaterfallDialog.BotInstance);
            _dialogSet.Add(ColorWaterfallDialog.BotInstance);
            _dialogSet.Add(NameWaterfallDialog.BotInstance);
            _dialogSet.Add(new ChoicePrompt("dialogChoice"));
            DialogBotConversationStateAndUserStateAccessor = accessor;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var botState = await DialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(turnContext, () => new UserProfile(), cancellationToken);
            turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessor", DialogBotConversationStateAndUserStateAccessor);

            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);

                //POP OFF ANY DIALOG IF THE "FLAG IS SWITCHED" 
                string welcomeState = "";
                if (turnContext.TurnState.ContainsKey("didWelcomeUser"))
                {
                    welcomeState = turnContext.TurnState["didWelcomeUser"] as string;
                }

                if (welcomeState == "name")
                {

                    //OPTION 1:
                    await dialogContext.CancelAllDialogsAsync();

                    //OPTION 2: //TRY BELOW OPTIONS - WHY DOES IT MISBEHAVE?
                    //NOTE-CALLING THIS HITS THE CONTINUE IN THE BELOW IF STATEMENT
                    //await dialogContext.ReplaceDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);

                    //OPTION 3:
                    //DOES NOT WORK WELL HERE - WHEN HAVE YOU SEEN IT WORK CORRECTLY IN PREVIOUS PROJECTS?
                    //await dialogContext.EndDialogAsync();
                }
                
                if (dialogContext.ActiveDialog == null)
                {
                    if (turnContext.TurnState.ContainsKey("didWelcomeUser"))
                    {
                        welcomeState = turnContext.TurnState["didWelcomeUser"] as string;
                        if (welcomeState == "name")
                        {
                            await dialogContext.BeginDialogAsync(NameWaterfallDialog.DialogId, null, cancellationToken);
                        }
                    }
                    else
                    {
                        await dialogContext.BeginDialogAsync(RootWaterfallDialog.DialogId, null, cancellationToken);
                    }
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