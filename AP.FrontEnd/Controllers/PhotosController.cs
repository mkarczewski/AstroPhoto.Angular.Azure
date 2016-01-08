using AP.DB;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AP.FrontEnd.Controllers
{
    public class PhotosController : Controller
    {
        private CloudBlobContainer _photosContainer;
        private CloudQueue _photosQueue;

        public PhotosController()
            : base()
        {
            var stAcc = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AstroPhotoBlobsConnection"].ConnectionString);
            var blobClient = stAcc.CreateCloudBlobClient();
            _photosContainer = blobClient.GetContainerReference("photos");

            var queueClient = stAcc.CreateCloudQueueClient();
            _photosQueue = queueClient.GetQueueReference("photosrequest");
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListObjects(string phrase)
        {
            using (var ctx = new FrontEndContext())
            {
                var objs = ctx.AstroObjects
                    .Where(x => String.IsNullOrEmpty(phrase) || x.Name.Contains(phrase))
                    .OrderBy(x => x.BigPhotoUrl)
                    .ThenBy(x => x.CroppedPhotoUrl)
                    .ToArray();
                return Json(objs, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<JsonResult> UploadPhoto(HttpPostedFileBase file)
        {
            var name = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blob = _photosContainer.GetBlockBlobReference(name);
            blob.Properties.ContentType = "image/jpeg";

            using (var fs = file.InputStream)
            {
                await blob.UploadFromStreamAsync(fs);
            }

            return Json(new { name = name, url = blob.Uri.ToString() });
        }

        [HttpPost]
        public async Task<JsonResult> Submit([Bind]Photo photo)
        {
            await _photosQueue.AddMessageAsync(new CloudQueueMessage(JsonConvert.SerializeObject(photo)));

            return Json(new { });
        }
    }
}