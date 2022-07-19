using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BalanceMicroservice.Web.Endpoints.ProfileEndpoints.ViewModels
{
    /// <summary>
    /// Application User Profile
    /// </summary>
    public class BalanceViewModel
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public Guid Id { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [BsonElement("Balance")]
        public int Balance { get; set; }
    }
}