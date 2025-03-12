using AutoMapper;
using Domain;
using Domain.Enums;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Services
{
    public class CustomService<T>
    {
        private readonly ILogger<T> _logger;
        private readonly IMapper _mapper;
        private readonly Utils _utils;

        public CustomService(ILogger<T> logger,
                             IMapper mapper,
                             Utils utils)
        {
            _logger = logger;
            _mapper = mapper;
            _utils = utils;
        }

        public void CS_Log(string message, LogTypeEnum type = LogTypeEnum.Information, PerformContext? context = null)
        {
            context?.ResetTextColor();
            switch (type)
            {
                case LogTypeEnum.Success:
                    context?.SetTextColor(ConsoleTextColor.Green);
                    _logger.LogInformation(message);
                    Trace.TraceInformation(message);
                    break;
                case LogTypeEnum.Information:
                    context?.SetTextColor(ConsoleTextColor.Cyan);
                    _logger.LogInformation(message);
                    Trace.TraceInformation(message);
                    break;
                case LogTypeEnum.Warning:
                    context?.SetTextColor(ConsoleTextColor.Yellow);
                    _logger.LogWarning(message);
                    Trace.TraceWarning(message);
                    break;
                case LogTypeEnum.Error:
                    context?.SetTextColor(ConsoleTextColor.Red);
                    _logger.LogError(message);
                    Trace.TraceError(message);
                    break;
                case LogTypeEnum.Critical:
                    context?.SetTextColor(ConsoleTextColor.DarkRed);
                    _logger.LogCritical(message);
                    Trace.TraceError(message);
                    break;
                case LogTypeEnum.Debug:
                    context?.SetTextColor(ConsoleTextColor.Magenta);
                    _logger.LogDebug(message);
                    Trace.TraceInformation(message);
                    break;
                case LogTypeEnum.Trace:
                    context?.SetTextColor(ConsoleTextColor.DarkGray);
                    _logger.LogTrace(message);
                    Trace.TraceInformation(message);
                    break;
                default:
                    context?.SetTextColor(ConsoleTextColor.Gray);
                    _logger.LogInformation(message);
                    Trace.TraceInformation(message);
                    break;
            }
            context?.WriteLine(message);
            context?.ResetTextColor();
        }

        public void CS_SetMemoryObject<X>(string key, X value)
        {
            _utils.SetMemoryObject(key, value);
        }
        public X? CS_GetMemoryObject<X>(string key)
        {
            return _utils.GetMemoryObject<X>(key);
        }

        public X CC_Map<X>(object source)
        {
            return _mapper.Map<X>(source);
        }
    }
}
