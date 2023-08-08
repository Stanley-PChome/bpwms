using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServices.PdaWebApi.Common
{
	public static class Scopes
	{
		public static IEnumerable<Scope> Get()
		{
			var scopes = new List<Scope>
				{
						new Scope
						{
								Name = "wmsPdaApi",
								Enabled=true,
								Type = ScopeType.Resource,
						}
				};

			scopes.AddRange(StandardScopes.All);

			return scopes;
		}
	}
}