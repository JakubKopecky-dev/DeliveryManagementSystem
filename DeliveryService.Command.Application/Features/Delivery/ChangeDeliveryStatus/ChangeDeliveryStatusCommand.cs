using DeliveryService.Command.Application.Abstraction.Massaging;
using DeliveryService.Command.Application.DTOs.Delivery;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryService.Command.Application.Features.Delivery.ChangeDeliveryStatus
{
    public sealed record ChangeDeliveryStatusCommand(Guid DeliveryId, ChangeDeliveryStatusDto ChangeDeliveryStatusDto) : ICommand<DeliveryDto?>;

}
