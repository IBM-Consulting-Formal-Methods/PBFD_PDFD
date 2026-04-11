using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLog_PDFD.Data;
using VisitorLog_PDFD.Models;
using VisitorLog_PDFD.ViewModels;

public class StatesController : Controller
{
    private readonly ApplicationDbContext _context;

    public StatesController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index(int[] selectedCountryIds)
    {
        // Fetch data for countries and related continents

        var stateViewModels = _context.States
            .Join(_context.SelectedCountries.Where(country => !country.IsDeleted), // Filter out deleted continents,
                  state => state.CountryId,   // Assuming State table has CountryId
                  selectedCountry => selectedCountry.CountryId,
                  (state, selectedCountry) => new StateViewModel
                  {
                      StateId = state.StateId,
                      StateName = state.Name,
                      SelectedCountryId = selectedCountry.SelectedCountryId,
                      CountryName = _context.Countries
                          .Where(c => c.CountryId == state.CountryId)
                          .Select(c => c.Name)
                          .FirstOrDefault()??"Unknown Country",
                      NameTypeName = state.NameType != null ? state.NameType.Name : "Unknown NameType", // Include NameTypeName
                      IsSelected = _context.SelectedStates
                    .Any(sc => sc.StateId == state.StateId &&
                               selectedCountryIds.Contains(sc.SelectedCountryId) &&
                               !sc.IsDeleted)
                  })
            .Where(cv => selectedCountryIds.Contains(cv.SelectedCountryId)) // Filter by selected countries
            .ToList();

        return View(stateViewModels);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Dictionary<int, int[]> selectedStates)
    {
        var allActiveSelections = new List<int>();

        // Process each selectedCountryId and its associated selected states
        foreach (var entry in selectedStates)
        {
            int selectedCountryId = entry.Key; // Get the selectedCountryId
            int[] stateIds = entry.Value; // Get the selected stateIds for this country

            // Get existing selections for this country
            var existingSelections = await _context.SelectedStates
                .Where(sc => sc.SelectedCountryId == selectedCountryId)
                .ToListAsync();

            // Determine unchanged, reversed, new, and deleted selections
            var unchangedSelections = existingSelections
                .Where(sc => stateIds.Contains(sc.StateId) && !sc.IsDeleted)
                .ToList();

            var reversedSelections = existingSelections
                .Where(sc => stateIds.Contains(sc.StateId) && sc.IsDeleted)
                .ToList();

            // Mark deleted selections
            foreach (var selection in reversedSelections)
            {
                selection.IsDeleted = false;
            }


            var newSelections = stateIds
                .Except(existingSelections.Select(sc => sc.StateId))
                .Select(stateId => new SelectedState
                {
                    SelectedCountryId = selectedCountryId,
                    StateId = stateId,
                    IsDeleted = false
                })
                .ToList();

            var deletedSelections = existingSelections
                .Where(sc => !stateIds.Contains(sc.StateId))
                .ToList();

            // Mark deleted selections
            foreach (var selection in deletedSelections)
            {
                selection.IsDeleted = true;
            }

            // Add new selections
            _context.SelectedStates.AddRange(newSelections);

            // Save changes for this country's batch
            await _context.SaveChangesAsync();

            // Collect active selections (unchanged + new+ reversed)
            allActiveSelections.AddRange(unchangedSelections.Select(sc => sc.SelectedStateId));
            allActiveSelections.AddRange(newSelections.Select(sc => sc.SelectedStateId));
            allActiveSelections.AddRange(reversedSelections.Select(sc => sc.SelectedStateId));
        }

        // Redirect to a confirmation page or the next step
        return RedirectToAction("Index", "Counties", new { selectedStateIds = allActiveSelections.ToArray() });
    }
}
