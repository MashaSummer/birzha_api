using Confluent.Kafka;
using Google.Protobuf;

namespace PortfolioMicroService.Infrastructure.Kafka.Protobuf;

public class ProtobufSerializer<T> : ISerializer<T> where T : IMessage<T>, new() 
{
    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}