using DeliveryService.Command.Application.Abstraction.Massaging;
using DeliveryService.Command.Application.DTOs.Delivery;
using DeliveryService.Command.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using DeliveryService.Command.Domain.Enums;
using MassTransit;
using Shared.Contracts.Events;

namespace DeliveryService.Command.Application.Features.Delivery.CreateDelivery
{
    public class CreateDeliveryHandler(IDeliveryRepisotry deliveryRepisotry,ITopicProducer<DeliveryCreatedEvent> producer) : ICommandHandler<CreateDeliveryCommand, DeliveryDto>
    {
        private readonly IDeliveryRepisotry _deliveryRepository = deliveryRepisotry;
        private readonly ITopicProducer<DeliveryCreatedEvent> _producer = producer;



        public async Task<DeliveryDto> Handle(CreateDeliveryCommand command, CancellationToken ct = default)
        {
            CreateDeliveryDto createDto = command.CreateDeliveryDto;


            Domain.Entities.Delivery delivery = new()
            {
                Id = Guid.Empty,
                OwnerId = createDto.OwnerId,
                ExternalOrderId = createDto.ExternalOrderId,
                CourierId = createDto.CourierId,
                RecipientName = createDto.RecipientName,
                Address = createDto.Address,
                Phone = createDto.Phone,
                PackageCount = (uint)createDto.PackageCount,
                PackageWeightKg = createDto.PackageWeightKg,
                TotalVolumeM3 = createDto.TotalVolumeM3,
                CreatedAt = DateTime.UtcNow,
                Status = DeliveryStatus.Created
            };


            Domain.Entities.Delivery createdDelivery = await _deliveryRepository.InsertAsync(delivery,ct);

            DeliveryCreatedEvent deliveryEvent = new()
            {
                Id = createdDelivery.Id,
                OwnerId = createdDelivery.OwnerId,
                ExternalOrderId = createdDelivery.ExternalOrderId,
                CourierId = createdDelivery.CourierId,
                RecipientName = createdDelivery.RecipientName,
                Address = createdDelivery.Address,
                Phone = createdDelivery.Phone,
                PackageCount = createdDelivery.PackageCount,
                PackageWeightKg = createdDelivery.PackageWeightKg,
                TotalVolumeM3 = createdDelivery.TotalVolumeM3,
                CreatedAt = createdDelivery.CreatedAt
            };

            await _producer.Produce(deliveryEvent, ct);

            return createdDelivery.DeliveryToDeliveryDto();
        }


    }
}
