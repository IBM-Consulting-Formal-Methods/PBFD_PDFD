using VisitorLog_PDFD.Data;
using VisitorLog_PDFD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLog_PDFD.ViewModels;

public class ContinentsController : Controller
{
    private readonly ApplicationDbContext _context;
    public ContinentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Continents
    public async Task<IActionResult> Index(int personId)
    {
        ViewBag.PersonId = personId;

        // Get selected continents for the person
        var selectedContinentIds = await _context.SelectedContinents
            .Where(p => p.PersonId == personId && !p.IsDeleted)
            .Select(p => p.ContinentId)
            .ToListAsync();

        // Get all continents with their NameType and construct the ViewModel
        var continentViewModels = await _context.Continents
            .Include(c => c.NameType) // Eagerly load NameType
            .Select(c => new ContinentViewModel
            {
                ContinentId = c.ContinentId,
                ContinentName = c.Name,
                PersonId = personId,
                NameTypeName = c.NameType != null ? c.NameType.Name : "Unknown NameType",
                IsSelected = selectedContinentIds.Contains(c.ContinentId)
            })
            .ToListAsync();

        return View(continentViewModels);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(int personId, int[] selectedContinents)
    {
        if (selectedContinents == null || !selectedContinents.Any())
        {
            ModelState.AddModelError("", "No continents were selected.");
            return RedirectToAction("Index", "Continents", new { personId = personId });
        }

        var existingSelections = await _context.SelectedContinents
            .Where(p => p.PersonId == personId)
            .ToListAsync();

        // Determine unchanged, reversed, new, and deleted selections
        var unchangedSelections = existingSelections
            .Where(p => selectedContinents.Contains(p.ContinentId) && !p.IsDeleted)
            .ToList();

        var reversedSelections = existingSelections
            .Where(p => selectedContinents.Contains(p.ContinentId) && p.IsDeleted)
            .ToList();

        // Mark deleted selections
        foreach (var selection in reversedSelections)
        {
            selection.IsDeleted = false;
        }

        var newSelections = selectedContinents
            .Except(existingSelections.Select(p => p.ContinentId))
            .Select(continentId => new SelectedContinent
            {
                PersonId = personId,
                ContinentId = continentId,
                IsDeleted = false
            })
            .ToList();

        var deletedSelections = existingSelections
            .Where(p => !selectedContinents.Contains(p.ContinentId))
            .ToList();

        // Mark deleted selections
        foreach (var selection in deletedSelections)
        {
            selection.IsDeleted = true;
        }

        // Add new selections
        _context.SelectedContinents.AddRange(newSelections);

        // Save all changes in one batch
        await _context.SaveChangesAsync();

        // Get all active selections (unchanged + reversed + new)
        var activeSelections = unchangedSelections
            .Select(p => p.SelectedContinentId)
            .Union(newSelections.Select(p => p.SelectedContinentId))
            .Union(reversedSelections.Select(p => p.SelectedContinentId))
            .ToArray();

        return RedirectToAction("Index", "Countries", new { selectedContinentIds = activeSelections });
    }
}
