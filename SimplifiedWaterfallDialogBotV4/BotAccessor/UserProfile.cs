using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimplifiedWaterfallDialogBotV4
{
    /// <summary>
    /// This is our application state. Just a regular serializable .NET class.
    /// </summary>
    public class UserProfile
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
