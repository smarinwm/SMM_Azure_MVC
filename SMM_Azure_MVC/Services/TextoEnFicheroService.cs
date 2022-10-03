using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Rest;
using SMM_Azure_MVC.smm.Models;
using System.Text;

namespace SMM_Azure_MVC.smm.Services
{
    public class TextoEnFicheroService : AzureBaseService<TextAnalyticsClient, TextoEnFichero>
    {
        public TextoEnFicheroService(IConfiguration configuration) : base(configuration)
        {
        }

        protected override string StorageAccountContainer => "TextoEnFicheroContainer";

        protected override TextAnalyticsClient CreateClient(ServiceClientCredentials clientCredentials) => new TextAnalyticsClient(new Uri(Endpoint), new AzureKeyCredential(Configuration["CognitiveServiceKey"]));

        protected override async Task<TextoEnFichero> GetResponse(TextAnalyticsClient client, string blobName, byte[] blobContent)
        {
            string blobText = Encoding.UTF8.GetString(blobContent);
            var textAnalysis = await client.AnalyzeSentimentAsync(blobText);
            return CreateResponse(blobName, blobText, textAnalysis);
        }

        TextoEnFichero CreateResponse(string blobName, string text, Response<DocumentSentiment>? textAnalysis) => new TextoEnFichero()
        {
            NombreFichero = blobName,
            Texto = text,
            Sentimiento = (textAnalysis?.Value.Sentiment).GetValueOrDefault().ToString(),
            PuntuacionNegativa = (textAnalysis?.Value.ConfidenceScores.Negative).GetValueOrDefault(),
            PuntuacionNeutral = (textAnalysis?.Value.ConfidenceScores.Neutral).GetValueOrDefault(),
            PuntuacionPositiva = (textAnalysis?.Value.ConfidenceScores.Positive).GetValueOrDefault(),
        };
    }
}
