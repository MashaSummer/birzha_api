namespace ProductMicroservice.Definitions.Mongodb.ViewModels;

public class ProductViewModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null;
    public double BestAsk { get; set; }
    public double BestBid { get; set; }
}