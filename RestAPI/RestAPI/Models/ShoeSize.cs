using System.ComponentModel.DataAnnotations;
using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class ShoeSize
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Size { get; set; }
    [Required]
    public EShoeSizeCode Code { get; set; }
}

public class ShoeSizeCreate {
    [Required]
    public string Size { get; set; }
    [Required]
    public EShoeSizeCode Code { get; set; }
}

public class ShoeSizeUpdate
{
    [Required]

    public string Size { get; set; }
    [Required]

    public EShoeSizeCode Code { get; set; }
}