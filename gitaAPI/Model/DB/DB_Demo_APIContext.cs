using System;
using Microsoft.EntityFrameworkCore;
using gitaAPI.Data;
using gitaAPI.Model;

namespace upformapi.Model.DB
{
    public partial class DB_Demo_APIContext : DbContext
{
    public DB_Demo_APIContext()
    {
    }

    public DB_Demo_APIContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

        public object output { get; internal set; }
    }
}
