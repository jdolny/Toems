using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Toems_ServiceCore.Data;

public class AppUser : IdentityUser
{
    public int UserId { get; set; }
    public string? DisplayName { get; set; }

    public bool IsLdapUser { get; set; }
    
    public int UserGroupId { get; set; }

    public string Theme { get; set; }
    
    public string? ImagingToken { get; set; }
    
    public string DefaultComputerView { get; set; }
    
    public string ComputerSortMode { get; set; }
    
    public string DefaultLoginPage { get; set; }
    
    public bool EnableWebMfa { get; set; }
    
    public bool EnableImagingMfa { get; set; }
    
    public string Membership { get; set; }
}