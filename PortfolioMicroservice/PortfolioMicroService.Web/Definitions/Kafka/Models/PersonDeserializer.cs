using System.Text.Json;
using Confluent.Kafka;

namespace PortfolioMicroService.Definitions.Kafka.Models;

public class PersonDeserializer : IDeserializer<EventPersonModel>
{
    public EventPersonModel Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => JsonSerializer.Deserialize<EventPersonModel>(data);
}