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
        public Guid? Id { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [BsonElement("Balance")]
        public int Balance { get; set; }
    }
}