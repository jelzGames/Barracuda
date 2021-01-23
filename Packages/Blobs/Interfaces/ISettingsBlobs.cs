using System;
using System.Collections.Generic;
using System.Text;

namespace Blobs.Application.Interfaces
{
    public interface ISettingsBlobs
    {
        string BlobKey { get; }
        string StorageAccountConnection { get; }
        int AddMinutesToStartTime { get; }
        int AddMinutesToEndTime { get; }
        int LimitBytesSizeContainerBlobs { get; }
    }
}
