using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangfireController : ControllerBase
    {
        private readonly IJobService _jobService;
        public HangfireController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobsExecuting()
        {
            return StatusCode((int)HttpStatusCode.OK, _jobService.GetProcessingJobs());
        }

        [HttpGet("Statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            return StatusCode((int)HttpStatusCode.OK, _jobService.GetJobStatistics());
        }
    }
}
