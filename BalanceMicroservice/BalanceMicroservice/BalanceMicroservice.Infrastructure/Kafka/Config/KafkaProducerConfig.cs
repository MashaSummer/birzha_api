using Confluent.Kafka;
using System.Net;

namespace BalanceMicroservice.Infrastructure.Kafka.Config;

public class KafkaProducerConfig : KafkaConfig
{
    public ProducerConfig ProducerConfig =>
        new ProducerConfig()
        {
            BootstrapServers = KafkaHost,
            ClientId = Dns.GetHostName()
        };
}