using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Blobs.Domain.Models
{
    public enum BlobTypes
    {
        File = 1,
        Image = 2,
        Video = 3,
        Audio = 4
    }
}
