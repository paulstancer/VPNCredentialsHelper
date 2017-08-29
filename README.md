# VPN Credentials Helper PowerShell Module
This repository contains the code used to build the PowerShell helper module: [VPNCredentialsHelper](https://www.powershellgallery.com/packages/VPNCredentialsHelper).

The module can set a username and password directly for a named VPN connection, so that you are not prompted to enter it the first time you connect.

To install the module enter the following PowerShell command.

 ```PowerShell
Install-Module -Name VPNCredentialsHelper
 ```

This will add the **Set-VpnConnectionUsernamePassword** as a PowerShell command.

And then you can script something like this:

 ```PowerShell
$name = "ExpressVPN Australia Sydney"
$address = "aus1-ubuntu-l2tp.expressprovider.com"
$username = "your_username"
$plainpassword = "your_password"
 
Add-VpnConnection -Name $name -ServerAddress $address -TunnelType L2tp -EncryptionLevel Required -AuthenticationMethod MSChapv2 -L2tpPsk "12345678" -Force:$true -RememberCredential:$true -SplitTunneling:$false 
 
Set-VpnConnectionUsernamePassword -connectionname $name -username $username -password $plainpassword -domain ''
 ```
### Security
Please note: you will have to set your policy to permit unsigned PowerShell scripts to execute, to run this command.

If you're nervous about doing this, the actual script source code can be found [here](https://www.powershellgallery.com/packages/VPNCredentialsHelper/1.1/Content/VPNCredentialsHelper.psm1).

## Thanks
A huge thanks to Jeff Winn for the DotRas project (https://dotras.codeplex.com/) which showed me the way, and who did all the really hard work.
___
[Paul Stancer](https://github.com/paulstancer)
