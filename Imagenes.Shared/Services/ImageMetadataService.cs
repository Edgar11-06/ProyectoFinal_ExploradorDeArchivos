using System.Drawing;

namespace Imagenes.Shared.Services;

public static class ImageMetadataService
{
    public static Bitmap CloneWithMetadata(Image source)
    {
        var clone = new Bitmap(source);
        CopyMetadata(source, clone);
        return clone;
    }

    public static void CopyMetadata(Image source, Image destination)
    {
        foreach (var propertyId in source.PropertyIdList)
        {
            try
            {
                destination.SetPropertyItem(source.GetPropertyItem(propertyId));
            }
            catch
            {
            }
        }
    }
}
