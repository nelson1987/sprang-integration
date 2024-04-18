using MessagePack;

namespace Sprang.Core.Base;

[MessagePackObject]
public class SerializableEvents
{
    [Key(0)] public Guid Id { get; set; }

    [Key(1)] public required string Message { get; set; }

    public byte[] Serialize()
    {
        return MessagePackSerializer.Serialize(this);
    }
}
