using System.IO;
using System.Web;
namespace Live_Quiz.Models.Repository
{
    public class ContentRepository
    {
        private readonly DataModel db = new DataModel();
        public int UploadImageInDataBase(HttpPostedFileBase file, ImageFielView contentViewModel)
        {
            contentViewModel.Image = ConvertToBytes(file);
            var imageFile = new ImageFile()
            {

                Image = contentViewModel.Image
            };
            db.Images.Add(imageFile);
            int i = db.SaveChanges();
            if (i == 1)
            {
                return imageFile.Id;
            }
            else
            {
                return 0;
            }

        }

        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}