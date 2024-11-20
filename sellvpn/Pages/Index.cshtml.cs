using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace sellvpn.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
        public async Task<JsonResult> OnGetPay(int id)
        {

            var res = await new NovinoPay().CreateTransaction(1000,"01","des","","","name");

            return new JsonResult(res);
        }
    }
}
