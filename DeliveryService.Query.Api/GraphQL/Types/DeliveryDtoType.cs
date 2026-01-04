using DeliveryService.Query.Api.GraphQL.References;
using DeliveryService.Query.Application.DTOs.Delivery;

namespace DeliveryService.Query.Api.GraphQL.Types
{
    public  class DeliveryDtoType: ObjectType<DeliveryDto>
    {
        protected override void Configure(IObjectTypeDescriptor<DeliveryDto> descriptor)
        {
            descriptor.Ignore(x => x.CourierId);

            descriptor
                .Field("courier")
                .Resolve(ctx =>
                {
                    var delivery = ctx.Parent<DeliveryDto>();
                    return new Courier(delivery.CourierId);
                });
        }

      
    }
}
