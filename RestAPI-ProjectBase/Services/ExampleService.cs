using AutoMapper;
using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ExampleService(Utils utils, IMapper mapper, IDbContextFactory<AppDbContext> dbFactory) : IExampleService
    {
        private readonly Utils _utils = utils;
        private readonly IMapper _mapper = mapper;
        private readonly IDbContextFactory<AppDbContext> _dbFactory = dbFactory;

        public async Task<DefaultResponseModel> Example()
        {
            _utils.SetLog("Services.ExampleService.Example method called", LogType.Information);
            List<LogEntity> currentLogs;
            using (var dbContext = await _dbFactory.CreateDbContextAsync())
            {
                dbContext.Logs.Add(new LogEntity
                {
                    Message = "Services.ExampleService.Example method called",
                    Type = LogType.Information,
                });
                await dbContext.SaveChangesAsync();
                currentLogs = await dbContext.Logs.ToListAsync();
            }

            return new DefaultResponseModel()
            {
                Success = true,
                StatusCode = 200,
                Message = "Example Service",
                Data = currentLogs
            };
        }
    }
}
