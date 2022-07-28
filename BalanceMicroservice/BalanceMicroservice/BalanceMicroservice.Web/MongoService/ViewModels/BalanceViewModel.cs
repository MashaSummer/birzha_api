using MongoDB.Bson;

using MongoDB.Bson.Serialization.Attributes;

namespace BalanceMicroservice.Web.MongoService.ViewModels
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
        public Guid? Id { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [BsonElement("Balance")]
        public double Balance { get; set; }
    }
}