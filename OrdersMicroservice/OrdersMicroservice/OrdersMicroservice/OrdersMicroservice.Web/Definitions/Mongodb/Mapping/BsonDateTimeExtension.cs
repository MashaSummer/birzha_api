using MongoDB.Bson;

namespace OrdersMicroservice.Definitions.Mongodb.Mapping;

public static class BsonDateTimeExtension
{
    public static BsonDateTime ToBsonDateTime(this Orders.DateTime orderDateTime)
    {
        var datetime = new DateTime(orderDateTime.Year, orderDateTime.Month, orderDateTime.Day, orderDateTime.Hour,
            orderDateTime.Minute, orderDateTime.Second);
        return new BsonDateTime(datetime);
    }
}