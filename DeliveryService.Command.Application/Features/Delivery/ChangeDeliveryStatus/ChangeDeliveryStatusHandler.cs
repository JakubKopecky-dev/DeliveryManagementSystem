using DeliveryService.Command.Application.Abstraction.Massaging;
using DeliveryService.Command.Application.DTOs.Delivery;
using DeliveryService.Command.Application.Interfaces.Repositories;
using MassTransit;
using Shared.Contracts.Enums;
using Shared.Contracts.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryService.Command.Application.Features.Delivery.ChangeDeliveryStatus
{
    public class ChangeDeliveryStatusHandler(IDeliveryRepisotry deliveryRepository, ITopicProducer<DeliveryStatusChangedEvent> producer ) : ICommandHandler<ChangeDeliveryStatusCommand,DeliveryDto?>
    {
        private readonly IDeliveryRepisotry _deliveryRepisotry = deliveryRepository;
        private readonly ITopicProducer<DeliveryStatusChangedEvent> _producer = producer ;



        public async Task<DeliveryDto?> Handle(ChangeDeliveryStatusCommand command, CancellationToken ct = default)
        {
            Domain.Entities.Delivery? delivery = await _deliveryRepisotry.FindByIdAsync(command.Id, ct);
            if(delivery is null)
                return null;

            delivery.UpdatedAt = DateTime.UtcNow;

            if (command.ChangeDeliveryStatusDto.Status is Domain.Enums.DeliveryStatus.Delivered)
                delivery.DeliveredAt = DateTime.UtcNow;

            Domain.Entities.Delivery updatedDelivery = await _deliveryRepisotry.UpdateAsync(delivery,ct);


            DeliveryStatusChangedEvent deliveryEvent = new(updatedDelivery.Id, (DeliveryStatus)(int)updatedDelivery.Status,updatedDelivery.UpdatedAt!.Value, updatedDelivery.UpdatedAt);
            await _producer.Produce(deliveryEvent, ct);

            return updatedDelivery.DeliveryToDeliveryDto();
        }



    }
}
