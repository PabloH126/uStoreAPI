using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using System.Security.Cryptography;

namespace uStoreAPI.Services
{
    public class UploadService
    {
        private readonly BlobServiceClient blobServiceClient;
        public UploadService(BlobServiceClient _blobServiceClient)
        {
            blobServiceClient = _blobServiceClient;
        }

        public async Task<string> UploadImageAdmin(IFormFile image, string imageFileName = null!)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("admins");
            var finalImageName = string.IsNullOrEmpty(imageFileName) ? image.FileName : imageFileName;
            var blobClient = containerClient.GetBlobClient(finalImageName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadImagePlazas(IFormFile image, string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("plazas");

            var blobName = $"{directorio}/{imageName}";
            var blobClient = containerClient.GetBlobClient(blobName);
            
            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadImageTiendas(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("tiendas");

            var blobName = $"{imageName}.png";
            
            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = image.OpenReadStream();
            
            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                }
            };

            await blobClient.UploadAsync(stream , blobUploadOptions);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadImageProductos(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("productos");

            var blobName = $"{imageName}.png";

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = image.ContentType
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task DeleteImageTiendas(string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("tiendas");

            var blobName = $"{directorio}/{imageName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteImagenesTiendas(string directorio)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("tiendas");

            var blobs = containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, directorio);
            await foreach (var blob in blobs) 
            {
                var blobClient = containerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        public async Task DeleteImagenesProductos(string directorio)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("productos");

            var blobs = containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, directorio);
            await foreach (var blob in blobs)
            {
                var blobClient = containerClient.GetBlobClient(blob.Name);
                await blobClient.DeleteIfExistsAsync();
            }
        }

        public async Task DeleteImageAdmins(string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("admins");

            var blobName = $"{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteImagesPlazas(string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("plazas");

            var blobName = $"{directorio}/{imageName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<int> CountBlobs(string container,string directorio)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient($"{container}");
            var blobs = containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, directorio);
            int counter = 0;
            await foreach (var blob in blobs)
            {
                counter++;
            }

            return counter;
        }

        public string GetBlobNameFromUrl(string? url)
        {
            if(string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            Uri uri = new Uri(url);
            string blobName = uri.Segments.Last();
            string blobNameNoExt = Path.GetFileNameWithoutExtension(blobName);
            return blobNameNoExt;
        }

        private string GetKeyImage()
        {
            byte[] secretKey = new byte[4];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(secretKey);
            }

            return Convert.ToBase64String(secretKey);
        }
    }
}
