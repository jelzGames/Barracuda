using Barracuda.Indentity.Provider.Interfaces;
using System;

namespace Barracuda.Indentity.Provider.Services
{
    public class SettingsRedis : ISettingsRedis
    {
        public SettingsRedis()
        {
            RedisCacheConnection = System.Environment.GetEnvironmentVariable("RedisCacheConnection", EnvironmentVariableTarget.Process);

        }

        public string RedisCacheConnection { get; private set; }
    }
}
