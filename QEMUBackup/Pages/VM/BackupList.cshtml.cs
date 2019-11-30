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
    public class BackupListModel : PageModel
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
            StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb);
            JArray backups = new JArray();

            SSH ssh = new SSH(Host, Username, Password);

            var vmBackupPath = BackupPath + "/" + VMName + "/";
            var backupDirs = helper.GetDirNames(ssh, vmBackupPath);

            foreach (var backupDir in backupDirs)
            {
                var epochTime = backupDir;
                if (epochTime.Length > 0)
                {
                    dynamic backupInfo = new JObject();
                    backupInfo.Date = epochTime;

                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var backupTime = epoch.AddSeconds(Int32.Parse(epochTime));
                    backupInfo.DatePretty = backupTime.ToString();

                    backups.Add(backupInfo);
                }
            }

            return new JsonResult(backups.ToString());
        }
    }
}