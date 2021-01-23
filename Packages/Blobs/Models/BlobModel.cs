using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blobs.Domain.Models
{
    public class BlobModel : BaseModel
    {
        public string Container { get; set; }
        public string BlobName { get; set; }
        public string BlobType { get; set; }
        public string Filename { get; set; }
        public string FileExtension { get; set; }
        public string MimeType { get; set; }
        public string fileType { get; set; }
        public string fileSize { get; set; }
    }
}
