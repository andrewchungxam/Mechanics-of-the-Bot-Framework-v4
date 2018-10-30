// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{ 
    public class SimplifiedEchoBot : IBot
    {
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var responseMessage1 = $"STEP 4: Thanks for typing: {turnContext.Activity.Text}.  This message was sent to you from the Bot. \n";
                await turnContext.SendActivityAsync(responseMessage1);
            }
        }
    }
}
