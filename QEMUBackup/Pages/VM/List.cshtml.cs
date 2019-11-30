using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QEMUBackup.Pages.VM
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ListModel : PageModel
    {
        [BindProperty]
        public string MainSelection { get; set; }
        [BindProperty]
        public string Host { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public string SSHOutput { get; set; }

        public JsonResult OnPost()
        {
            SSH ssh = new SSH(Host, Username, Password);
            SSHOutput = ssh.ExecuteSSHCommand("virsh list --all --title");

            return new JsonResult(SSHOutput);
        }
    }
}