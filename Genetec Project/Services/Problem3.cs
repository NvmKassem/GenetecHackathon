using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Genetec_Project.Services
{
    class Problem3
    {
        public static async Task<string> UploadImage(string fileName, FileStream inputStream) {

            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=stteam02strathack;AccountKey=wcgl81nRRObzj8cN/Dvk9WsclRSnvBhbqL7BSq3kdJ/IyQnVf63f2fBqNJpYAUroVzzvyR1RNTJLmL1oN1piZg==;EndpointSuffix=core.windows.net";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

            //create a block blob 
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            //create a container 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("appcontainer");
            if (await cloudBlobContainer.CreateIfNotExistsAsync()) {

                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            }

            string imageName = "wanted" + Guid.NewGuid() + ".jpg";

            //get Blob reference

            CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);

            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}", cloudBlockBlob.Uri);
            //cloudBlockBlob.Properties.ContentType = inputStream.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(inputStream);

            return cloudBlockBlob.Uri.ToString();
            
        }
    }
}
