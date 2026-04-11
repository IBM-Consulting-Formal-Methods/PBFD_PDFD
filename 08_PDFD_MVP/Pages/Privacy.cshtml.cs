using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VisitorLog_PDFD.Pages
{
    public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger = logger;

        public void OnGet()
        {
        }
    }

}
