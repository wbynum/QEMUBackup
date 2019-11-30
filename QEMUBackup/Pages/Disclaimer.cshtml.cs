using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace QEMUBackup.Pages
{
    public class DisclaimerModel : PageModel
    {
        private readonly ILogger<DisclaimerModel> _logger;

        public DisclaimerModel(ILogger<DisclaimerModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
