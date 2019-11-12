using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MessengerClientDB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        internal static HttpClient _httpClient;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundlesConfig.RegisterBundles(BundleTable.Bundles);

            Uri baseUri = new Uri("http://localhost:56523/");
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseUri;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.FindServicePoint(baseUri).ConnectionLeaseTimeout = 60 * 1000;
        }
    }
}
