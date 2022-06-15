using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class UserType
{
    public int Id { get; set; }
    public EUserType Type { get; set; }
}