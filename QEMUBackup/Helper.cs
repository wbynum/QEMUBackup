using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace QEMUBackup
{
    public class Helper
    {
        StringBuilder mySB;
        bool appendToSB = true;

        public Helper(StringBuilder sb)
        {
            mySB = sb;
        }

        public Helper(StringBuilder sb, bool useSB)
        {
            mySB = sb;
            appendToSB = useSB;
        }

        public XmlDocument GetVMXML(SSH ssh, string vmName)
        {
            XmlDocument xmlDoc = new XmlDocument();

            var xmlString = GetVMXMLString(ssh, vmName);
            xmlString = xmlString.Trim(new Char[] { ' ', '\r', '\n' });

            if (xmlString.Length > 0)
            {
                xmlDoc.LoadXml(xmlString);
            }
            else
            {
                xmlDoc = null;
            }

            return xmlDoc;
        }

        public string GetVMXMLString(SSH ssh, string vmName)
        {
            string sshCommand = "virsh dumpxml \"" + vmName + "\"";
            mySB.AppendLine(sshCommand);
            return ssh.ExecuteSSHCommand(sshCommand);
        }

        public string GetVMName(SSH ssh, string vmName)
        {
            return GetNodeInnerText(ssh, vmName, "/domain/name");
        }

        public string GetVMUUID(SSH ssh, string vmName)
        {
            return GetNodeInnerText(ssh, vmName, "/domain/uuid");
        }

        public string GetVMTitle(SSH ssh, string vmName)
        {
            return GetNodeInnerText(ssh, vmName, "/domain/title");
        }

        public string GetVMDesc(SSH ssh, string vmName)
        {
            return GetNodeInnerText(ssh, vmName, "/domain/description");
        }

        public string GetVMDomInfo(SSH ssh, string vmName)
        {
            string sshCommand = "virsh dominfo \"" + vmName + "\"";
            return ssh.ExecuteSSHCommand(sshCommand);
        }

        public string GetVMState(SSH ssh, string vmName)
        {
            string vmState = null;

            var sshOutput = ssh.ExecuteSSHCommand("virsh dominfo \"" + vmName + "\"");
            string[] vmInfoArray = sshOutput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string temp in vmInfoArray)
            {
                if (temp.StartsWith("State:"))
                {
                    vmState = temp.Substring(16);
                }
            }

            return vmState;
        }

        public string GetVMServerDate(SSH ssh)
        {
            string date;

            string sshCommand = "date +%s";
            mySB.AppendLine(sshCommand);
            date = ssh.ExecuteSSHCommand(sshCommand);
            date = date.ToString().Replace("\r\n", "");
            date = date.ToString().Replace("\r", "");
            date = date.ToString().Replace("\n", "");

            return date;
        }

        public void MakeVMServerDirectory(SSH ssh, string dirPath)
        {
            string sshCommand = "mkdir -p " + "'" + dirPath + "'";
            mySB.AppendLine(sshCommand);
            ssh.ExecuteSSHCommand(sshCommand);
        }

        public void DumpVMXMLToFile(SSH ssh, string vmName, string dirPath)
        {
            string sshCommand = "virsh dumpxml \"" + vmName + "\"" + " > " + "'" + dirPath + vmName + ".xml" + "'";
            mySB.AppendLine(sshCommand);
            ssh.ExecuteSSHCommand(sshCommand);
        }

        //
        // This method will create a VM if it does not exist. If it exists then it will
        // overwrite the VM's XML config.
        //
        public void ImportVMXML(SSH ssh, string vmName, string dirPath)
        {
            string sshCommand = "virsh define " + "'" + dirPath + vmName + ".xml" + "'";
            mySB.AppendLine(sshCommand);
            ssh.ExecuteSSHCommand(sshCommand);
        }

        public void Rsync(SSH ssh, string source, string dest)
        {
            string sshCommand = "rsync -a " + "'" + source + "'" + " " + "'" + dest + "'";
            mySB.AppendLine(sshCommand);
            ssh.ExecuteSSHCommand(sshCommand);
        }

        public List<string> GetDirNames(SSH ssh, string dirPath)
        {
            List<string> returnList = new List<string>();

            dirPath = dirPath.Replace(" ", "\\ ");
            string sshCommand = "ls -d " + dirPath + "*";
            if (appendToSB == true)
                mySB.AppendLine(sshCommand);
            string sshOutput = ssh.ExecuteSSHCommand(sshCommand);

            if (sshOutput.Length > 0)
            {
                var backupDirs = sshOutput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var backupDir in backupDirs)
                {
                    if (backupDir.StartsWith("/"))
                    {
                        var dirSplit = backupDir.Split(new[] { "/" }, StringSplitOptions.None);
                        returnList.Add(dirSplit.Last().Trim(new Char[] { ' ', '\r', '\n' }));
                    }
                }
            }

            return returnList;
        }

        public string SetVMTitle(SSH ssh, string vmName, string vmTitle)
        {
            string vmTitleOutput01, vmTitleOutput02;

            string sshCommand = "virsh desc '" + vmName + "' --config --live --title '" + vmTitle + "'";
            vmTitleOutput01 = ssh.ExecuteSSHCommand(sshCommand);

            sshCommand = "virsh desc '" + vmName + "' --current --title '" + vmTitle + "'";
            vmTitleOutput02 = ssh.ExecuteSSHCommand(sshCommand);

            return vmTitleOutput01 + "\n" + vmTitleOutput02;
        }

        public string SetVMDesc(SSH ssh, string vmName, string vmDesc)
        {
            string vmDescOutput01, vmDescOutput02;

            string sshCommand = "virsh desc '" + vmName + "' --config --live --new-desc '" + vmDesc + "'";
            vmDescOutput01 = ssh.ExecuteSSHCommand(sshCommand);

            sshCommand = "virsh desc '" + vmName + "' --current --new-desc '" + vmDesc + "'";
            vmDescOutput02 = ssh.ExecuteSSHCommand(sshCommand);

            return vmDescOutput01 + "\n" + vmDescOutput02;
        }

        private string GetNodeInnerText(SSH ssh, string vmName, string xpath)
        {
            XmlDocument xmlDoc = null;

            xmlDoc = GetVMXML(ssh, vmName);
            if (xmlDoc != null)
            {
                XmlNode node = xmlDoc.SelectSingleNode(xpath);
                if (node != null)
                {
                    return node.InnerText;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
