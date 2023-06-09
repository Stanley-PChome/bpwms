using IdentityServer3.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServices.WebApi.Common
{
	public static class Clients
	{
		public static IEnumerable<Client> Get()
		{
			return new[]
			{
					new Client
					{
						ClientName = "BPWMS API Client (service communication)",
						//連線ID
						ClientId = "BPApiService",
						Flow = Flows.ClientCredentials,
						//Token 可用時間(600秒=10分鐘)
						AccessTokenLifetime = 600,
						//如果不使用憑證可以取消註解下面這行程式但密碼比較短
						//AccessTokenType = AccessTokenType.Reference,

						ClientSecrets = new List<Secret>
										{
							          //類似連線密碼
												new Secret("bpwmsapi".Sha256())
										},
						AllowedScopes = new List<string>
										{
												"bpApi"
										}
					}
			};

		}
	}
}