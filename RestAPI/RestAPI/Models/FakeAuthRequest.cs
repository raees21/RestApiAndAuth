using RestAPI.Common.Enums;

namespace RestAPI.Models;

public class FakeAuthRequest
{
    public Guid? UserId { get; set; }
    public EUserType Type { get; set; }
}