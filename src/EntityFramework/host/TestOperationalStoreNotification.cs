﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer8.EntityFramework;
using IdentityServer8.EntityFramework.Entities;

namespace IdentityServerHost
{
    public class TestOperationalStoreNotification : IOperationalStoreNotification
    {
        public TestOperationalStoreNotification()
        {
            Console.WriteLine("ctor");
        }

        public Task PersistedGrantsRemovedAsync(IEnumerable<PersistedGrant> persistedGrants)
        {
            foreach (var grant in persistedGrants)
            {
                Console.WriteLine("cleaned: " + grant.Type);
            }
            return Task.CompletedTask;
        }

        public Task DeviceCodesRemovedAsync(IEnumerable<DeviceFlowCodes> deviceCodes)
        {
            foreach (var deviceCode in deviceCodes) 
            {
                Console.WriteLine("cleaned device code");
            }
            return Task.CompletedTask;
        }
    }
}
