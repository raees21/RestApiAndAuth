using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class Gender
{
    public int Id { get; set; }
    public EGender Name { get; set; }
    
    public Gender() {}
}