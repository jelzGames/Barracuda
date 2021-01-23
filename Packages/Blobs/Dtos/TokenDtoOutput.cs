using System;
using System.Collections.Generic;
using System.Text;

namespace Blobs.Application.Dtos
{
    public class TokenDtoOutput
    {
        public string Container { get; set; }
        public string BlobName { get; set; }
        public string Permission { get; set; }
        public string Token { get; set; }
        public string Uri { get; set; }
    }
}
