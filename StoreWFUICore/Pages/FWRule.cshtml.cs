using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreWFUICore.Data;

namespace StoreWFUICore.Pages
{
    public class FWRuleModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFWDataService _ifwDataService;
       
        [BindProperty]
        public Storagefirewall Firewall { get; set; } = default!;

        public Root FWRoot { get; set; }
        public FWRuleModel(ILogger<IndexModel> logger, IFWDataService ifwDataService)
        {
            _logger = logger;
            _ifwDataService = ifwDataService;
            FWRoot = _ifwDataService.FWRoot;
        }

        public void OnGet(int id, string name)
        {
            if (string.IsNullOrEmpty(name) == false)
            {
                Firewall = FWRoot.storagefirewalls.Find(f => f.GetKey() == name);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (FWRoot.storagefirewalls.Exists(f => f.GetKey() == Firewall.GetKey()))
            {
                Storagefirewall existing = FWRoot.storagefirewalls.Find(f => f.GetKey() == Firewall.GetKey());
                existing.firewallIPs = Firewall.firewallIPs;
                existing.isPrivate = Firewall.isPrivate;
            }
            _ifwDataService.SaveDataToBlob();
            return RedirectToPage("./Index");
        }
    }
}
