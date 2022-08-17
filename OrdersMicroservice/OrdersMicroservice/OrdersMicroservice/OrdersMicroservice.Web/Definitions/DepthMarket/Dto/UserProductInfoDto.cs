namespace OrdersMicroservice.Definitions.DepthMarket.Dto
{
    public class UserProductInfoDto
    {
        public int Spent { get; set; }
        public int Earned { get; set; }
        public int BestAsk { get; set; }
        public int BestBid { get; set; }
        public string ProductId { get; set; } = null!;
    }
}
