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
    public class DuelingDialogsWithAccessorsBot : IBot
    {
        private readonly DialogSet _dialogSet;

        //THE MAIN IDEA OF THE CHANGE IS HERE -- THE LOCAL FIELD BECOMES A PUBLIC PROPERTY
        //ALSO DETAILS ARE PASSED AROUND AND SAVED IN THE TURNCONTEXT'S TURNSTATE
        //private readonly DialogBotConversationStateAndUserStateAccessor _dialogBotConversationStateAndUserStateAccessor;
        public DialogBotConversationStateAndUserStateAccessor DialogBotConversationStateAndUserStateAccessor { get; set; }

        public DuelingDialogsWithAccessorsBot(DialogBotConversationStateAndUserStateAccessor accessor)
        {
            //_dialogBotConversationStateAndUserStateAccessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            //_dialogSet = new DialogSet(_dialogBotConversationStateAndUserStateAccessor.ConversationDialogState);

            _dialogSet = new DialogSet(accessor.ConversationDialogState);
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
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            var botState = await DialogBotConversationStateAndUserStateAccessor.TheUserProfile.GetAsync(turnContext, () => new UserProfile(), cancellationToken);
            turnContext.TurnState.Add("DialogBotConversationStateAndUserStateAccessor", DialogBotConversationStateAndUserStateAccessor);

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                if (dialogContext.ActiveDialog == null)
                {
                    await dialogContext.BeginDialogAsync(RootWaterfallDialog.DialogId, null, cancellationToken);
                }
                else
                {
                    await dialogContext.ContinueDialogAsync(cancellationToken);
                }

                await DialogBotConversationStateAndUserStateAccessor.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
                await DialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
        }
    }
}