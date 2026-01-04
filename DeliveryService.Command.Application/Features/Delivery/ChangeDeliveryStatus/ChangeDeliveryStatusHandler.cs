using DeliveryService.Command.Application.Abstraction.Massaging;
using DeliveryService.Command.Application.DTOs.Delivery;
using DeliveryService.Command.Application.Interfaces.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Enums;
using Shared.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryService.Command.Application.Features.Delivery.ChangeDeliveryStatus
{
    public class ChangeDeliveryStatusHandler(IDeliveryRepisotry deliveryRepository, ITopicProducer<DeliveryStatusChangedEvent> producer,ILogger<ChangeDeliveryStatusHandler> logger  ) : ICommandHandler<ChangeDeliveryStatusCommand,DeliveryDto?>
    {
        private readonly IDeliveryRepisotry _deliveryRepisotry = deliveryRepository;
        private readonly ITopicProducer<DeliveryStatusChangedEvent> _producer = producer ;
        private readonly ILogger<ChangeDeliveryStatusHandler> _logger = logger;


        public async Task<DeliveryDto?> Handle(ChangeDeliveryStatusCommand command, CancellationToken ct = default)
        {
            Domain.Entities.Delivery? delivery = await _deliveryRepisotry.FindByIdAsync(command.DeliveryId, ct);
            if (delivery is null)
            {
                _logger.LogWarning("Cannot change delivery status. Delivery not found. DeliveryId={DeliveryId}", command.DeliveryId);
                return null;
            }

            delivery.UpdatedAt = DateTime.UtcNow;

            if (command.ChangeDeliveryStatusDto.Status is Domain.Enums.DeliveryStatus.Delivered)
                delivery.DeliveredAt = DateTime.UtcNow;

            Domain.Entities.Delivery updatedDelivery = await _deliveryRepisotry.UpdateAsync(delivery,ct);


            DeliveryStatusChangedEvent deliveryEvent = new(updatedDelivery.Id, (DeliveryStatus)(int)updatedDelivery.Status, updatedDelivery.UpdatedAt!.Value, updatedDelivery.Email, updatedDelivery.UpdatedAt);
            await _producer.Produce(deliveryEvent, ct);

            return updatedDelivery.DeliveryToDeliveryDto();
        }



    }
}
