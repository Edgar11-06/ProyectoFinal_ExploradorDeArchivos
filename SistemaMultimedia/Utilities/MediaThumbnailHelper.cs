using System.Drawing;
using OpenCvSharp;

namespace SistemaMultimedia.Utilities;

public static class MediaThumbnailHelper
{
    public static Bitmap? TryCreateVideoThumbnail(string videoPath, int width = 128, int height = 128)
    {
        try
        {
            using var capture = new VideoCapture(videoPath);
            if (!capture.IsOpened())
            {
                return CreatePlaceholder(width, height, "Video");
            }

            using var frame = new Mat();
            if (capture.Read(frame) && !frame.Empty())
            {
                var encoded = frame.ImEncode(".jpg");
                using var stream = new MemoryStream(encoded);
                return new Bitmap(stream);
            }
        }
        catch
        {
        }

        return CreatePlaceholder(width, height, "Video");
    }

    public static Bitmap CreatePlaceholder(int width, int height, string label)
    {
        var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Black);
        using var font = new Font("Segoe UI", 12, FontStyle.Bold);
        var size = graphics.MeasureString(label, font);
        graphics.DrawString(
            label,
            font,
            Brushes.White,
            (width - size.Width) / 2f,
            (height - size.Height) / 2f);
        return bitmap;
    }
}
