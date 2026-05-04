using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

//of this allows us to upload and connect to Azure storage explorer

namespace MovieBookingAppDemo.Services
{

   

    public class BlobService
    {
        private readonly string _connectionString;
        //constructor

        public BlobService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("BlobStorage"); //Gets the connection string from the appsettings.json --> into construcotr --> connect to Azure storage constructor

        }
        public async Task<string> UploadFileAsync(IFormFile file, string containerName) //uploads file to local blob storage and returns the image url
        {
            if (file == null || file.Length == 0)
                return null;

            //Creates a connection to the local blob storage
            var blobServiceClient = new BlobServiceClient(_connectionString);

            //access the container where files (i.e images) will be stored
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            //if the continer doesn't exist, create it:
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob); // PublicAccessType.Blob allows the browser to display image

            //gets file extension (.jpg, .png, .jpeg etc)
            var extension = Path.GetExtension(file.FileName);

            //creates a unique name for each file using GUID (avoids duplication)
            var blobName = $"{Guid.NewGuid()}{extension}";

            //Get a reference to the blob (file) inside the container
            var blobClient = containerClient.GetBlobClient(blobName);

            // Open the file stream and upload it to the blob storage
            using (var stream = file.OpenReadStream())      //Stream: super long link thingy that appears in the db 9is a picture in the web app when it runs though)
            {
                await blobClient.UploadAsync(stream, overwrite: true);

            }

            //Return the URL of the uploading file
            //Url is saved in the database (as string)
            return blobClient.Uri.ToString();




        }



    }
}
