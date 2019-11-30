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
    public class DetailsXMLModel : PageModel
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

        public JsonResult OnPost()
        {
            StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb);

            string vmUUID, vmXML;

            dynamic jObject = new JObject();
            SSH ssh = new SSH(Host, Username, Password);

            vmUUID = helper.GetVMUUID(ssh, VMName);
            if (vmUUID == null)
            {
                jObject.Error = true;
                jObject.ErrorMsg = "VM does not exist.";
                return new JsonResult(jObject.ToString());
            }

            vmXML = helper.GetVMXMLString(ssh, VMName);

            jObject = new JObject();
            jObject.vmXML = vmXML;

            return new JsonResult(jObject.ToString());
        }
    }
}