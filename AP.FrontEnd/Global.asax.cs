using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AP.FrontEnd
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitializeStorage();
        }

        private void InitializeStorage()
        {
            var stAcc = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AstroPhotoBlobsConnection"].ConnectionString);
            var blobClient = stAcc.CreateCloudBlobClient();

            var photosContainer = blobClient.GetContainerReference("photos");
            if (photosContainer.CreateIfNotExists())
            {
                photosContainer.SetPermissions(new BlobContainerPermissions()
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            var queueClient = stAcc.CreateCloudQueueClient();
            var photosQueue = queueClient.GetQueueReference("photosrequest");
            photosQueue.CreateIfNotExists();
        }
    }
}
