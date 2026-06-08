namespace Procurement.Components.Model
{
    public interface IFileService
    {
        /// <summary>
        /// Uploads a PDF to Firebase Storage and returns the permanent download URL.
        /// </summary>
        Task<string> UploadPdfAsync(string storageFolder, Stream fileStream, string fileName, string userToken);

        /// <summary>
        /// Retrieves the download URL for a specific storage path.
        /// </summary>
        Task<string> OpenPdf(string storagePath);
    }
}