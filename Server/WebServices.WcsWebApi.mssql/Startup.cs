using Microsoft.Owin;
using Owin;
using System.Web.Http;
using IdentityServer3.AccessTokenValidation;
using System.Configuration;
using IdentityServer3.Core.Configuration;
using WebServices.WcsWebApi.Common;
using System;
using System.Security.Cryptography.X509Certificates;

[assembly: OwinStartup(typeof(WebServices.WcsWebApi.Startup))]

namespace WebServices.WcsWebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {


            // token validation
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                ValidationMode = ValidationMode.ValidationEndpoint,
                Authority = ConfigurationManager.AppSettings["AuthorityUrl"],
                RequiredScopes = new[] { "wcsApi" },
            });



            // web api configuration
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            app.UseIdentityServer(new IdentityServerOptions
            {
                SiteName = "BPWMS IdentityServer",
                RequireSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["RequireSsl"].ToString()),
                SigningCertificate = LoadCertificate(),
                Factory = new IdentityServerServiceFactory()
                .UseInMemoryUsers(Users.Get())
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get()),

            });
        }
        X509Certificate2 LoadCertificate()
        {
            //產生新憑證方式請參考以下網址
            //http://blogs.uuu.com.tw/Articles/post/2013/02/06/%E4%BD%BF%E7%94%A8X509%E6%86%91%E8%AD%89%E5%8A%A0%E8%A7%A3%E5%AF%86.aspx
            return new X509Certificate2(
                    string.Format(@"{0}\bin\bpwmsapi.pfx", AppDomain.CurrentDomain.BaseDirectory), "bankpro");
        }
    }
}
