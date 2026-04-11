using VisitorLog_PDFD.Data;
using VisitorLog_PDFD.Models;
using VisitorLog_PDFD.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CountriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CountriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(int[] selectedContinentIds)
    {
        var countryViewModels = _context.Countries
            .Join(_context.SelectedContinents.Where(continent => !continent.IsDeleted), // Filter out deleted continents,
                  country => country.ContinentId,   // Assuming State table has CountryId
                  selectedContinent => selectedContinent.ContinentId,
                  (country, selectedContinent) => new CountryViewModel
                  {
                      CountryId = country.CountryId,
                      CountryName = country.Name,
                      SelectedContinentId = selectedContinent.SelectedContinentId,
                      ContinentName = _context.Continents
                          .Where(c => c.ContinentId == country.ContinentId)
                          .Select(c => c.Name)
                          .FirstOrDefault() ?? "Unknown Continent",
                      NameTypeName = country.NameType != null ? country.NameType.Name : "Unknown NameType", // Include NameTypeName
                      IsSelected = _context.SelectedCountries
                        .Any(sc => sc.CountryId == country.CountryId &&
                                   selectedContinentIds.Contains(sc.SelectedContinentId) &&
                                   !sc.IsDeleted)
                  })
            .Where(cv => selectedContinentIds.Contains(cv.SelectedContinentId)) // Filter by selected countries
            .ToList();

        return View(countryViewModels);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Dictionary<int, int[]>? selectedCountries)
    {
        var allActiveSelections = new List<int>();

        // Process each selectedContinentId and its associated selected countries
        foreach (var entry in selectedCountries)
        {
            int selectedContinentId = entry.Key; // Get the selectedContinentId
            int[] countryIds = entry.Value; // Get the selected CountryIds for this continent

            // Get existing selections for this continent
            var existingSelections = await _context.SelectedCountries
                .Where(sc => sc.SelectedContinentId == selectedContinentId)
                .ToListAsync();

            // Determine unchanged, reversed, new, and deleted selections
            var unchangedSelections = existingSelections
                .Where(sc => countryIds.Contains(sc.CountryId) && !sc.IsDeleted)
                .ToList();

            var reversedSelections = existingSelections
            .Where(sc => countryIds.Contains(sc.CountryId) && sc.IsDeleted)
            .ToList();

            // Mark deleted selections
            foreach (var selection in reversedSelections)
            {
                selection.IsDeleted = false;
            }

            var newSelections = countryIds
                .Except(existingSelections.Select(sc => sc.CountryId))
                .Select(countryId => new SelectedCountry
                {
                    SelectedContinentId = selectedContinentId,
                    CountryId = countryId,
                    IsDeleted = false
                })
                .ToList();

            var deletedSelections = existingSelections
                .Where(sc => !countryIds.Contains(sc.CountryId))
                .ToList();

            // Mark deleted selections
            foreach (var selection in deletedSelections)
            {
                selection.IsDeleted = true;
            }

            // Add new selections
            _context.SelectedCountries.AddRange(newSelections);

            // Save changes for this continent's batch
            await _context.SaveChangesAsync();

            // Collect active selections (unchanged + new)
            allActiveSelections.AddRange(unchangedSelections.Select(sc => sc.SelectedCountryId));
            allActiveSelections.AddRange(newSelections.Select(sc => sc.SelectedCountryId));
            allActiveSelections.AddRange(reversedSelections.Select(sc => sc.SelectedCountryId));
        }

        // Redirect to a confirmation page or the next step
        return RedirectToAction("Index", "States", new { selectedCountryIds = allActiveSelections.ToArray() });
    }
}
