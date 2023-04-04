namespace PAKN.WebAPI.Extensions;

public static class StreamExtensions
{
    public static byte[] ReadAllBytes(this Stream instream)
    {
        if (instream is MemoryStream)
            return ((MemoryStream)instream).ToArray();

        using (var memoryStream = new MemoryStream())
        {
            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }

    public static void WriteFile<T>(T obj, string path)
    {
        File.WriteAllBytes(path, obj as byte[] ?? Array.Empty<byte>());
    }
}