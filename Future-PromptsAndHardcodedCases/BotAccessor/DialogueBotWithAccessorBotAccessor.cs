// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DialogWithAccessorBotV4
{

    /// In Startup.cs, this class is created as a Singleton and passed into the Bot's constructor.
    public class DialogueBotWithAccessorBotAccessor
    {
        // Contains the ConversationState and the IStatePropertyAccessor<DialogState> ConversationDialogState
        public DialogueBotWithAccessorBotAccessor(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        // ConversationDialogState is a type of DialogState. Under the covers, Dialog State is a serialized dialog stack.
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        // Gets the "ConversationState" for the conversation.
        public ConversationState ConversationState { get; }
    }
}

//THIS IS THE DEFINITION OF THE DIALOG STATE --> NOTICE HOW IT HAS A LIST OF A DIALOGINSTANCE
//public class DialogState
//{
//    public DialogState();
//    public DialogState(List<DialogInstance> stack);

//    public List<DialogInstance> DialogStack { get; }
//}