using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BotMarathon.Bot.Dialogs
{
    [Serializable]
    public class QnaDialog : QnAMakerDialog
    {
        public QnaDialog() : base(
            new QnAMakerService(
            new QnAMakerAttribute(
                ConfigurationManager.AppSettings["QnaSubscriptionKey"], 
                ConfigurationManager.AppSettings["QnaKnowledgeBaseId"],
                "Não encontrei sua resposta",
                0.5)))
        {

        }

        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults result)
        {
            var primeiraResposta = result.Answers.First().Answer;

            var resposta = ((Activity)context.Activity).CreateReply();

            var dadosResposta = primeiraResposta.Split(';');

            if (dadosResposta.Length == 1)
            {
                await context.PostAsync(primeiraResposta);
                return;
            }

            var titulo = dadosResposta[0];
            var descricao = dadosResposta[1];
            var site = dadosResposta[2];
            var imagem = dadosResposta[3];

            HeroCard card = new HeroCard
            {
                Title = titulo,
                Subtitle = descricao
            };

            card.Buttons = new List<CardAction>
            {
                new CardAction(ActionTypes.OpenUrl, "Saiba mais", value:site)
            };

            card.Images = new List<CardImage>
            {
                new CardImage(url:imagem)
            };

            resposta.Attachments.Add(card.ToAttachment());

            await context.PostAsync(resposta);
        }
    }
}