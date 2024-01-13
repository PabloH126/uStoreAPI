using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace uStoreAPI.Services
{
    public class UploadService
    {
        private readonly BlobServiceClient blobServiceClient;
        public UploadService(BlobServiceClient _blobServiceClient)
        {
            blobServiceClient = _blobServiceClient;
        }

        public async Task<string[]> UploadImageAdmin(IFormFile image, string imageFileName = null!)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("admins");
            var finalImageName = string.IsNullOrEmpty(imageFileName) ? image.FileName : imageFileName;
            var blobClient = containerClient.GetBlobClient(finalImageName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            var thumbNailName = $"{finalImageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }
           

            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri};
        }

        public async Task<string[]> UploadImageGerente(IFormFile image, string imageFileName = null!)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("gerentes");
            var finalImageName = string.IsNullOrEmpty(imageFileName) ? image.FileName : imageFileName;
            var blobClient = containerClient.GetBlobClient(finalImageName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            var thumbNailName = $"{finalImageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }


            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri };
        }

        public async Task<string[]> UploadImageUser(IFormFile image, string imageFileName = null!)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("users");
            var finalImageName = string.IsNullOrEmpty(imageFileName) ? image.FileName : imageFileName;
            var blobClient = containerClient.GetBlobClient(finalImageName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);
            var thumbNailName = $"{finalImageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }


            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri };
        }

        public async Task<string[]> UploadImagePlazas(IFormFile image, string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("plazas");

            var blobName = $"{directorio}/{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            
            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            var thumbNailName = $"{directorio}/{imageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }


            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri };
        }

        public async Task<string[]> UploadImageTiendas(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("tiendas");

            var blobName = $"{imageName}.png";
            
            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = image.OpenReadStream();
            
            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream , blobUploadOptions);

            var thumbNailName = $"{imageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }


            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri };
        }

        public async Task<string[]> UploadImageProductos(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("productos");

            var blobName = $"{imageName}.png";

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await blobClient.UploadAsync(stream, blobUploadOptions);

            var thumbNailName = $"{imageName}thumb.png";
            var thumbNailBlobClient = containerClient.GetBlobClient(thumbNailName);

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(100, 100)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await thumbNailBlobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }


            return new string[] { blobClient.Uri.AbsoluteUri, thumbNailBlobClient.Uri.AbsoluteUri };
        }

        public async Task<string> UploadImagePublicacion(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("publicaciones");

            var blobName = $"{imageName}.png";

            var blobClient = containerClient.GetBlobClient(blobName);

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(400, 400)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await blobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadImageMensaje(IFormFile image, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("mensajes");

            var blobName = $"{imageName}.png";

            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = image.OpenReadStream();

            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/png"
                }
            };

            await using (var thumbNailStream = new MemoryStream())
            {
                await using (var imageStream = image.OpenReadStream())
                {
                    using var thumbNail = await Image.LoadAsync(imageStream);
                    thumbNail.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(450, 450)
                    }));

                    await thumbNail.SaveAsPngAsync(thumbNailStream);
                }

                thumbNailStream.Seek(0, SeekOrigin.Begin);
                await blobClient.UploadAsync(thumbNailStream, blobUploadOptions);
            }

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task DeleteImageTiendas(string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("tiendas");

            var blobName = $"{directorio}/{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            var blobNameThumb = $"{directorio}/{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
            
        }

        public async Task DeleteImageProducto(string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("productos");

            var blobName = $"{directorio}/{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            var blobNameThumb = $"{directorio}/{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
        }

        public async Task DeleteImagePublicacion(string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("publicaciones");

            var blobName = $"{imageName}.png";
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

            var blobNameThumb = $"{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
        }

        public async Task DeleteImageGerentes(string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("gerentes");

            var blobName = $"{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            var blobNameThumb = $"{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
        }

        public async Task DeleteImageUser(string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("users");

            var blobName = $"{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            var blobNameThumb = $"{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
        }

        public async Task DeleteImagesPlazas(string directorio, string imageName)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("plazas");

            var blobName = $"{directorio}/{imageName}.png";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            var blobNameThumb = $"{directorio}/{imageName}thumb.png";
            var blobClientThumb = containerClient.GetBlobClient(blobNameThumb);
            await blobClientThumb.DeleteIfExistsAsync();
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
