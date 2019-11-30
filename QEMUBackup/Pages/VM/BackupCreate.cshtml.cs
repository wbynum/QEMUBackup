using System;
using System.Collections.Generic;
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
    public class BackupCreateModel : PageModel
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
            SSH ssh = new SSH(Host, Username, Password);

            string vmUUID, vmState;

            dynamic jObject = new JObject();
            jObject.Error = false;

            //
            // Get the VM info
            //
            vmUUID = helper.GetVMUUID(ssh, VMName);
            if (vmUUID == null)
            {
                jObject.Error = true;
                jObject.ErrorMsg = "VM does not exist.";
                return new JsonResult(jObject.ToString());
            }

            vmState = helper.GetVMState(ssh, VMName);

            sb.AppendLine("VM Name: " + VMName);
            sb.AppendLine("VM UUID: " + vmUUID);
            sb.AppendLine("VM State: " + vmState);

            //
            // Make sure the VM is in the "shut off" state
            //
            if (vmState != "shut off")
            {
                jObject.Error = true;
                jObject.ErrorMsg = "VM not in \"shut off\" state.";
                return new JsonResult(jObject.ToString());
            }
            else
            {
                //
                // Dump the VM XML into the backup location
                //
                var serverDate = helper.GetVMServerDate(ssh);
                var datedBackupPath = BackupPath + "/" + VMName + "/" + serverDate + "/";
                helper.MakeVMServerDirectory(ssh, datedBackupPath);
                helper.DumpVMXMLToFile(ssh, VMName, datedBackupPath);

                //
                // Get the list of vdisks associated with the VM and back them up
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

                    helper.Rsync(ssh, diskPath, vdiskBackupPath);
                }

                //
                // Backup the ovmf nvram if it exists
                //
                XmlNode nvramPath = xmlDoc.SelectSingleNode("/domain/os/nvram");
                if (nvramPath != null)
                {
                    var diskPath = nvramPath.InnerText;
                    helper.Rsync(ssh, diskPath, datedBackupPath);
                }
            }

            jObject.backupOutput = sb.ToString();
            return new JsonResult(jObject.ToString());
        }
    }
}