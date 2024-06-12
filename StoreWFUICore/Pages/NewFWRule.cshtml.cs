using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreWFUICore.Data;

namespace StoreWFUICore.Pages
{
    public class NewFWRuleModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFWDataService _ifwDataService;

        [BindProperty]
        public Storagefirewall Firewall { get; set; } = default!;

        public Root FWRoot { get; set; }
        public NewFWRuleModel(ILogger<IndexModel> logger, IFWDataService ifwDataService)
        {
            _logger = logger;
            _ifwDataService = ifwDataService;
            FWRoot = _ifwDataService.FWRoot;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (FWRoot.storagefirewalls.Exists(f => f.GetKey() == Firewall.GetKey()))
            {
                return Page();
            }

            _ifwDataService.FWRoot.storagefirewalls.Add(Firewall);
            _ifwDataService.SaveDataToBlob();
            return RedirectToPage("./Index");
        }
    }
}
