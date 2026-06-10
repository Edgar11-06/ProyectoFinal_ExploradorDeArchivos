using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SistemaMultimedia.Forms.ImageEditor;

internal static class ImageProcessingHelper
{
    public static Bitmap ApplyGrayscale(Image source) =>
        ApplyColorMatrix(source, GrayscaleMatrix);

    public static Bitmap ApplySepia(Image source) =>
        ApplyColorMatrix(source, SepiaMatrix);

    public static Bitmap AdjustBrightnessContrast(Image source, int brightness, int contrast)
    {
        var bitmap = new Bitmap(source.Width, source.Height);
        var contrastFactor = (100.0f + contrast) / 100.0f;
        contrastFactor *= contrastFactor;
        var sourceBitmap = (Bitmap)source;

        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var color = sourceBitmap.GetPixel(x, y);
                var red = AdjustChannel(color.R / 255f, contrastFactor, brightness);
                var green = AdjustChannel(color.G / 255f, contrastFactor, brightness);
                var blue = AdjustChannel(color.B / 255f, contrastFactor, brightness);
                bitmap.SetPixel(x, y, Color.FromArgb(color.A, red, green, blue));
            }
        }

        return bitmap;
    }

    public static Bitmap BoxBlur(Image source, int radius)
    {
        var bitmap = new Bitmap(source.Width, source.Height);
        var sourceBitmap = (Bitmap)source;

        for (var y = 0; y < source.Height; y++)
        {
            for (var x = 0; x < source.Width; x++)
            {
                var red = 0;
                var green = 0;
                var blue = 0;
                var count = 0;

                for (var yy = Math.Max(0, y - radius); yy <= Math.Min(source.Height - 1, y + radius); yy++)
                {
                    for (var xx = Math.Max(0, x - radius); xx <= Math.Min(source.Width - 1, x + radius); xx++)
                    {
                        var pixel = sourceBitmap.GetPixel(xx, yy);
                        red += pixel.R;
                        green += pixel.G;
                        blue += pixel.B;
                        count++;
                    }
                }

                bitmap.SetPixel(x, y, Color.FromArgb(255, red / count, green / count, blue / count));
            }
        }

        return bitmap;
    }

    private static int AdjustChannel(float channel, float contrastFactor, int brightness)
    {
        channel = ((channel - 0.5f) * contrastFactor) + 0.5f + (brightness / 255f);
        return (int)(Math.Min(1f, Math.Max(0f, channel)) * 255);
    }

    private static Bitmap ApplyColorMatrix(Image source, ColorMatrix matrix)
    {
        var bitmap = new Bitmap(source.Width, source.Height);
        using var graphics = Graphics.FromImage(bitmap);
        using var attributes = new ImageAttributes();
        attributes.SetColorMatrix(matrix);
        graphics.DrawImage(
            source,
            new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            0,
            0,
            source.Width,
            source.Height,
            GraphicsUnit.Pixel,
            attributes);
        return bitmap;
    }

    private static ColorMatrix GrayscaleMatrix => new(new float[][]
    {
        new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
        new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
        new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
        new float[] { 0, 0, 0, 1, 0 },
        new float[] { 0, 0, 0, 0, 1 }
    });

    private static ColorMatrix SepiaMatrix => new(new float[][]
    {
        new float[] { 0.393f, 0.349f, 0.272f, 0, 0 },
        new float[] { 0.769f, 0.686f, 0.534f, 0, 0 },
        new float[] { 0.189f, 0.168f, 0.131f, 0, 0 },
        new float[] { 0, 0, 0, 1, 0 },
        new float[] { 0, 0, 0, 0, 1 }
    });
}
