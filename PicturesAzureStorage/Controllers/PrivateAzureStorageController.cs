using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesAzureStorage.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class PrivateAzureStorageController : Controller
    {
        private CloudBlobContainer cloudBlobContainer;

        public PrivateAzureStorageController()
        {
            //calling function to initialize blob contianer
            CreatBolbClientNContainer();
        }
        /// <summary>
        /// action to delete all blobs when you're have valid account.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteAllBlobsFromContainer()
        {
            var results = cloudBlobContainer.ListBlobs();
            foreach (IListBlobItem item in results)
            {
                ((CloudBlockBlob)item).DeleteIfExists();
            }

            return Ok();
        }
        /// <summary>
        /// creation of the container object by its name and connection string
        /// </summary>
        /// <returns></returns>
        public CloudBlobContainer CreatBolbClientNContainer()
        {
            string storageConnectionString = Environment.GetEnvironmentVariable("CONNECT_STR");

            // Check whether the connection string can be parsed.
            CloudStorageAccount storageAccount;
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                // If the connection string is valid, proceed with operations against Blob
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                cloudBlobContainer = cloudBlobClient.GetContainerReference("images");
                cloudBlobContainer.CreateIfNotExists();
                // Set the permissions so the blobs are Private.               
                BlobContainerPermissions permissions = cloudBlobContainer.GetPermissions();
                //set shared access pploicy to allow delete.
                permissions.SharedAccessPolicies.Add(
                    "deleteAllpolicy", new SharedAccessBlobPolicy()
                    {
                        Permissions = SharedAccessBlobPermissions.Delete
                    });
                // no one access or do any operation on blob as it set to private.
                permissions.PublicAccess = BlobContainerPublicAccessType.Off;
                cloudBlobContainer.SetPermissionsAsync(permissions);

                return cloudBlobContainer;
            }
            else
            {
                return cloudBlobContainer;
            }
        }

    }
}
