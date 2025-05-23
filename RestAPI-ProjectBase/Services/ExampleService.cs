﻿using AutoMapper;
using Data.Database.EntityFrameworkContexts;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class ExampleService : IExampleService
    {
        private readonly Utils _utils;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public ExampleService(Utils utils, IMapper mapper, IDbContextFactory<AppDbContext> dbFactory)
        {
            _utils = utils;
            _mapper = mapper;
            _dbFactory = dbFactory;
        }

        public async Task<DefaultReponseModel> Example()
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

            return new DefaultReponseModel()
            {
                Success = true,
                StatusCode = 200,
                Message = "Example Service",
                Data = currentLogs
            };
        }
    }
}
