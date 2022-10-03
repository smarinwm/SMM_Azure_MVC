using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Rest;
using SMM_Azure_MVC.smm.Models;

namespace SMM_Azure_MVC.smm.Services
{
    public class ObjetosEnImagenService : AzureBaseService<ComputerVisionClient, ObjetosEnImagen>
    {
        public ObjetosEnImagenService(IConfiguration configuration) : base(configuration)
        {
        }

        protected override string StorageAccountContainer => "ObjetosEnImagenContainer";

        protected override ComputerVisionClient CreateClient(ServiceClientCredentials clientCredentials) => new ComputerVisionClient(clientCredentials) { Endpoint = Endpoint };

        protected override async Task<ObjetosEnImagen> GetResponse(ComputerVisionClient client, string blobName, byte[] blobContent)
        {
            using (var memStream = new MemoryStream(blobContent))
            {
                var response = await client.DetectObjectsInStreamAsync(memStream);
                return CreateResponse($"data:image/{Path.GetExtension(blobName)};base64,{Convert.ToBase64String(blobContent)}", response);
            }
        }

        ObjetosEnImagen CreateResponse(string imageUri, DetectResult response) => new ObjetosEnImagen()
        {
            UriImagen = imageUri,
            Objetos = response.Objects.Select(s => s.ObjectProperty).ToList()
        };
    }
}
