using System.ComponentModel.DataAnnotations;

namespace RestAPI.Models;

public class Address
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public string Country { get; set; }
    
    public string? Province { get; set; }
    
    [Required]
    public string City { get; set; }
    
    public string? Suburb { get; set; }
    
    [Required]
    public int PostalCode { get; set; }
    
    [Required]
    public string StreetNumber { get; set; }
    
    [Required]
    public string StreetName { get; set; }
    
    public string? UnitNumber { get; set; }
    
    public string? ComplexName { get; set; }
    
    [Required]
    public UserProfile UserProfile { get; set; }

    public Address() {}
    
    public Address(AddressCreate address)
    {
        Country = address.Country;
        Province = address.Province;
        City = address.City;
        Suburb = address.Suburb;
        PostalCode = address.PostalCode;
        StreetNumber = address.StreetNumber;
        StreetName = address.StreetName;
        UnitNumber = address.UnitNumber;
        ComplexName = address.ComplexName;
    }
    
    public Address(AddressCreate address, UserProfile userProfile)
    {
        Country = address.Country;
        Province = address.Province;
        City = address.City;
        Suburb = address.Suburb;
        PostalCode = address.PostalCode;
        StreetNumber = address.StreetNumber;
        StreetName = address.StreetName;
        UnitNumber = address.UnitNumber;
        ComplexName = address.ComplexName;
        UserProfile = userProfile;
    }
}

public class AddressCreate 
{
    [Required]
    public string Country { get; set; }
    public string? Province { get; set; }
    [Required]
    public string City { get; set; }
    public string? Suburb { get; set; }
    [Required]
    public int PostalCode { get; set; }
    [Required]
    public string StreetNumber { get; set; }
    [Required]
    public string StreetName { get; set; }
    public string? UnitNumber { get; set; }
    public string? ComplexName { get; set; }

    public AddressCreate()
    {
    }
}

public class AddressUpdate
{
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Suburb { get; set; }
    public int? PostalCode { get; set; }
    public string? StreetNumber { get; set; }
    public string? StreetName { get; set; }
    public string? UnitNumber { get; set; }
    public string? ComplexName { get; set; }

    public AddressUpdate()
    {
    }
}

public class AddressResponse
{
    public Guid Id { get; set; }
    public string Country { get; set; }
    public string? Province { get; set; }
    public string City { get; set; }
    public string? Suburb { get; set; }
    public int PostalCode { get; set; }
    public string StreetNumber { get; set; }
    public string StreetName { get; set; }
    public string? UnitNumber { get; set; }
    public string? ComplexName { get; set; }

    public AddressResponse()
    {
        
    }
    public AddressResponse(Address? address)
    {
        Id = address.Id;
        Country = address.Country;
        City = address.City;
        Province = address.Province;
        Suburb = address.Suburb;
        PostalCode = address.PostalCode;
        StreetNumber = address.StreetNumber;
        StreetName = address.StreetName;
        UnitNumber = address.UnitNumber;
        ComplexName = address.ComplexName;
    }
    
}