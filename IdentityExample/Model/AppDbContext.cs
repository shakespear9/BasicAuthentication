using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Model
{

    // IdentityDbContext contains all the user tables



    // if using your own class 
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    //public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base (options)
        {

        }
    }
}
