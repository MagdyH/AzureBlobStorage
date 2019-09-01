using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using PicturesAzureStorage.Models;

namespace PicturesAzureStorage.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    public class AzureStorageController : Controller
    {
        private CloudBlobContainer cloudBlobContainer;

        public AzureStorageController()
        {
            //calling function to initialize blob contianer
            CreatBolbClientNContainer();
        }
        /// <summary>
        /// action to upload image to container.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadBlobsToContainer([FromBody]FileUpload file)
        {
            List<IListBlobItem> listBlobItems = new List<IListBlobItem>();
            if (file != null)
            {                
                string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string localFileName = file.FileName;
                string sourceFile = Path.Combine(localPath, localFileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);

                //check if this blob is already exists.
                if (!cloudBlockBlob.Exists())
                {
                    await cloudBlockBlob.UploadFromByteArrayAsync(file.FileBytes, 0, file.FileBytes.Length);

                    var results = cloudBlobContainer.ListBlobs();
                    foreach (IListBlobItem item in results)
                    {
                        listBlobItems.Add(item);
                    }
                    return Ok(listBlobItems);
                }
                else
                {
                    return BadRequest("This bBlob is already Exists.");
                }
            }
            return BadRequest("This Blob is invalid.");
        }
        /// <summary>
        /// action to list all blobs in the container.
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IEnumerable<IListBlobItem>> GetImagesBlob()
        {
            BlobContinuationToken blobContinuationToken = null;
            List<IListBlobItem> listBlobItems = new List<IListBlobItem>();

            do
            {
                var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                // Get the value of the continuation token returned by the listing call.
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    listBlobItems.Add(item);
                }
            } while (blobContinuationToken != null); // Loop while the continuation token is not null.
            return listBlobItems;
        }
        /// <summary>
        /// action to delete blob by its name.
        /// </summary>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        [HttpDelete("{localFileName}/[action]")]
        public async Task<IActionResult> DeleteBlobsFromContainer(string localFileName)
        {
            List<IListBlobItem> listBlobItems = new List<IListBlobItem>();
            if (localFileName != null)
            {
                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(localFileName);
                await cloudBlockBlob.DeleteIfExistsAsync();

                var results = cloudBlobContainer.ListBlobs();
                foreach (IListBlobItem item in results)
                {
                    listBlobItems.Add(item);
                }
                return Ok(listBlobItems);
            }
            return BadRequest("This Blob is invalid.");
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
                // Set the permissions so the blobs are public.
                BlobContainerPermissions permissions = new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                };
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
