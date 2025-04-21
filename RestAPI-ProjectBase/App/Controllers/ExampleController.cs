using Domain;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly Utils _utils;

        private readonly IExampleService _service;

        public ExampleController(Utils utils, IExampleService service)
        {
            _utils = utils;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DefaultReponseModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            _utils.SetLog("ExampleController Get method called", LogType.Information);
            return StatusCode((int)HttpStatusCode.OK, await _service.Example());
        }
    }
}
