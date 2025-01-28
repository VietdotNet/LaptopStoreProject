using System;
using System.Collections.Generic;
using LaptopStoreProject_MVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LaptopStoreProject_MVC;

public partial class LaptopStoreProjectContext : IdentityDbContext<AppUser>
{
    public LaptopStoreProjectContext()
    {
    }

    public LaptopStoreProjectContext(DbContextOptions<LaptopStoreProjectContext> options)
        : base(options)
    {
    }

    //Đã đky service DBcontext trong Program rồi thì hàm OnConfiguring này không cần thiết nữa
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        IConfigurationRoot configuration = builder.Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("LaptopStoreProject"));
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); //Phải gọi base trước khi thay đổi bất kỳ thực thể nào(ko gọi là ko migrations đc đâu) 

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
