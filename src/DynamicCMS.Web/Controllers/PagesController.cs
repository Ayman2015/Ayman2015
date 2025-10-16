using System;
using System.Linq;
using System.Threading.Tasks;
using DynamicCMS.Web.Data;
using DynamicCMS.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DynamicCMS.Web.Controllers;

[Route("admin/pages")]
public class PagesController : Controller
{
    private readonly CmsDbContext _db;

    public PagesController(CmsDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var pages = await _db.ContentPages
            .OrderByDescending(p => p.UpdatedAtUtc)
            .ToListAsync();
        return View(pages);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new ContentPage { IsPublished = true });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContentPage page)
    {
        if (!ModelState.IsValid)
        {
            return View(page);
        }

        page.Slug = GenerateSlug(string.IsNullOrWhiteSpace(page.Slug) ? page.Title : page.Slug);
        page.CreatedAtUtc = DateTime.UtcNow;
        page.UpdatedAtUtc = DateTime.UtcNow;

        var exists = await _db.ContentPages.AnyAsync(p => p.Slug == page.Slug);
        if (exists)
        {
            ModelState.AddModelError(nameof(ContentPage.Slug), "Slug already exists");
            return View(page);
        }

        _db.ContentPages.Add(page);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var page = await _db.ContentPages.FindAsync(id);
        if (page == null) return NotFound();
        return View(page);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContentPage input)
    {
        var page = await _db.ContentPages.FindAsync(id);
        if (page == null) return NotFound();

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        page.Title = input.Title;
        page.BodyHtml = input.BodyHtml;
        page.IsPublished = input.IsPublished;
        page.Slug = GenerateSlug(string.IsNullOrWhiteSpace(input.Slug) ? input.Title : input.Slug);
        page.UpdatedAtUtc = DateTime.UtcNow;

        var slugTaken = await _db.ContentPages.AnyAsync(p => p.Id != id && p.Slug == page.Slug);
        if (slugTaken)
        {
            ModelState.AddModelError(nameof(ContentPage.Slug), "Slug already exists");
            return View(input);
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var page = await _db.ContentPages.FindAsync(id);
        if (page == null) return NotFound();
        return View(page);
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var page = await _db.ContentPages.FindAsync(id);
        if (page == null) return NotFound();
        _db.ContentPages.Remove(page);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private static string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var normalized = value.ToLowerInvariant();
        var cleaned = new string(normalized
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray());
        while (cleaned.Contains("--")) cleaned = cleaned.Replace("--", "-");
        return cleaned.Trim('-');
    }
}
