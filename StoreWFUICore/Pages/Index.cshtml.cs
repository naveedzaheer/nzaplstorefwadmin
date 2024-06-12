using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreWFUICore.Data;

namespace StoreWFUICore.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IFWDataService _ifwDataService;

        public Root FWRoot { get; set;}
        public IndexModel(ILogger<IndexModel> logger, IFWDataService ifwDataService)
        {
            _logger = logger;
            _ifwDataService = ifwDataService;
            FWRoot = _ifwDataService.FWRoot;
        }

        public void OnGet()
        {

        }
    }
}
