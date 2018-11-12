//// Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT License.

//using System;
//using Microsoft.Bot.Builder;

//namespace DialogWithAccessorBotV4.BotAccessor
//{
//    /// This class holds a set of accessors (to specific properties) that the bot uses to access
//    /// specific data. These are created as singleton and available via Direct Injection.
//    public class DialogBotUserStateAccessor
//    {
//        //The state object that stores the counter.</param>
//        public DialogBotUserStateAccessor(UserState userState)
//        {
//            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
//        }
        
//        /// Gets the name used for the (IStatePropertyAccessor) WelcomeUserState accessor.  Accessors require a unique name.
//        public static string WelcomeUserName { get; } = $"{nameof(DialogBotUserStateAccessor)}.WelcomeUserState";

//        /// <summary>
//        /// Gets or sets the IStatePropertyAccessor{T} for DidBotWelcome.
//        /// The accessor stores if the bot has welcomed the user or not.
//        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }

//        /// Gets the UserState object for the conversation.
//        public UserState UserState { get; }
//    }
//}
