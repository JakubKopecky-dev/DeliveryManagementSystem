using CourierService.Domain.Enums;

namespace CourierService.Api.GraphQL.Lookups
{
    [GraphQLName("Courier")]
    public class CourierGraph
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "";

        public string Email { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public CourierStatus Status { get; set; }

        public int OrderDelivered { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
