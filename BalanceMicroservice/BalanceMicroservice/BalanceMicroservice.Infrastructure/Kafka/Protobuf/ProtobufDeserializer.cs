using Confluent.Kafka;
using Google.Protobuf;

namespace BalanceMicroservice.Infrastructure.Kafka.Protobuf;

public class ProtobufDeserializer<T> : IDeserializer<T> where T : IMessage<T>, new()
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
        {
            return default(T);
        }

        var parser = new MessageParser<T>((Func<T>)(() => new T()));

        return parser.ParseFrom(data);
    }
}