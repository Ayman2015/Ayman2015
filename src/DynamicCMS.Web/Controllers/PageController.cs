using System.Threading.Tasks;
using DynamicCMS.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicCMS.Web.Controllers;

public class PageController : Controller
{
    private readonly CmsDbContext _db;
    public PageController(CmsDbContext db) { _db = db; }

    public async Task<IActionResult> Show(string? slug)
    {
        slug = string.IsNullOrWhiteSpace(slug) ? "home" : slug;
        var page = await _db.ContentPages.FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);
        if (page == null) return NotFound();
        return View(page);
    }
}
