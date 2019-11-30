using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QEMUBackup
{
    public class SSH
    {
        private ConnectionInfo connectionInfo;

        public SSH(string host, string username, string password)
        {
            connectionInfo = new ConnectionInfo(host, username, new PasswordAuthenticationMethod(username, password));
        }

        public string ExecuteSSHCommand(string remoteCommand)
        {
            string result;
            using (var ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                var command = ssh.CreateCommand(remoteCommand);
                result = command.Execute();
                ssh.Disconnect();
            }

            return result;
        }
    }
}
