namespace BlazorClient.Infrastructure
{
    public class PriceDefineService
    {
        public PriceDefineService()
        {

        }
        public int DefinePrice(double value)
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
        public double DefineNormalPrice(int value)
        {
            return ((double)value) / 100;
        }
    }
}
