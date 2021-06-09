using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace FrontToBack.Helpers
{
    public class IdentityErrorDescriberAz : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError 
            {
                Code = nameof(DuplicateEmail),
                Description = $"Bu {email} artiq Movcuddur"
            };
        }
    }
}
