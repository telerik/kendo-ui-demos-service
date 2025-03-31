using SkiaSharp;

namespace KendoCRUDService.FileBrowser
{
    public class ThumbnailCreator
    {
        private static readonly IDictionary<string, SKEncodedImageFormat> ImageFormats = new Dictionary<string, SKEncodedImageFormat>{
            {"image/png", SKEncodedImageFormat.Png},
            {"image/gif", SKEncodedImageFormat.Gif},
            {"image/jpeg", SKEncodedImageFormat.Jpeg}
        };

        private readonly ImageResizer resizer;

        public ThumbnailCreator()
        {
            this.resizer = new ImageResizer();
        }

        public byte[] Create(byte[] source, ImageSize desiredSize, string contentType)
        {
            using (var image = SKBitmap.Decode(source))
            {               
                var originalSize = new ImageSize
                {
                    Height = image.Height,
                    Width = image.Width
                };

                var size = resizer.Resize(originalSize, desiredSize);
                var thumbnail = ScaleImage(image, size);

                return thumbnail.Encode(ImageFormats[contentType], 100).ToArray();
            }
        }

        public byte[] CreateFill(Stream source, ImageSize desiredSize, string contentType)
        {
            using (var image = SKBitmap.Decode(source))
            {
                using (var memoryStream = new MemoryStream())
                {
                    return FixedSize(image, desiredSize.Width, desiredSize.Height, true).Encode(ImageFormats[contentType], 100).ToArray();
                }
            }
        }

        private SKBitmap ScaleImage(SKBitmap source, ImageSize size)
        {
            return source.Resize(new SKImageInfo(size.Width, size.Height), SKFilterQuality.High);
        }


        private SKBitmap FixedSize(SKBitmap imgPhoto, int Width, int Height, bool needToFill)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (!needToFill)
            {
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                }
                else
                {
                    nPercent = nPercentW;
                }
            }
            else
            {
                if (nPercentH > nPercentW)
                {
                    nPercent = nPercentH;
                    destX = (int)Math.Round((Width -
                        (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = (int)Math.Round((Height -
                        (sourceHeight * nPercent)) / 2);
                }
            }

            if (nPercent > 1)
                nPercent = 1;

            int destWidth = (int)Math.Round(sourceWidth * nPercent);
            int destHeight = (int)Math.Round(sourceHeight * nPercent);

            var bmPhoto = new SKBitmap(
                destWidth <= Width ? destWidth : Width,
                destHeight < Height ? destHeight : Height);

            var canvas = new SKCanvas(bmPhoto);

            canvas.Clear(new SKColor(255, 255, 255));
            canvas.DrawBitmap(imgPhoto,
                SKRect.Create(sourceX, sourceY, sourceWidth, sourceHeight),
                SKRect.Create(destX, destY, destWidth, destHeight));

            canvas.Flush();

            return bmPhoto;
        }
    }
}
