using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Connector;


namespace Microsoft.Bot.Sample.QnABot
{
    [Serializable]
    public class RootDialog : IDialog<Object> //RootDialog : QnAMakerDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            /* Wait until the first message is received from the conversation and call MessageReceviedAsync 
            *  to process that message. */
            context.Wait(this.MessageReceivedAsync);
        }
/*
        public RootDialog() : base(new QnAMakerService(new QnAMakerAttribute(RootDialog.GetSetting("QnAAuthKey"), Utils.GetAppSetting("QnAKnowledgebaseId"), "this - No good match in FAQ.", 0.5)))
        {

        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {

            var answer = result.Answers.First().Answer;
            Activity reply = ((Activity)context.Activity).CreateReply();

            string[] qnaAnswerData = answer.Split(';');
            int dataSize = qnaAnswerData.Length;

            // image or video
            if (dataSize > 1 && dataSize <= 6)
            {

                var attachment = GetSelectedCard(answer);
                reply.Attachments.Add(attachment);
                await context.PostAsync(reply);

            }
            else
            {

                await context.Forward(new BasicQnAMakerDialog(), AfterAnswerAsync, message, CancellationToken.None);
                await context.PostAsync(reply);

            }

        }


        private static Attachment GetSelectedCard(string answer)
        {

            int len = answer.Split(';').Length;


            switch (len)
            {

                case 4: return GetHeroCard(answer);
                //case 6: return GetVideoCard(answer); 
                default: return GetHeroCard(answer);

            }
            return GetHeroCard(answer);
        }

        /// <summary>
        /// Hero Card or Image Card
        /// </summary>
        /// <param name = "answer"></param>
        /// <returns></returns>

        private static Attachment GetHeroCard(string answer)
        {

            string[] qnaAnswerData = answer.Split(';');
            string title = qnaAnswerData[0];
            string description = qnaAnswerData[1];
            string url = qnaAnswerData[2];
            string imageURL = qnaAnswerData[3];

            HeroCard card = new HeroCard
            {
                Title = title,
                Subtitle = description,
            };

            card.Buttons = new List<CardAction>{
                new CardAction(ActionTypes.OpenUrl,"Learn More", value: url)
            };

            card.Images = new List<CardImage>{
            new CardImage(url = imageURL)
            };

            return card.ToAttachment();

        }

*/

        private new async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            /* When MessageReceivedAsync is called, it's passed an IAwaitable<IMessageActivity>. To get the message,
             *  await the result. */
            var message = await result;

            var qnaAuthKey = GetSetting("QnAAuthKey");
            var qnaKBId = Utils.GetAppSetting("QnAKnowledgebaseId");
            var endpointHostName = Utils.GetAppSetting("QnAEndpointHostName");

            // QnA Subscription Key and KnowledgeBase Id null verification
            if (!string.IsNullOrEmpty(qnaAuthKey) && !string.IsNullOrEmpty(qnaKBId))
            {
                // Forward to the appropriate Dialog based on whether the endpoint hostname is present
                if (string.IsNullOrEmpty(endpointHostName))
                    await context.Forward(new BasicQnAMakerPreviewDialog(), AfterAnswerAsync, message, CancellationToken.None);
                else
                    await context.Forward(new BasicQnAMakerDialog(), AfterAnswerAsync, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Please set QnAKnowledgebaseId, QnAAuthKey and QnAEndpointHostName (if applicable) in App Settings. Learn how to get them at https://aka.ms/qnaabssetup.");
            }

        }


        private async Task AfterAnswerAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            // wait for the next user message
            context.Wait(MessageReceivedAsync);
        }

        public static string GetSetting(string key)
        {
            var value = Utils.GetAppSetting(key);
            if (String.IsNullOrEmpty(value) && key == "QnAAuthKey")
            {
                value = Utils.GetAppSetting("QnASubscriptionKey"); // QnASubscriptionKey for backward compatibility with QnAMaker (Preview)
            }
            return value;
        }
    }

    // Dialog for QnAMaker Preview service
    [Serializable]
    public class BasicQnAMakerPreviewDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        // Parameters to QnAMakerService are:
        // Required: subscriptionKey, knowledgebaseId, 
        // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
        public BasicQnAMakerPreviewDialog() : base(new QnAMakerService(new QnAMakerAttribute(RootDialog.GetSetting("QnAAuthKey"), Utils.GetAppSetting("QnAKnowledgebaseId"), "this2 - No good match in FAQ.", 0.5)))
        { }
    }

    // Dialog for QnAMaker GA service
    [Serializable]
    public class BasicQnAMakerDialog : QnAMakerDialog
    {
        // Go to https://qnamaker.ai and feed data, train & publish your QnA Knowledgebase.
        // Parameters to QnAMakerService are:
        // Required: qnaAuthKey, knowledgebaseId, endpointHostName
        // Optional: defaultMessage, scoreThreshold[Range 0.0 – 1.0]
        public BasicQnAMakerDialog() : base(new QnAMakerService(new QnAMakerAttribute(RootDialog.GetSetting("QnAAuthKey"), Utils.GetAppSetting("QnAKnowledgebaseId"), "this3-No good match in FAQ.", 0.5, 1, Utils.GetAppSetting("QnAEndpointHostName"))))
        { }

    }
}




/* From 10min
         private static Attachment GetVideoCard(string answer){
            string[] qnaAnswerData = answer.Split(';');
            string title = qnaAnswerData[0];
             string subtitle= qnaAnswerData[1];
            string url = qnaAnswerData[2];
            string imageURL= qnaAnswerData[3];

            HeroCard card = new HeroCard{
            Title = title,
            Subtitle = description,
            };

            card.Buttons = new List<CardAction>{
                new CardAction(ActionTypes.OpenUrl,"Learn More", value: url)
            };

            card.Images = new List<CardImage>{
            new CardImage (url = imageURL)
            };

            retrun card.ToAttachement();
        }
*/
