using System.Text.Json;

namespace PortfolioMicroService.Definitions.Kafka.Models;

public class EventPersonModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public override string ToString() => $"FirstName: {FirstName} LastName: {LastName}";
}