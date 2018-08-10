using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.QnABot
{ // comments to commit tests
	[LuisModel(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))]
	[Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
		//var luisAppId = GetSetting("LuisAppId"); 
    	//var luisApiKey= Utils.GetAppSetting("LuisAPIKey");
	
		//[LuisIntent("")]
		[LuisIntent("None")]
		public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result){
			var messageToForward = await message as Activity;
			await context.Forward(new RootDialog(), this.AfterQnA, messageToForward, CancellationToken.None);
		}
		
		private async Task AfterQnA(IDialogContext context, IAwaitable<IMessageActivity> result){
			context.Wait(this.MessageReceived);			
		}
	
	
	}