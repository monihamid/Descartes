using System.Threading.Tasks;

namespace DiffingAPI.Helper
{

    public interface IDiffDataHelper
    {

        Task<bool> UpdateDiff(string side, int id, Diff diffdata);


    }

}