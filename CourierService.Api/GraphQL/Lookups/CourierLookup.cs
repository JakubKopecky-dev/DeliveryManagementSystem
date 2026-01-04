using CourierService.Application.DTOs.Courier;
using CourierService.Application.Interfaces.Repositories;
using CourierService.Application.Interfaces.Services;
using CourierService.Domain.Entities;
using HotChocolate.Fusion.SourceSchema;
using HotChocolate.Fusion.SourceSchema.Types;

namespace CourierService.Api.GraphQL.Lookups
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public static class CourierLookup
    {
        [Lookup]
        [Internal]
        public static async Task<CourierGraph?> CourierById(Guid id, [Service] ICourierService courierService, CancellationToken ct)
        {
            CourierDto? courier = await courierService.GetCourierByIdAsync(id, ct);

            if(courier is null)
                return null;

            return new()
            {
                Id = courier.Id,
                Email = courier.Email,
                CreatedAt = courier.CreatedAt,
                UpdatedAt = courier.UpdatedAt,
                Latitude = courier.Latitude,
                Longitude = courier.Longitude,
                Name = courier.Name,
                OrderDelivered = courier.OrderDelivered,
                Status = courier.Status,
                PhoneNumber = courier.PhoneNumber,
            };
        }




    }
}
