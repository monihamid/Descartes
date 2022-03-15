using System.Threading.Tasks;
using DiffingAPI.Helper;
using DiffingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiffingAPI.Controllers
{
    [ApiController]
    public class PutDiffDataController : ControllerBase
    {
        private readonly ILogger<PutDiffDataController> _logger;
        private readonly IDiffDataHelper _diffHelper;
        public PutDiffDataController(ILogger<PutDiffDataController> log, IDiffDataHelper dh)
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
            diffdata.Side = DiffSide.left.ToString();
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
            diffdata.Side = DiffSide.right.ToString();
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