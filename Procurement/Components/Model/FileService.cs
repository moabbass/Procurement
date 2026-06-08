using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata.Ecma335;
namespace Procurement.Components.Model
{
    public class FileService : IFileService
    {
        private readonly string _bucket;
        private readonly string _apiKey;

        public FileService(IConfiguration config)
        {
            _bucket = config["Firebase:Bucket"]!;
            _apiKey = config["Firebase:ApiKey"]!;
        }
        public FileService() { }

        public async Task<string> UploadPdfAsync(string email,Stream fileStream, string fileName,string userToken)
        {
            var task = new FirebaseStorage(
                _bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(userToken),
                    ThrowOnCancel = true
                })
                .Child(email)
                .Child(fileName)
                .PutAsync(fileStream);

            return await task;
        }

        public async Task<string> OpenPdf(string storagePath)
        {
            
            var task = new FirebaseStorage(_bucket)
                .Child(storagePath)
                .GetDownloadUrlAsync();

            var url = await task;
                        
            return url;
        }
    }
}
