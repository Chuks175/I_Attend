using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using I_Attend.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

//using MySqlConnector;

namespace I_Attend.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<I_Attend.Models.View> View { get; set; } = default!;
}

