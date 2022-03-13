using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DiffingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiffingAPI.Controllers
{
    [ApiController]
    public class DiffController : ControllerBase
    {
        private readonly DiffContext _context;
        private readonly ILogger<DiffController> _logger;
      public DiffController(DiffContext context, ILogger<DiffController> log)
        {
            _context = context;
            _logger = log;
        }
        // GET: api/diff/1
        [Route("v1/diff/{id}")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IList<Diff>>> GetDiff(int id)
        {
            //TODO create a sepater helper class
            dynamic result = new ExpandoObject();
            try
            {
                
                List<Diff> leftdiff = await _context.Diff.Where(d => d.DiffKey == id &&
                d.Side == "left").ToListAsync();

                List<Diff> rightdiff = await _context.Diff.Where(d => d.DiffKey == id &&
                d.Side == "right").ToListAsync();
                
                if (leftdiff.Count == 0 || rightdiff.Count == 0)
                {
                    return NotFound();
                }
            
                else if (leftdiff[0].Size.Equals( rightdiff[0].Size) && leftdiff[0].Data.Equals(rightdiff[0].Data))
                {
                    //return NotFound();
                    result.diffResultType = "Equals";
                    return Ok(result);

                }
                else if (!leftdiff[0].Size.Equals( rightdiff[0].Size))
                {
                    result.diffResultType = "SizeDoNotMatch";
                    return Ok(result);

                }
                else if(!leftdiff[0].Data.Equals(rightdiff[0].Data))  //when 2 side are not same
                {
                    List <dynamic> diff = new List<dynamic>();
                    dynamic addDiff = new ExpandoObject();

                    result.diffResultType = "ContentDoNotMatch";
                    //creating the addDiff object
                    addDiff.offset = 0; 
                    addDiff.length = 1;
                    //Adding/inserting addDiff ofject to diff list
                    diff.Add(addDiff);
                    //adding the diff list to result object 
                    dynamic addDiff1 = new ExpandoObject();
                    addDiff1.offset = 2; 
                    addDiff1.length = 2;
                    diff.Add(addDiff1);
                    result.diff= diff; 
                    return Ok(result);   

                }
                else
                {
                    return Ok();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Failed to Differ: {ex}");
                return BadRequest("Error Occurred");
            }
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
            diffdata.DiffKey = id;
            diffdata.Side = "left";  //hardcoded not good
             bool addDiff = await UpdateDiff(diffdata.Side, id, diffdata);
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
            bool addDiff = await UpdateDiff(diffdata.Side, id, diffdata);
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
        // should be moved to a helper class
        private async Task<bool> DiffExists(int id, string side)
        {
            bool diffExist = false;
            List<Diff> diff = await  _context.Diff.AsNoTracking().Where(d => d.DiffKey == id &&
             d.Side == side).ToListAsync();
             if(diff.Count >0)
             {
                 diffExist = true;
             }
             else
             {
              diffExist = false;   
             }
            return diffExist;
        }
        // TODO separate helper class
        private async Task<bool> UpdateDiff(string side, int id, Diff diffdata)
        {
            //get the size
            int size = diffdata.Data.Length;
            // define the data object
            diffdata.DiffKey = id;
            diffdata.Size = size;
            bool update = false;
            try
            {
                bool diff = await DiffExists(diffdata.DiffKey, diffdata.Side);
                if(diff)
                {
                    var updatediff = _context.Diff.First(d => d.Side == side &&
                    d.DiffKey == id);
                    updatediff.Data = diffdata.Data;
                    updatediff.Size = size;
                    _context.SaveChanges();
                    update = true; 
                }
                else
                {
                    _context.Diff.Add(diffdata);
                    await _context.SaveChangesAsync();
                    update = true; 
                }
             
            }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (diff)
            //     {
            //         //return NotFound();
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to update or add diff: {ex}");
                update = false; 
            }
            return update;

        }
    }
        
}
