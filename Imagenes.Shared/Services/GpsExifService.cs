using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using Imagenes.Shared.Models;

namespace Imagenes.Shared.Services;

public static class GpsExifService
{
    private const int GpsVersionId = 0x0000;
    private const int GpsLatitudeRef = 0x0001;
    private const int GpsLatitude = 0x0002;
    private const int GpsLongitudeRef = 0x0003;
    private const int GpsLongitude = 0x0004;

    public static GpsCoordinates? TryReadCoordinates(Image image, string? filePath = null)
    {
        var fromImage = TryReadCoordinatesFromImage(image);
        if (fromImage.HasValue)
        {
            return fromImage;
        }

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return null;
        }

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var fileImage = Image.FromStream(stream, useEmbeddedColorManagement: false, validateImageData: false);
            return TryReadCoordinatesFromImage(fileImage);
        }
        catch
        {
            return null;
        }
    }

    private static GpsCoordinates? TryReadCoordinatesFromImage(Image image)
    {
        try
        {
            if (!image.PropertyIdList.Contains(GpsLatitude) ||
                !image.PropertyIdList.Contains(GpsLongitude))
            {
                return null;
            }

            var latRef = image.PropertyIdList.Contains(GpsLatitudeRef)
                ? GetAsciiString(image.GetPropertyItem(GpsLatitudeRef))
                : "N";
            var lonRef = image.PropertyIdList.Contains(GpsLongitudeRef)
                ? GetAsciiString(image.GetPropertyItem(GpsLongitudeRef))
                : "E";

            var latitude = ConvertToDegrees(image.GetPropertyItem(GpsLatitude).Value);
            var longitude = ConvertToDegrees(image.GetPropertyItem(GpsLongitude).Value);

            if (latRef.Equals("S", StringComparison.OrdinalIgnoreCase)) latitude = -latitude;
            if (lonRef.Equals("W", StringComparison.OrdinalIgnoreCase)) longitude = -longitude;

            return new GpsCoordinates(latitude, longitude);
        }
        catch
        {
            return null;
        }
    }

    public static void WriteCoordinates(Image image, double latitude, double longitude)
    {
        var latRef = latitude < 0 ? "S" : "N";
        var lonRef = longitude < 0 ? "W" : "E";
        var latAbs = Math.Abs(latitude);
        var lonAbs = Math.Abs(longitude);

        var properties = new[]
        {
            CreateProperty(GpsVersionId, 1, new byte[] { 2, 3, 0, 0 }),
            CreateProperty(GpsLatitudeRef, 2, Encoding.ASCII.GetBytes(latRef + '\0')),
            CreateProperty(GpsLatitude, 5, ToRationalBytes(latAbs)),
            CreateProperty(GpsLongitudeRef, 2, Encoding.ASCII.GetBytes(lonRef + '\0')),
            CreateProperty(GpsLongitude, 5, ToRationalBytes(lonAbs))
        };

        foreach (var property in properties)
        {
            image.SetPropertyItem(property);
        }
    }

    private static PropertyItem CreateProperty(int id, short type, byte[] value)
    {
        var property = CreatePropertyItem();
        property.Id = id;
        property.Type = type;
        property.Value = value;
        property.Len = value.Length;
        return property;
    }

    private static PropertyItem CreatePropertyItem()
    {
        var ctor = typeof(PropertyItem).GetConstructor(
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            Type.EmptyTypes,
            null);

        if (ctor != null)
        {
            return (PropertyItem)ctor.Invoke(null);
        }

        return (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true)!;
    }

    private static string GetAsciiString(PropertyItem property)
    {
        return Encoding.ASCII.GetString(property.Value).Trim('\0');
    }

    private static double ConvertToDegrees(byte[] value)
    {
        var degrees = ToRational(value, 0);
        var minutes = ToRational(value, 8);
        var seconds = ToRational(value, 16);
        return degrees + (minutes / 60.0) + (seconds / 3600.0);
    }

    private static double ToRational(byte[] bytes, int index)
    {
        var numerator = BitConverter.ToUInt32(bytes, index);
        var denominator = BitConverter.ToUInt32(bytes, index + 4);
        return denominator == 0 ? 0 : (double)numerator / denominator;
    }

    private static byte[] ToRationalBytes(double value)
    {
        var degrees = (int)Math.Floor(value);
        var minutesFull = (value - degrees) * 60.0;
        var minutes = (int)Math.Floor(minutesFull);
        var seconds = (minutesFull - minutes) * 60.0;

        var degNum = (uint)degrees;
        var degDen = 1u;
        var minNum = (uint)minutes;
        var minDen = 1u;
        var secDen = 10000u;
        var secNum = (uint)Math.Round(seconds * secDen);

        var bytes = new byte[24];
        Buffer.BlockCopy(BitConverter.GetBytes(degNum), 0, bytes, 0, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(degDen), 0, bytes, 4, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(minNum), 0, bytes, 8, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(minDen), 0, bytes, 12, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(secNum), 0, bytes, 16, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(secDen), 0, bytes, 20, 4);
        return bytes;
    }
}
