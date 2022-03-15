using System.Collections.Generic;
using System.Threading.Tasks;
using DiffingAPI.Models;

namespace DiffingAPI.Helper
{

    public interface IDiffDataHelper
    {

        Task<bool> UpdateDiff(string side, int id, Diff diffdata);
        Task<List<Diff>> DiffExists(int id, string side);


    }

}