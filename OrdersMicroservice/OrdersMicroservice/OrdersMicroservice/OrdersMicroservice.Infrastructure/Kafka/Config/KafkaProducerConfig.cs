using System.Net;
using Confluent.Kafka;

namespace OrdersMicroservice.Infrastructure.Kafka.Config;

public class KafkaProducerConfig : KafkaConfig
{
    public ProducerConfig ProducerConfig =>
        new ProducerConfig()
        {
            BootstrapServers = KafkaHost,
            ClientId = Dns.GetHostName()
        };
}