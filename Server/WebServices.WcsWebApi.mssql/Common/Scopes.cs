using IdentityServer3.Core.Models;
using System.Collections.Generic;

namespace WebServices.WcsWebApi.Common
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
                {
                        new Scope
                        {
                                Name = "wcsApi",
                                Enabled=true,
                                Type = ScopeType.Resource,
                        }
                };

            scopes.AddRange(StandardScopes.All);

            return scopes;
        }
    }
}