using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class WeatherDialog : WaterfallDialog
    {
        public WeatherDialog(string dialogId, IEnumerable<WaterfallStep> steps = null) : base(dialogId, steps)
        {

             

        }

        public static string Id => "waterfallDialog";

        public static WaterfallDialog Instance { get; } = new WaterfallDialog(Id);

    }
}
