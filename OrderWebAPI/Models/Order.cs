using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderWebAPI.Models
{
    [Serializable, BsonIgnoreExtraElements]
    public class Order
    {
        [BsonId, BsonElement("_Id"), BsonRepresentation(BsonType.ObjectId)]
        public string OrderId { get; set; }
        [BsonElement("customer_id"), BsonRepresentation(BsonType.Int32)]
        public int CustomerId { get; set; }
        [BsonElement("ordered_on"), BsonRepresentation(BsonType.DateTime)]
        public DateTime OrderedOn { get; set; }
        [BsonElement("order_details")]
        public List<OrderDetail> OrderDetails { get; set; }

    }
}
