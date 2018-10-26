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
                var responseMessage1 = $"You sent '{turnContext.Activity.Text}.' \n  The above was called from OnTurnAsync. \n  ";
                await turnContext.SendActivityAsync(responseMessage1);
            }
        }
    }
}
