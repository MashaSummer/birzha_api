using System.Net;
using Confluent.Kafka;

namespace ProductMicroservice.Infrastructure.Kafka.Config;

public class KafkaProducerConfig : KafkaConfig
{
    public Acks AcksSetting { get; set; }
    public ProducerConfig ProducerConfig =>
        new ProducerConfig()
        {
            BootstrapServers = KafkaHost,
            ClientId = Dns.GetHostName(),
            Acks = AcksSetting
        };
}
