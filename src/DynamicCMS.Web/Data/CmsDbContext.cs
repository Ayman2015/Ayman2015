using DynamicCMS.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicCMS.Web.Data;

public class CmsDbContext : DbContext
{
    public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options)
    {
    }

    public DbSet<ContentPage> ContentPages => Set<ContentPage>();
}
