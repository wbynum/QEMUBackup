using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace QEMUBackup.Pages.VM
{
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class BackupRestoreModel : PageModel
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
            StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb);
            SSH ssh = new SSH(Host, Username, Password);

            string vmUUID = null;
            string vmState = null;

            dynamic jObject = new JObject();
            jObject.Error = false;

            //
            // Get the VM info
            //
            vmUUID = helper.GetVMUUID(ssh, VMName);
            vmState = helper.GetVMState(ssh, VMName);

            sb.AppendLine("VM Name: " + VMName);
            sb.AppendLine("VM UUID: " + vmUUID);
            sb.AppendLine("VM State: " + vmState);

            //
            // Make sure the VM doesn't exist or is in the "shut off" state
            //
            if (vmState != null && vmState != "shut off")
            {
                jObject.restoreOutput = "VM not in \"shut off\" state.";
                return new JsonResult(jObject.ToString());
            }
            else
            {
                //
                // Import the XML
                //
                var datedBackupPath = BackupPath + "/" + VMName + "/" + BackupSelection + "/";
                helper.ImportVMXML(ssh, VMName, datedBackupPath);

                //
                // Copy over the disks
                //
                var vdiskBackupPath = datedBackupPath + "vdisks/";
                sb.AppendLine("VM VDISK Backup Path: " + vdiskBackupPath);

                XmlDocument xmlDoc = helper.GetVMXML(ssh, VMName);
                XmlNodeList diskList = xmlDoc.SelectNodes("/domain/devices/disk[@device='disk']");
                foreach (XmlNode disk in diskList)
                {
                    XmlNode diskSource = disk.SelectSingleNode("source");
                    XmlAttribute diskSourceFile = diskSource.Attributes["file"];
                    var diskPath = diskSourceFile.Value;
                    var filename = diskPath.Split('/').Last();

                    helper.Rsync(ssh, vdiskBackupPath + filename, diskPath.Substring(0, diskPath.LastIndexOf("/") + 1));
                }

                //
                // Copy over the nvram
                //
                XmlNode nvramPath = xmlDoc.SelectSingleNode("/domain/os/nvram");
                if (nvramPath != null)
                {
                    var diskPath = nvramPath.InnerText;
                    var filename = diskPath.Split('/').Last();

                    helper.Rsync(ssh, datedBackupPath + filename, diskPath.Substring(0, diskPath.LastIndexOf("/") + 1));
                }

                jObject.restoreOutput = sb.ToString();
                return new JsonResult(jObject.ToString());
            }
        }
    }
}