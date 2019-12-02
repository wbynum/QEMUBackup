## QEMUBackup

### Description

QEMUBackup is a ASP.NET Core app that facilitates manual backups of QEMU virtual machines. Allows user to add a title and description to a virtual machine. Then user can create a backup which will contain the title and description in the VM's XML. Useful for describing the state of a VM when taking a backup.

### Usage

```
docker run -d \
    -p 43080:80 \
    --name <container name> \
    -e "QEMUBackupHost=<host running VMs>" \
    -e "QEMUBackupUsername=<host username>" \
    -e "QEMUBackupBackupPath=<filesystem path on host for VM backups>" \
    wbynum/qemubackup
```

Please replace all user variables in the above command defined by <> with the correct values.

### Access application

```
http://<host ip>:43080
```

### Example

```
docker run -d \
    -p 43080:80 \
    --name qemubackup \
    -e "QEMUBackupHost=192.168.1.10" \
    -e "QEMUBackupUsername=root" \
    -e "QEMUBackupBackupPath=/mnt/user/vmbackups" \
    wbynum/qemubackup
```

### Notes

- VM XML, vdisks and NVRAM file (if applicable) are backed up.
- All logic to update the VM title/description, retrieve location of VM disks for backup, update/create a VM from backup, etc is done with the appropriate virsh commands and QEMU VM XML inspection. No hard coded paths are used.
- App has been tested on a unRAID system. Should work on any Linux system running QEMU (untested).
