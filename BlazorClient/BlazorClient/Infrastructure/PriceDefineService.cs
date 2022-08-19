namespace BlazorClient.Infrastructure
{
    public class PriceDefineService
    {
        public PriceDefineService()
        {

        }
        public async Task<int> DefinePriceAsync(double value)
        {
            var stringValue = value.ToString();
            var unitNanos = stringValue.Split(".");
            if (unitNanos.Count().Equals(1))
            {
                return Convert.ToInt32(unitNanos[0]) * 100;
            }
            else
            {
                return Convert.ToInt32(unitNanos[0]) * 100 + Convert.ToInt32(unitNanos[1]);
            }
        }
    }
}
