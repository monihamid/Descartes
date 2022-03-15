
namespace DiffingAPI.Models 
{
public class Diff
{
    public int Id {get; set;}
    public int DiffKey{ get; set; }
    public string Data{ get; set; }
    public string Side {get; set;}
    public int  Size {get; set;}          

}
public enum DiffSide
    {
        left,
        right,
    }
}