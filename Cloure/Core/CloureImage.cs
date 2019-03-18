using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Cloure.Core
{
    public class CloureImage
    {
        public string Name { get; set; }
        public string Title { get; set; }
        private byte[] bytes { get; set; }
        public ImageSource ImageSrc { get; set; }
        
        public CloureImage(string Name, byte[] bytes)
        {
            this.Name = Name;
            this.bytes = bytes;
        }
        

        public async Task<BitmapImage> GetBitmapImage()
        {
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0));
            writer.WriteBytes(bytes);
            await writer.StoreAsync();

            BitmapImage image = new BitmapImage();
            await image.SetSourceAsync(stream);
            return image;
        }

        public byte[] GetBytes()
        {
            return bytes;
        }

        public string ToJSONString()
        {
            string json = "";
            json += "{";
            json += "\"Name\":\""+Name+"\",";
            json += "\"Data\":\"" + Convert.ToBase64String(bytes)+"\"";
            json += "}";

            return json;
        }
    }
}
