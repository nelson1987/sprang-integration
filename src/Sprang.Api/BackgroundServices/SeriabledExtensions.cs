using MessagePack;

namespace Sprang.Api.BackgroundServices;

public static class SeriabledExtensions
{
    public static string ConvertToJson(this byte[] obj)
    {
        return MessagePackSerializer.ConvertToJson(obj);
    }

    public static T Deserialize<T>(this byte[] obj)
    {
        return MessagePackSerializer.Deserialize<T>(obj);
    }
}
