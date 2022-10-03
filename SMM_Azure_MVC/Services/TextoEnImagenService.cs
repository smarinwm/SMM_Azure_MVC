using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
using SMM_Azure_MVC.smm.Models;

namespace SMM_Azure_MVC.smm.Services
{
    public class TextoEnImagenService : AzureBaseService<ComputerVisionClient, TextoEnImagen>
    {
        const int NUMBER_OF_CHARS_IN_OPERATION_ID = 36;
        public TextoEnImagenService(IConfiguration configuration) : base(configuration)
        {
        }

        protected override string StorageAccountContainer => "TextoEnImagenContainer";

        protected override ComputerVisionClient CreateClient(ServiceClientCredentials clientCredentials) => new ComputerVisionClient(clientCredentials)
        { Endpoint = Endpoint };

        protected override async Task<TextoEnImagen> GetResponse(ComputerVisionClient client, string blobName, byte[] blobContent)
        {
            using (var memStream = new MemoryStream(blobContent))
            {
                var response = await client.ReadInStreamAsync(memStream);
                var results = await ObtenerRespuestasDeOperacionDeLectura(client, response);
                var textUrlFileResults = results.AnalyzeResult.ReadResults;
                return CreateResponse(ToBase64ImageSrc(blobName, blobContent), textUrlFileResults);
            }
        }

        TextoEnImagen CreateResponse(string imageUri, IList<ReadResult> textUrlFileResults) => new TextoEnImagen()
        {
            UriImagen = imageUri,
            TextosEncontrados = ObtenerLineasDeTextoDeRespuesta(textUrlFileResults)
        };

        private static List<string> ObtenerLineasDeTextoDeRespuesta(IList<ReadResult> textUrlFileResults)
        {
            var textResult = new List<string>();
            textUrlFileResults.ToList().ForEach(f => textResult.AddRange(f.Lines.Select(l => l.Text)));
            return textResult;
        }

        private static async Task<ReadOperationResult> ObtenerRespuestasDeOperacionDeLectura(ComputerVisionClient client, ReadInStreamHeaders response)
        {
            string operationLocation = response.OperationLocation;
            string operationId = operationLocation.Substring(operationLocation.Length - NUMBER_OF_CHARS_IN_OPERATION_ID);
            ReadOperationResult results;
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running || results.Status == OperationStatusCodes.NotStarted));
            return results;
        }
    }
}
