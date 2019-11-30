using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace QEMUBackup.Pages
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class IndexModel : PageModel
    {
        public string MainSelection { get; set; }
        public string[] MainSelectionValues = new[] { "List Virtual Machines", "Get Virtual Machine Details", "Set Virtual Machine Details", "Get Virtual Machine XML", "Backup Virtual Machine", "List Virtual Machine Backups" };

        public string Host { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string BackupPath { get; private set; }


        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Host = "";
            Username = "";
            Password = "";
            BackupPath = "";
        }
    }
}
