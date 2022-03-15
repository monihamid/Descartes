
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiffingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DiffingAPI.Helper
{   
    public class DiffDataHelper: IDiffDataHelper
    {
        private readonly DiffContext _context;
        private readonly ILogger<DiffDataHelper> _logger;
        
        public DiffDataHelper(DiffContext dc, ILogger<DiffDataHelper> log)
        {
            _context = dc;
            _logger = log;
        }
        public async Task<bool> UpdateDiff(string side, int id, Diff diffdata)
        {
            //get the size
            int size = diffdata.Data.Length;
            // define the data object
            diffdata.DiffKey = id;
            diffdata.Size = size;
            bool update = false;
            try
            {
                List<Diff> diff = await DiffExists(diffdata.DiffKey, diffdata.Side);
                if(diff.Count > 0)
                {
                    var updatediff = _context.Diff.First(d => d.Side == side &&
                    d.DiffKey == id);
                    updatediff.Data = diffdata.Data;
                    updatediff.Size = size;
                    _context.SaveChanges();
                    update = true; 
                }
                else if (diff.Count == 0)
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
        ///<summary> 
        /// Checks if diff data  exists or not
        ///</summary>
        ///<param name="id">int </param>
        ///<param name="side">string </param>
        ///<return> boolean: true if did data exist </return>

        public async Task<List<Diff>> DiffExists(int id, string side)
        {
            List<Diff> diffExist = new List<Diff>();
            return diffExist = await  _context.Diff.AsNoTracking().Where(d => d.DiffKey == id &&
              d.Side == side).ToListAsync();
         }

    }
}
    