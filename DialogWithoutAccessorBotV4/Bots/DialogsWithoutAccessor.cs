// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Bot_Builder_Simplified_Echo_Bot_V4
{
    public class SimplePromptBotAccessors
    {
        // Conversation state is of type DialogState. Under the covers this is a serialized dialog stack.
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        //THIS IS THE DEFINITION OF THE DIALOG STATE --> NOTICE HOW IT HAS A LIST OF A DIALOGINSTANCE
        //public class DialogState
        //{
        //    public DialogState();
        //    public DialogState(List<DialogInstance> stack);

        //    public List<DialogInstance> DialogStack { get; }
        //}
    }

    public class DialogsWithoutAccessorBot : IBot
    {
        private readonly DialogSet _dialogSet;
        private const string WelcomeMessageWithAccessors = @"Welcome to the Welcome bot.  This is the Welcome message!  
        This bot will introduce you to welcoming and greeting users. If you are running this bot in the Bot Framework
        Emulator, press the 'Start Over' button to simulate user joining a bot or a channel. 
        Behind the scenes, we're looking for an Activity Type of 'Conversation Update'.";

        private const string WelcomeMessageWithoutAccessors = @"Welcome to the Welcome bot.  
        Notice that even though a local bool 'DidBotWelcomeUser' flag is flipped - it always welcomes the user.
        This is because the OnTurn is called and recreated from scratch each time an activity needs to be processed.  
        Therefore, state cannot be saved on the OnTurn ... something needs to be persist between turns.  
        This is where the BotAccessors and Conversation + User State comes into play (in future samples).
        Notice also that the Activity.ConversationUpdate does not get triggered.";

        public bool DidBotWelcomeUser = false;
        public DialogsWithoutAccessorBot()
        {

            //THIS SHOULD NOT LIVE HERE --> THIS SHOULD BE IN THE STARTUP.CS
            //THIS IS ONLY CREATED HERE TO DEMO A DIALOG SET WITHOUT INTRODUCING ACCESSORS
            //SIMPLIFIED ACCESSOR - NEED THIS TO CREATE A DIALOG SET 
            //THE DIALOG SET NEEDS IT BECAUSE IT WANTS TO KEEP TRACK OF THE DIALOG SET 
            //YOU CANNOT CREATE A DIALOG SET WITHOUT THE ACCESSOR

            // Memory Storage is for local bot debugging only. When the bot
            // is restarted, everything stored in memory will be gone.
            IStorage dataStore = new MemoryStorage();

            // Create Conversation State object.
            // The Conversation State object is where we persist anything at the conversation-scope.
            var conversationState = new ConversationState(dataStore);
       
            var simplePromptBotAccessor = new SimplePromptBotAccessors();
            simplePromptBotAccessor.ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState");
            var conversationDialogState = simplePromptBotAccessor.ConversationDialogState;

            _dialogSet = new DialogSet(conversationDialogState);
            _dialogSet.Add(new TextPrompt("dialogName"));
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Run the DialogSet - let the framework identify the current state of the dialog from
                // the dialog stack and figure out what (if any) is the active dialog.
                var dialogContext = await _dialogSet.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                // If the DialogTurnStatus is Empty we should start a new dialog.
                if (results.Status == DialogTurnStatus.Empty)
                {
                    // A prompt dialog can be started directly on the DialogContext. The prompt text is given in the PromptOptions.
                    await dialogContext.PromptAsync(
                        "dialogName",
                        new PromptOptions { Prompt = MessageFactory.Text("STEP 4: (This is the TextPrompt Dialog.  Notice how this repeats.  See explaination at bottom of DialogsWithoutAccessor.cs) --> PLEASE ENTER YOUR NAME.") },
                        cancellationToken);
                }

                // We had a dialog run (it was the prompt). Now it is Complete.
                else if (results.Status == DialogTurnStatus.Complete)
                {
                    // Check for a result.
                    if (results.Result != null)
                    {
                        // Finish by sending a message to the user. Next time ContinueAsync is called it will return DialogTurnStatus.Empty.
                        await turnContext.SendActivityAsync(MessageFactory.Text($"THANK YOU, I HAVE YOUR NAME AS: '{results.Result}'."));
                    }
                }
            }
        }
        //public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    if (DidBotWelcomeUser == false)
        //    {
        //        await turnContext.SendActivityAsync($"{WelcomeMessageWithoutAccessors}", cancellationToken: cancellationToken);


        //        DidBotWelcomeUser = true;
        //    }
        //    if (turnContext.Activity.Type == ActivityTypes.Message)
        //    {
        //        var responseMessage1 = $"Welcome Message Middleware Echo Bot :: STEP 4: Thanks for typing: {turnContext.Activity.Text}.  This message was sent to you from the Bot. \n";
        //        await turnContext.SendActivityAsync(responseMessage1, cancellationToken: cancellationToken);
        //    }

        //    //YOU NEED AN ACCESSOR + KEEP TRACK OF CONVERSATION STATE AND BOT STATE
        //    //THEN THIS WILL BE TRIGGERED WHEN SOMEBODY NEW JOINS THE CONVERSATION
        //    if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
        //    {
        //        if (turnContext.Activity.MembersAdded != null)
        //        {
        //            // Iterate over all new members added to the conversation
        //            foreach (var member in turnContext.Activity.MembersAdded)
        //            {
        //                // Greet anyone that was not the target (recipient) of this message
        //                // the 'bot' is the recipient for events from the channel,
        //                // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
        //                // bot was added to the conversation.
        //                if (member.Id != turnContext.Activity.Recipient.Id)
        //                {
        //                    await turnContext.SendActivityAsync($"Hi there - {member.Name}. {WelcomeMessageWithoutAccessors}", cancellationToken: cancellationToken);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // Default behavior for all other type of activities.
        //        await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected");
        //    }
        //}
    }
}

// THE DIALOG IS REPEATING . . . EXPLANATION
// NOTICE THAT IS REPEATING AT THIS POINT --> 
//                 if (results.Status == DialogTurnStatus.Empty)
// AND IT NEVER HITS THIS POINT
//                 else if (results.Status == DialogTurnStatus.Complete)
//
// THE REASON THIS IS HAPPENING LIES IN THE BELOW EXPLAINATION.
// Represents a bot that processes incoming activities.
// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
// This is a Transient lifetime service.  Transient lifetime services are created
// each time they're requested. For each Activity received, a new instance of this
// class is created. Objects that are expensive to construct, or have a lifetime
// beyond the single turn, should be carefully managed.
// 
//
//IN ORDER TO CREATE THE DIALOG SET --> IT NEEDED A CONVERSATIONAL DIALOG STATE (WHICH KEEPS TRACK OF THE ORDER STACK OF DIALOGS)
//IN ORDER TO CREATE A DIALOG STATE --> WE NEEDED A CONVERSATION STATE (WHICH PERSISTS ANYTHING AT THE CONVERSATION LEVEL) //FROM THE CONVERSATION STATE --> WE CREATED A PROPERTY OF TYPE DIALOG STATE)
//IN ORDER TO CREATE A CONVERSATION STATE - WE NEEDED AN OBJECT OF TYPE ISTORAGE

//Relevant code is here:
//IStorage dataStore = new MemoryStorage();
//// Create Conversation State object.
//// The Conversation State object is where we persist anything at the conversation-scope.
//var conversationState = new ConversationState(dataStore);

//var simplePromptBotAccessor = new SimplePromptBotAccessors();
//simplePromptBotAccessor.ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState");
//            var conversationDialogState = simplePromptBotAccessor.ConversationDialogState;
//_dialogSet = new DialogSet(conversationDialogState);
//_dialogSet.Add(new TextPrompt("dialogName"));

//NOW ^ ALL OF THE ABOVE WAS CREATED IN SIMPLIFIED FORM SO THAT YOU COULD SEE HOW IT WAS CREATED
//HOWEVER, IT MUST LIVE SOMEWHERE THAT CAN PERSIST AT THE PROGRAM LEVEL AND NOT AT THE TURN-LEVEL
//THE REASON THIS IS NOT PROGRESSING IS BECAUSE ALL THE ABOVE CODE LIVES AT THE TURN-LEVEL 
//AND DISAPPEARS FROM TURN TO TURN MAKING THE DATA DISAPPEAR BETWEEN EACH TURN --> 
// HENCE IT NEVER GETS PAST HERE --> if (results.Status == DialogTurnStatus.Empty)
// IN OUR NEXT ITERATION, WE'LL PUT THE ABOVE CODE IN THE STARTUP.CS 

// WE'LL ADD A SINGLETON THAT THE BOT CAN ACCESS THROUGH DEPENDENCY INJECTION
// wE'LL CREATE IT THERE ONCE AND THE IBOT CLASS CAN ACCESS IT PER REQUEST