namespace BalanceMicroservice.Infrastructure.Kafka.Config;

public class KafkaConfig
{
    public string Topic { get; set; } = null!;
    public string KafkaHost { get; set; } = null!;
}