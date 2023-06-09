using IdentityServer3.Core.Services.InMemory;
using System.Collections.Generic;

namespace WebServices.WcsWebApi.Common
{
    public static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {

                }
            };
        }
    }
}