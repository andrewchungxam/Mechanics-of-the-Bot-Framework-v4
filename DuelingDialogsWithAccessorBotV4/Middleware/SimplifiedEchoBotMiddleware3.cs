// Licensed under the MIT License.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using SimplifiedWaterfallDialogBotV4.BotAccessor;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedEchoBotMiddleware3 : IMiddleware
    {
        private readonly IStatePropertyAccessor<string> _languageStateProperty;


        public SimplifiedEchoBotMiddleware3(IStatePropertyAccessor<string> languageStateProperty)
        {
            _languageStateProperty = languageStateProperty ?? throw new ArgumentNullException(nameof(languageStateProperty));

        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text == "name")
            {
                //await turnContext.SendActivityAsync($"STEP 3: MIDDLEWARE - BEFORE ");

                //string userLanguage = await _languageStateProperty.GetAsync(turnContext, () => TranslationSettings.DefaultLanguage) ?? TranslationSettings.DefaultLanguage;

                //var didBotWelcomeUser = await _dialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState());
                //var didBotWelcomeUser = await _dialogBotConversationStateAndUserStateAccessor.WelcomeUserState.GetAsync(turnContext, () => new WelcomeUserState());

                //set the language property to "name"

                var didBotWelcomeUser = await _languageStateProperty.GetAsync(turnContext, () => string.Empty, cancellationToken);

                //if (turnContext.Activity.Type == ActivityTypes.Message)
                //{
                    // Your bot should proactively send a welcome message to a personal chat the first time
                    // (and only the first time) a user initiates a personal chat with your bot.
                   //if (didBotWelcomeUser != nam)
                   // {
                        didBotWelcomeUser = "name";
                        // Update user state flag to reflect bot handled first user interaction.
                        await _languageStateProperty.SetAsync(turnContext, didBotWelcomeUser);
                        turnContext.TurnState.Add("didWelcomeUser", didBotWelcomeUser);


                        //turnContext.TurnState.   // .ActiveDialog == null


                //await _dialogBotConversationStateAndUserStateAccessor.UserState.SaveChangesAsync(turnContext);
                await next(cancellationToken);
                //await turnContext.SendActivityAsync($"STEP 5: MIDDLEWARE - AFTER ");
            }
            else
            {
                //await turnContext.SendActivityAsync($"STEP 3: MIDDLEWARE - BEFORE (skip was typed - notice no bot)");

                await next(cancellationToken);

                //await turnContext.SendActivityAsync($"STEP 5: MIDDLEWARE - AFTER (skip was typed - notice no bot)");

            }

        }
    }
}