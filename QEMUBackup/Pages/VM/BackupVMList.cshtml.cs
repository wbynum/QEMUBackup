using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace QEMUBackup.Pages.VM
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class BackupVMListModel : PageModel
    {
        [BindProperty]
        public string MainSelection { get; set; }
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
            System.Text.StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb, false);
            JArray backups = new JArray();

            SSH ssh = new SSH(Host, Username, Password);

            var backupDirs = helper.GetDirNames(ssh, BackupPath + "/");
            sb.AppendLine("---------------------");
            sb.AppendLine("VM LIST WITH BACKUPS:");
            sb.AppendLine("---------------------");

            foreach (string vm in backupDirs)
            {
                sb.AppendLine(vm);
            }

            return new JsonResult(sb.ToString());
        }
    }
}