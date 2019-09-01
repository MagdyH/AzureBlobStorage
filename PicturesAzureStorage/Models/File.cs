using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicturesAzureStorage.Models
{
    public class FileUpload
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
