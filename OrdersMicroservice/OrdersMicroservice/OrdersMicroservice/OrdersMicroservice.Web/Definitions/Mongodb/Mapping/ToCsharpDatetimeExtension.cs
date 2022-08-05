namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public static class ToCsharpDatetimeExtension
{
    public static DateTime ToCShartDateTime(this Orders.DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute,
            dateTime.Second);
    }
}