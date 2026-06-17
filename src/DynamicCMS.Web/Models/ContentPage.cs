using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DynamicCMS.Web.Models;

[Index(nameof(Slug), IsUnique = true)]
public class ContentPage
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Display(Name = "Published")] 
    public bool IsPublished { get; set; }

    [Display(Name = "Content")]
    public string? BodyHtml { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
