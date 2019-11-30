using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QEMUBackup.Pages.VM
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class BackupDetailsModel : PageModel
    {
        [BindProperty]
        public string BackupSelection { get; set; }
        [BindProperty]
        public string Host { get; set; }
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string BackupPath { get; set; }
        [BindProperty]
        public string VMName { get; set; }

        public string SSHOutput { get; set; }

        public JsonResult OnPost()
        {
            SSH ssh = new SSH(Host, Username, Password);

            var datedBackupPath = BackupPath + "/" + VMName + "/" + BackupSelection;
            string xmlDumpCommand = "cat " + "'" + datedBackupPath + "/" + VMName + ".xml" + "'";
            SSHOutput = ssh.ExecuteSSHCommand(xmlDumpCommand);

            return new JsonResult(SSHOutput);
        }
    }
}