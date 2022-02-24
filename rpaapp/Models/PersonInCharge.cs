namespace rpaapp.Models;

public class PersonInCharge
{
    public int id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string fullname { get { return FirstName + " " + LastName;}}
}