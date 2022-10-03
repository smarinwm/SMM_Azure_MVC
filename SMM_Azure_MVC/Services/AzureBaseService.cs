using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Rest;

namespace SMM_Azure_MVC.smm.Services
{
    public abstract class AzureBaseService<TClient, TResult> : IAzureService<TResult>
    {
        static string ID_DIRECTIVA_ACCESO_LECTURA = "DirectivaLecturaBlobs";
        public IConfiguration Configuration { get; set; }
        protected BlobContainerClient Container;
        protected abstract string StorageAccountContainer { get; }
        protected string Endpoint => Configuration["CognitiveServiceEndpoint"];
        public AzureBaseService(IConfiguration configuration)
        {
            Configuration = configuration;
            Container = new BlobContainerClient(
                configuration.GetValue<string>("StorageAccount:ConnectionString"),
                configuration.GetValue<string>($"StorageAccount:{StorageAccountContainer}")
            );
        }

        async Task EstablecerDirectivaDeAccesoLectura() => await Container.SetAccessPolicyAsync(PublicAccessType.Blob, permissions: new BlobSignedIdentifier[] { CrearDirectivaDeLectura() });

        BlobSignedIdentifier CrearDirectivaDeLectura() => new BlobSignedIdentifier()
        {
            Id = ID_DIRECTIVA_ACCESO_LECTURA,
            AccessPolicy = new BlobAccessPolicy
            {
                PolicyExpiresOn = DateTimeOffset.UtcNow.AddHours(1),
                Permissions = "r"
            }
        };

        ServiceClientCredentials CreateClientCredentials() => new ApiKeyServiceClientCredentials(Configuration["CognitiveServiceKey"]);

        protected abstract TClient CreateClient(ServiceClientCredentials clientCredentials);

        public async Task<IEnumerable<TResult>> GetResult()
        {
            //await EstablecerDirectivaDeAccesoLectura();
            var results = new List<TResult>();
            var client = CreateClient(CreateClientCredentials());
            await foreach (BlobItem blobItem in Container.GetBlobsAsync())
            {
                BlobClient blob = Container.GetBlobClient(blobItem.Name);
                try
                {
                    results.Add(await GetResponse(client, blob));
                    GC.Collect();
                }
                catch { }
            }
            DisposeClient(client);
            return results;
        }

        protected string GetBlobSasUri(BlobClient blob)
        {
            BlobSasBuilder sas = new BlobSasBuilder
            {
                BlobContainerName = blob.BlobContainerName,
                BlobName = blob.Name,
                Resource = "b",
                //Identifier = ID_DIRECTIVA_ACCESO_LECTURA
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sas.SetPermissions(BlobAccountSasPermissions.Read);
            return blob.GenerateSasUri(sas).ToString();
        }

        async Task<TResult> GetResponse(TClient client, BlobClient blob)
        {
            using (var memStream = new MemoryStream())
            {
                await blob.DownloadToAsync(memStream);
                return await GetResponse(client, blob.Name, memStream.ToArray());
            }
        }

        protected string ToBase64ImageSrc(string imageName, byte[] imageContent) => $"data:image/{Path.GetExtension(imageName)};base64,{Convert.ToBase64String(imageContent)}";

        protected abstract Task<TResult> GetResponse(TClient client, string blobName, byte[] blobContent);

        public double ObtenerPromedio(IEnumerable<double> valores)
        {
            if (valores.Count() == 0)
            {
                return 0;
            }
            return valores.Average();
        }

        void DisposeClient(TClient client)
        {
            if (!(client is IDisposable disposable))
            {
                return;
            }
            disposable.Dispose();
        }
    }
}
