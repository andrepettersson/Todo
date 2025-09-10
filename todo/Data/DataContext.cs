using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todo.Entities;

namespace todo.Data;

public class DataContext : IdentityDbContext<User>
{
public DataContext(DbContextOptions<DataContext> options) : base(options) {}

public DbSet<TodoItem> Todo { get; set; }
}

    
 