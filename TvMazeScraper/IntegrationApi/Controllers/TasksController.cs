using System.Threading;
using System.Threading.Tasks;
using IntegrationBl.Services;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IUpdateService _updateBl;
        
        public TasksController(IUpdateService updateBl)
        {
            _updateBl = updateBl;
        }

        [Route("updatelist")]
        [HttpPost]
        public async Task<IActionResult> StartUpdateProcessAsync(CancellationToken cancellationToken)
        {
            await _updateBl.StartUpdateProcessAsync(cancellationToken);

            return Ok();
        }
    }
}
