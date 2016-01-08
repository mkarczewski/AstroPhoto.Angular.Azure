using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using AP.DB;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Drawing;
using System.Drawing.Imaging;

namespace AP.PhotosCropper
{
    public class Functions
    {
        public static void CroppPhoto(
            [QueueTrigger("photosrequest")]Photo photo,
            [Blob("photos/{PhotoUid}", FileAccess.Read)] Stream bigFoto,
            CloudStorageAccount acc)
        {
            var client = acc.CreateCloudBlobClient();
            var photosRef = client.GetContainerReference("photos");

            using (var ctx = new FrontEndContext())
            {

                using (var srcImg = Image.FromStream(bigFoto))
                {
                    foreach (var item in photo.Items)
                    {
                        using (var dstImg = new Bitmap(200, 200))
                        using (var dstGrp = Graphics.FromImage(dstImg))
                        {
                            dstGrp.FillRectangle(Brushes.Black, 0, 0, 200, 200);
                            dstGrp.DrawImage(srcImg, new Rectangle(0, 0, 200, 200), new Rectangle(item.X - 100, item.Y - 100, 200, 200), GraphicsUnit.Pixel);

                            var uid = item.No + "_" + photo.PhotoUid;
                            var objPhoto = photosRef.GetBlockBlobReference(uid);
                            objPhoto.Properties.ContentType = "image/jpeg";

                            using (var ms = new MemoryStream())
                            {
                                dstImg.Save(ms, ImageFormat.Jpeg);
                                ms.Seek(0, SeekOrigin.Begin);
                                objPhoto.UploadFromStream(ms);
                            }

                            ctx.AstroObjects.Add(new AstroObject()
                            {
                                Uid = uid,
                                BigPhotoUrl = photo.PhotoUrl,
                                CroppedPhotoUrl = objPhoto.Uri.ToString(),
                                BigPhotoX = item.X,
                                BigPhotoY = item.Y,
                                Name = item.Name
                            });
                        }
                    }
                }

                ctx.SaveChanges();
            }
        }
    }
}
