using Microsoft.EntityFrameworkCore;
using DynamicCMS.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add EF Core with SQLite
builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseSqlite($"Data Source={System.IO.Path.Combine(builder.Environment.ContentRootPath, "cms.db")}"));

var app = builder.Build();

// Initialize and seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CmsDbContext>();
    db.Database.EnsureCreated();
    if (!db.ContentPages.Any(p => p.Slug == "home"))
    {
        db.ContentPages.Add(new DynamicCMS.Web.Models.ContentPage
        {
            Title = "Home",
            Slug = "home",
            IsPublished = true,
            BodyHtml = "<h1>Welcome</h1><p>This is your dynamic home page.</p>",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Admin route (explicit prefix)
app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=Pages}/{action=Index}/{id?}");

// Default MVC route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// CMS dynamic page route as a catch-all (placed last)
app.MapControllerRoute(
    name: "cms",
    pattern: "{**slug}",
    defaults: new { controller = "Page", action = "Show" });

app.Run();
