using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Domain
{
    public class AppUser : IdentityUser,IMustHaveTenant
    {
        public string FirstName { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

      //  public ICollection<ResetPassword> UserResetsPassword {get;set;}
    }
}
