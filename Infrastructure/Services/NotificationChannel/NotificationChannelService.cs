using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.ServicesImpl;
using Domain.Enums;
using Domain.Entities;
using Persistence.Repositories;
using System.Runtime.InteropServices;

namespace Infrastructure.Services.NotificationChannel
{
    public class NotificationChannelService : INotificationChannelService
    {
        private readonly IRepository<Domain.Entities.NotificationChannel> _repository;

        public NotificationChannelService(IRepository<Domain.Entities.NotificationChannel> repository)
        {
            _repository = repository;
        }

        public async Task<Domain.Entities.NotificationChannel> AddNotificationChannelAsync(CreateNotificationChannelRequestDto dto)
        {
            // Kullanıcı başına tek kanal constraint      
            var exists = (await _repository.GetFilteredAsync(x => x.UserId == dto.UserId && x.ChannelType == dto.ChannelType)).Any();
            if (exists)
                throw new InvalidOperationException("Bu kullanıcı zaten bu tipte bir bildirim kanalı tanımlamış!");

            var entity = new Domain.Entities.NotificationChannel
            {
                Id = Guid.NewGuid(),
                UserId = dto.UserId,
                WatchedAddress = dto.WatchedAddress,
                ChannelType = dto.ChannelType,
                Target = dto.Target,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return entity;
        }

        public async Task<Domain.Entities.NotificationChannel> UpdateNotificationChannelAsync(CreateNotificationChannelRequestDto dto)
        {
            var entityList = await _repository.GetFilteredAsync(x => x.Id == dto.Id);
            var entity = entityList.FirstOrDefault();

            if (entity == null)
                throw new KeyNotFoundException("Bildirim kanalı bulunamadı!");

            entity.WatchedAddress = dto.WatchedAddress;
            entity.ChannelType = dto.ChannelType;
            entity.Target = dto.Target;
            // Gerekirse başka alanları da ekle  

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();
            return entity;
        }

        public async Task<Domain.Entities.NotificationChannel> DeleteNotificationChannelAsync(Guid channelId)
        {
            var entityList = await _repository.GetFilteredAsync(x => x.Id == channelId);
            var entity = entityList.FirstOrDefault();

            if (entity == null)
                throw new KeyNotFoundException("Bildirim kanalı bulunamadı!");

            await _repository.DeleteAsync(channelId);
            await _repository.SaveChangesAsync();
            return entity;
        }

        public async Task<List<Domain.Entities.NotificationChannel>> GetNotificationChannelsAsync(ChannelType? channelType, int? userId)
        {
            var allData = await _repository.GetAllAsync();
            var filtered = allData.AsEnumerable();

            //channelType varsa veya userId varsa filtrele
            if (channelType.HasValue) 
            {
                filtered = filtered.Where(x => x.ChannelType == channelType.Value);
            }
            if (userId.HasValue) 
            {
                filtered = filtered.Where(x => x.UserId == userId.Value);
            }
            return filtered.ToList();
        }

        public async Task<Domain.Entities.NotificationChannel> GetNotificationChannelByIdAsync(Guid channelId)
        {
            var entityList = await _repository.GetFilteredAsync(x => x.Id == channelId);
            var entity = entityList.FirstOrDefault();

            if (entity == null)
                throw new KeyNotFoundException("Bildirim kanalı bulunamadı!");

            return entity;
        }
    }
}
