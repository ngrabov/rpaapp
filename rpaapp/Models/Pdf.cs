using System.ComponentModel.DataAnnotations;
namespace rpaapp.Models;

public class Pdf
{
    public int Id { get; set; }
    public string fname { get; set; }
    public Guid guid { get; set; }
    public long fsize { get; set; }
    
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy.}")] 
    public DateTime uploaded { get; set; }
}