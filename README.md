Current Status:  
[![Build status](https://ci.appveyor.com/api/projects/status/pgr63y6tlxgdsgdc?svg=true)](https://ci.appveyor.com/project/gpduck/wdsipxe)

WDSiPXE is a web application that generates customized answer files for OS builds over HTTP (in particular Linux and ESXi over [iPXE][iPXE]).  This is accomplished by merging templates with information from a device repository.  The currently supported device repositories are [WDS Standalone](#WDS) and [MDT Database](#MDT) ([Active Directory](#ActiveDirectory) support is planned).

# Quick Start
Download the latest Zip file from the releases page and extract to a web application in IIS.  The app pool needs to be running .Net 4.5 or above.

# Urls

`http://example.com/{TemplateFolder}/{DeviceID}?Template={TemplateName}`

<dl>
<dt>TemplateFolder</dt>
<dd>The folder under Views to locate the template in.</dd>

<dt>DeviceID</dt>
<dd>The value used to locate the device in the repository. Generally the device's MAC address, but check the documentation for the provider you plan on using to see what key values are suppored.</dd>

<dt>TemplateName</dt>
<dd>The name of the template file to merge for the device.  This file should be located at <code>/Views/{TemplateFolder}/{TemplateName}.cshtml</code></dd>
</dl>

`http://example.com/{TemplateFolder}/{DeviceID}?TemplateProperty={PropertyName}`

<dl>
<dt>TemplateFolder</dt>
<dd>The folder under Views to locate the template in.</dd>

<dt>DeviceID</dt>
<dd>The value used to locate the device in the repository. Generally the device's MAC address, but check the documentation for the provider you plan on using to see what key values are suppored.</dd>

<dt>PropertyName</dt>
<dd>The name of a property in the device repository that will have the value of the template name.  The device will be located by the <code>DeviceID</code>, then the template will be located by the device's <code>PropertyName</code> value. The template will be located at <code>/Views/{TemplateFolder}/{Device.PropertyName}.cshtml</code></dd>

</dl>

# Device Repositories

<a id="ActiveDirectory"></a>
## Active Directory

Active Directory support is planned for a future release.

<a id="MDT"></a>
## MDT Database

#### AppSettings

| Name | Mandatory | Default | Value |
| ---- | --------- | ------- | ----- |
| DeviceRepository | _True_ | | "MDT" |

#### ConnectionStrings

| Name | Mandatory | Default | Value |
| ---- | --------- | ------- | ----- |
| MDT | _True_ | | The SQL connection string to your MDT database. You can provide a username/password for SQL authentication or use the AppPool account with Windows authentication. |

### Device Lookup

Device lookup is currently hard coded to use the MacAddress field in the MDT database.  Use ${net0/mac} in your [iPXE][iPXE] script or DHCP option 67 to pass the device's MAC address to you WDSiPXE site.

<a id="WDS"></a>
## WDS (Standalone)

#### AppSettings

| Name | Mandatory | Default | Value |
| ---- | --------- | ------- | ----- |
| DeviceRepository | _True_ | | "WDS" |
| WDS.RemoteInstallPath | _False_ | (registry) | The path to the RemoteInst folder of WDS. If this is a network path the AppPool user will be used to access the remote share.<br />Ex: c:\RemoteInstall|

[iPXE]:http://ipxe.org/