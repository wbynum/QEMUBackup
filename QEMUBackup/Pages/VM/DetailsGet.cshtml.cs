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
    public class DetailsGetModel : PageModel
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
        public string VMName { get; set; }

        public string SSHOutput { get; set; }

        public JsonResult OnPost()
        {
            StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb);

            string vmUUID, vmName, vmTitle, vmDesc, vmInfo, vmXML;

            dynamic jObject = new JObject();
            SSH ssh = new SSH(Host, Username, Password);
            
            vmUUID = helper.GetVMUUID(ssh, VMName);
            if (vmUUID == null)
            {
                jObject.Error = true;
                jObject.ErrorMsg = "VM does not exist.";
                return new JsonResult(jObject.ToString());
            }

            vmName = helper.GetVMName(ssh, VMName);
            vmTitle = helper.GetVMTitle(ssh, VMName);
            vmDesc = helper.GetVMDesc(ssh, VMName);
            vmXML = helper.GetVMXMLString(ssh, VMName);
            vmInfo = helper.GetVMDomInfo(ssh, VMName);

            // vmDesc = vmDesc.Trim(new Char[] { ' ', '\r', '\n' });
            // vmTitle = vmTitle.Trim(new Char[] { ' ', '\r', '\n' });

            jObject = new JObject();
            jObject.vmUUID = vmUUID;
            jObject.vmName = vmName;
            jObject.vmTitle = vmTitle;
            jObject.vmDesc = vmDesc;
            jObject.vmXML = vmXML;
            jObject.vmInfo = vmInfo;

            return new JsonResult(jObject.ToString());
        }
    }
}