# CrossCert Config System

CrossCert uses a modular JSON config file to control behavior. Users may edit this file manually or through the GUI.

## 🧾 Example Config

```json
{
  "httpConfigBackup": {
    "enabled": true,
    "backupDirectory": "C:\\CrossCert\\Backups\\HTTP",
    "retainCount": 5
  },
  "retryPolicy": {
    "maxAttempts": 3,
    "initialDelaySeconds": 10,
    "backoffMultiplier": 2
  }
}
