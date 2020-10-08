using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Model
{
    public class Role : IdentityRole<Guid>
    {
        public byte RoleId { get; set; }

        public string RoleName { get; set; }

    }
}
