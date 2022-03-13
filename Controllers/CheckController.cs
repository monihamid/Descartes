using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using DiffingAPI.Helper;
using DiffingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiffingAPI.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class CheckController : ControllerBase
    {
        private readonly DiffContext _context;
        private readonly ILogger<CheckController> _logger;
        private readonly IDiffDataHelper _diffHelper;
        public CheckController(DiffContext context, ILogger<CheckController> log, IDiffDataHelper dh)
        {
            _context = context;
            _logger = log;
            _diffHelper = dh;
        }
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
        
    }
        
}
