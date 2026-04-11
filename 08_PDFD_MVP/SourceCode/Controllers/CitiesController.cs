using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitorLog_PDFD.Data;
using VisitorLog_PDFD.Models;
using VisitorLog_PDFD.ViewModels;

namespace VisitorLog_PDFD.Controllers
{
    public class CitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int[] selectedCountyIds)
        {
            // Fetch data for cities and related counties
            var cityViewModels = _context.Cities
                .Join(_context.SelectedCounties.Where(county => !county.IsDeleted), // Filter out deleted continents
                      city => city.CountyId,   // Assuming City table has CountyId
                      personSelectedCounty => personSelectedCounty.CountyId,
                      (city, personSelectedCounty) => new CityViewModel
                      {
                          CityId = city.CityId,
                          CityName = city.Name,
                          SelectedCountyId = personSelectedCounty.SelectedCountyId,
                          CountyName = _context.Counties
                              .Where(c => c.CountyId == city.CountyId)
                              .Select(c => c.Name)
                              .FirstOrDefault()??"Unknown County",
                          NameTypeName = city.NameType != null ? city.NameType.Name : "Unknown NameType", // Include NameTypeName
                          IsSelected = _context.SelectedCities
                    .Any(sc => sc.CityId == city.CityId &&
                               selectedCountyIds.Contains(sc.SelectedCountyId) &&
                               !sc.IsDeleted)
                      })
                .Where(cv => selectedCountyIds.Contains(cv.SelectedCountyId)) // Filter by selected countries
                .ToList();

            return View(cityViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Dictionary<int, int[]> selectedCities)
        {
            var allActiveSelections = new List<int>();

            // Process each selectedCountyId and its associated selected cities
            foreach (var entry in selectedCities)
            {
                int selectedCountyId = entry.Key; // Get the selectedCountyId
                int[] cityIds = entry.Value; // Get the selected cityIds for this county

                // Get existing selections for this county
                var existingSelections = await _context.SelectedCities
                    .Where(sc => sc.SelectedCountyId == selectedCountyId)
                    .ToListAsync();

                // Determine unchanged, new, and deleted selections
                var unchangedSelections = existingSelections
                    .Where(sc => cityIds.Contains(sc.CityId) && !sc.IsDeleted)
                    .ToList();

                var reversedSelections = existingSelections
                .Where(sc => cityIds.Contains(sc.CityId) && sc.IsDeleted)
                .ToList();

                // Mark deleted selections
                foreach (var selection in reversedSelections)
                {
                    selection.IsDeleted = false;
                }

                var newSelections = cityIds
                    .Except(existingSelections.Select(sc => sc.CityId))
                    .Select(cityId => new SelectedCity
                    {
                        SelectedCountyId = selectedCountyId,
                        CityId = cityId,
                        IsDeleted = false
                    })
                    .ToList();

                var deletedSelections = existingSelections
                    .Where(sc => !cityIds.Contains(sc.CityId))
                    .ToList();

                // Mark deleted selections
                foreach (var selection in deletedSelections)
                {
                    selection.IsDeleted = true;
                }

                // Add new selections
                _context.SelectedCities.AddRange(newSelections);

                // Save changes for this county's batch
                await _context.SaveChangesAsync();

                // Collect active selections (unchanged + reversed + new)
                allActiveSelections.AddRange(unchangedSelections.Select(sc => sc.SelectedCityId));
                allActiveSelections.AddRange(newSelections.Select(sc => sc.SelectedCityId));
                allActiveSelections.AddRange(reversedSelections.Select(sc => sc.SelectedCityId));
            }

            // Redirect to a confirmation page or the next step
            return RedirectToAction("Index", "Reports", new { selectedCityIds = allActiveSelections.ToArray() });
        }

    }
}
