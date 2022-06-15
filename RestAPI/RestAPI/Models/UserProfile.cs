using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RestAPI.Common.Enums;
using RestAPI.DataAccess;

namespace RestAPI.Models;

public class UserProfile
{
    [Required]
    public Guid Id { get; set; }
    public UserType UserType { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    public string? Surname { get; set; }
    
    [Required]
    public string Email { get; set; }

    [Required]
    public Gender Gender { get; set; }
    
    public IEnumerable<Address> Addresses { get; set; }
    
    public UserProfile() {}

    public UserProfile(UserProfileCreate userProfile, UserType userType, Gender gender, Guid userID)
    {
        Id = userID;
        UserType = userType;
        FirstName = userProfile.FirstName;
        Surname = userProfile.Surname;
        Email = userProfile.Email;
        Gender = gender;
    }
}

public class UserProfileCreate
{
    [Required]
    public string FirstName { get; set; }
    public string? Surname { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public EGender Gender { get; set; }
}

public class UserProfileUpdate
{
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public EGender? Gender { get; set; }
}

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string? Surname { get; set; }
    public string Email { get; set; }
    public EGender Gender { get; set; }
    public EUserType Role { get; set; }
}