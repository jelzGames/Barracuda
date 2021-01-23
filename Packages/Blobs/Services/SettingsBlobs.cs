using Blobs.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blobs.Application.Services
{
    public class SettingsBlobs : ISettingsBlobs
    {
        public SettingsBlobs()
        {

            BlobKey = System.Environment.GetEnvironmentVariable("BlobKey", EnvironmentVariableTarget.Process);
            StorageAccountConnection = System.Environment.GetEnvironmentVariable("StorageAccountConnection", EnvironmentVariableTarget.Process);
            LimitBytesSizeContainerBlobs = Convert.ToInt32(System.Environment.GetEnvironmentVariable("LimitBytesSizeContainerBlobs",
                    EnvironmentVariableTarget.Process));
            AddMinutesToStartTime = Convert.ToInt32(System.Environment.GetEnvironmentVariable("AddMinutesToStartTime",
                EnvironmentVariableTarget.Process));
            AddMinutesToEndTime = Convert.ToInt32(System.Environment.GetEnvironmentVariable("AddMinutesToEndTime",
                EnvironmentVariableTarget.Process));

        }


        public string BlobKey { get; private set; }
        public string StorageAccountConnection { get; private set; }
        public int AddMinutesToStartTime { get; private set; }
        public int AddMinutesToEndTime { get; private set; }
        public int LimitBytesSizeContainerBlobs { get; private set; }
    }

}
