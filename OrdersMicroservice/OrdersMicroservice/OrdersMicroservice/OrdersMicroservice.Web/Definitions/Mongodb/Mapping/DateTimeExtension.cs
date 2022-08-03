namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public static class DateTimeExtension
{
    public static DateTime ToCsharpDateTime(this Orders.DateTime orderDateTime)
    {
        return new DateTime(orderDateTime.Year, orderDateTime.Month, orderDateTime.Day, orderDateTime.Hour,
            orderDateTime.Minute, orderDateTime.Second);
    }
}