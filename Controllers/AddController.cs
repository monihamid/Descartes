using System.Threading.Tasks;
using DiffingAPI.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiffingAPI.Controllers
{
    [ApiController]
    public class AddController : ControllerBase
    {
        private readonly ILogger<AddController> _logger;
        private readonly IDiffDataHelper _diffHelper;
        public AddController(ILogger<AddController> log, IDiffDataHelper dh)
        {
            _logger = log;
            _diffHelper = dh;
        }
        [Route("v1/diff/{Id}/left")]
        [HttpPut("{id}")]        
        public async Task<ActionResult<Diff>> PutDiffData(int id, Diff diffdata)
        {
            //data validation
            if (diffdata == null || string.IsNullOrWhiteSpace(diffdata.Data))
            {
                return BadRequest();
            }
            // define the data object
            diffdata.Side = "left";  //hardcoded not good
            bool addDiff = await _diffHelper.UpdateDiff(diffdata.Side, id, diffdata);
             if(addDiff)
             {
                 return Created("Created","201");
             }
             else
             {
                 _logger.LogError($"Failed to update or add diff");
                 return BadRequest("Failed to update or add diff");

             }
        }

        [Route("v1/diff/{Id}/right")]
        [HttpPut("{Id}")]        
        public async Task<ActionResult<Diff>> PostDiffData(int id, Diff diffdata)
        {
            if (diffdata == null || string.IsNullOrWhiteSpace(diffdata.Data))
            {
                return BadRequest();
            }
            diffdata.Side = "right";  //hardcoded not good
            bool addDiff = await _diffHelper.UpdateDiff(diffdata.Side, id, diffdata);
            if(addDiff)
             {
                 return Created("Created","201");
             }
             else
             {
                 _logger.LogError($"Failed to update or add diff");
                 return BadRequest("Failed to update or add diff");
             }
        }
    }
}