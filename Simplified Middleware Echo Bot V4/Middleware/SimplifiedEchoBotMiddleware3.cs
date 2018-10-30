// Licensed under the MIT License.
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplifiedEchoBotMiddleware3 : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text != "skip")
            {
                await turnContext.SendActivityAsync($"STEP 3: MIDDLEWARE - BEFORE ");

                await next(cancellationToken);

                await turnContext.SendActivityAsync($"STEP 5: MIDDLEWARE - AFTER ");

            }
            else
            {
                await turnContext.SendActivityAsync($"STEP 3: MIDDLEWARE - BEFORE (skip was typed)");

                await turnContext.SendActivityAsync($"STEP 5: MIDDLEWARE - AFTER (skip was typed)");

            }

        }
    }
}