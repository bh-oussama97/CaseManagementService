using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string CompanyDataBaseName { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

      //  public ICollection<ResetPassword> UserResetsPassword {get;set;}
    }
}
