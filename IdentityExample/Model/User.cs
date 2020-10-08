﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Model
{
    public class User : IdentityUser<Guid>
    {
        public string Username { get; set; }

        public string password { get; set; }

    }
}