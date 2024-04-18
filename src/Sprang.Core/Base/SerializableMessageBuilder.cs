using System.Text.Json;

namespace Sprang.Core.Base;

public static class SerializableMessageBuilder
{
    public static SerializableEvents Init<T>(T @event) where T : class
    {
        return new SerializableEvents
        {
            Id = Guid.NewGuid(),
            Message = JsonSerializer.Serialize(@event)
        };
    }
}
