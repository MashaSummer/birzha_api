using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System.Globalization;

namespace AuthMicroservice.Web.Definitions.MongoIdentity
{
    public class DictionaryCultureInfoSerializer : IBsonSerializer<IReadOnlyDictionary<CultureInfo, string>>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args,
            IReadOnlyDictionary<CultureInfo, string> value)
        {
            context.Writer.WriteStartDocument();
            foreach (var (key, s) in value)
            {
                context.Writer.WriteString(key.Name, s);
            }

            context.Writer.WriteEndDocument();
        }

        public IReadOnlyDictionary<CultureInfo, string> Deserialize(BsonDeserializationContext context,
            BsonDeserializationArgs args)
        {
            var dictionary = new Dictionary<CultureInfo, string>();

            context.Reader.ReadStartDocument();
            while (context.Reader.ReadBsonType() != BsonType.EndOfDocument && context.Reader.State != BsonReaderState.EndOfDocument)
            {
                var key = context.Reader.ReadName();
                var value = context.Reader.ReadString();

                var cultureInfo = CultureInfo.GetCultureInfo(key);
                dictionary.Add(cultureInfo, value);
            }
            context.Reader.ReadEndDocument();

            return dictionary;
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, (IReadOnlyDictionary<CultureInfo, string>)value);
        }

        public Type ValueType => typeof(IReadOnlyDictionary<CultureInfo, string>);
    }
}