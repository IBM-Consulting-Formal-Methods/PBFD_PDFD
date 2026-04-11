using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLog_PDFD.Data;
using VisitorLog_PDFD.Models;
using VisitorLog_PDFD.ViewModels;

namespace VisitorLog_PDFD.Controllers
{
    public class CountiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CountiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int[] selectedStateIds)
        {
            // Fetch data for counties and related states
            var countyViewModels = _context.Counties
                .Join(_context.SelectedStates.Where(state => !state.IsDeleted), // Filter out deleted continents
                      county => county.StateId,   // Assuming County table has StateId
                      selectedState => selectedState.StateId,
                      (county, selectedState) => new CountyViewModel
                      {
                          CountyId = county.CountyId,
                          CountyName = county.Name,
                          SelectedStateId = selectedState.SelectedStateId,
                          StateName = _context.States
                              .Where(c => c.StateId == county.StateId)
                              .Select(c => c.Name)
                              .FirstOrDefault()??"Unknown State",
                          NameTypeName = county.NameType != null ? county.NameType.Name : "Unknown NameType", // Include NameTypeName
                          IsSelected = _context.SelectedCounties
                    .Any(sc => sc.CountyId == county.CountyId &&
                               selectedStateIds.Contains(sc.SelectedStateId) &&
                               !sc.IsDeleted)
                      })
                .Where(cv => selectedStateIds.Contains(cv.SelectedStateId)) // Filter by selected states
                .ToList();

            return View(countyViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Dictionary<int, int[]> selectedCounties)
        {
            var allActiveSelections = new List<int>();

            // Process each selectedStateId and its associated selected counties
            foreach (var entry in selectedCounties)
            {
                int selectedStateId = entry.Key; // Get the selectedStateId
                int[] countyIds = entry.Value; // Get the selected countyIds for this state

                // Get existing selections for this state
                var existingSelections = await _context.SelectedCounties
                    .Where(sc => sc.SelectedStateId == selectedStateId)
                    .ToListAsync();

                // Determine unchanged, new, and deleted selections
                var unchangedSelections = existingSelections
                    .Where(sc => countyIds.Contains(sc.CountyId) && !sc.IsDeleted)
                    .ToList();

                var reversedSelections = existingSelections
                .Where(sc => countyIds.Contains(sc.CountyId) && sc.IsDeleted)
                .ToList();

                // Mark deleted selections
                foreach (var selection in reversedSelections)
                {
                    selection.IsDeleted = false;
                }

                var newSelections = countyIds
                    .Except(existingSelections.Select(sc => sc.CountyId))
                    .Select(countyId => new SelectedCounty
                    {
                        SelectedStateId = selectedStateId,
                        CountyId = countyId,
                        IsDeleted = false
                    })
                    .ToList();

                var deletedSelections = existingSelections
                    .Where(sc => !countyIds.Contains(sc.CountyId))
                    .ToList();

                // Mark deleted selections
                foreach (var selection in deletedSelections)
                {
                    selection.IsDeleted = true;
                }

                // Add new selections
                _context.SelectedCounties.AddRange(newSelections);

                // Save changes for this state's batch
                await _context.SaveChangesAsync();

                // Collect active selections (unchanged + new)
                allActiveSelections.AddRange(unchangedSelections.Select(sc => sc.SelectedCountyId));
                allActiveSelections.AddRange(newSelections.Select(sc => sc.SelectedCountyId));
                allActiveSelections.AddRange(reversedSelections.Select(sc => sc.SelectedCountyId));
            }

            // Redirect to a confirmation page or the next step
            return RedirectToAction("Index", "Cities", new { selectedCountyIds = allActiveSelections.ToArray().Distinct() });
        }

    }
}
