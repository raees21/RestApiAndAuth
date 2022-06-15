using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class FootSide
{
    public int Id { get; set; }
    public EFootSide Side { get; set; }
}