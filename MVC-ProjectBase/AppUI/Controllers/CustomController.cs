using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AppUI.Controllers
{
    public class CustomController<T> : Controller
    {
        private readonly ILogger<T> _logger;
        private readonly IMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly ApplicationUtils _utils;

        public CustomController(ILogger<T> logger,
                                IMemoryCache cache,
                                IMapper mapper,
                                ApplicationUtils utils)
        {
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
            _utils = utils;
        }

        public void SetOrUpdateCookie(string key, string value, int? expireDays = null)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddDays(expireDays ?? 1),
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps,
                SameSite = SameSiteMode.Lax
            };

            HttpContext.Response.Cookies.Append(key, value, cookieOptions);
        }

        public void RemoveCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        public void ClearAllCookies(HttpContext httpContext)
        {
            foreach (var cookie in httpContext.Request.Cookies.Keys)
            {
                if (cookie.ToUpper().StartsWith(".ASPNETCORE."))
                {
                    continue;
                }
                RemoveCookie(cookie);
            }
        }

        public string ReadCookie(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string? cookie);

            if (String.IsNullOrEmpty(cookie))
            {
                return string.Empty;
            }

            return cookie;
        }

        public X Map<X>(object source)
        {
            return _mapper.Map<X>(source);
        }

        #region Loaders

        #endregion
    }
}
