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
    public class DetailsSetModel : PageModel
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
        [BindProperty]
        public string VMTitle { get; set; }
        [BindProperty]
        public string VMDesc { get; set; }

        public string SSHOutput { get; set; }

        public JsonResult OnPost()
        {
            StringBuilder sb = new StringBuilder();
            Helper helper = new Helper(sb);

            string vmUUID;

            string vmTitleOutput = "";
            string vmDescOutput = "";

            dynamic jObject = new JObject();
            SSH ssh = new SSH(Host, Username, Password);

            vmUUID = helper.GetVMUUID(ssh, VMName);
            if (vmUUID == null)
            {
                jObject.Error = true;
                jObject.ErrorMsg = "VM does not exist.";
                return new JsonResult(jObject.ToString());
            }

            if (String.IsNullOrEmpty(VMTitle))
            {
                VMTitle = "";
            }

            VMTitle = VMTitle.Trim(new Char[] { ' ', '\r', '\n' });
            vmTitleOutput = helper.SetVMTitle(ssh, VMName, VMTitle);


            if (String.IsNullOrEmpty(VMDesc))
            {
                VMDesc = "";
            }

            VMDesc = VMDesc.Trim(new Char[] { ' ', '\r', '\n' });
            vmDescOutput = helper.SetVMDesc(ssh, VMName, VMDesc);
             
            jObject.vmTitleOutput = vmTitleOutput;
            jObject.vmDescOutput = vmDescOutput;

            return new JsonResult(jObject.ToString());
        }
    }
}