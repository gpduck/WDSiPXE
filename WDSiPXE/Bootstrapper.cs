using System;
using System.Configuration;
using Nancy;
using System.Collections.Specialized;
using System.IO;

namespace WDSiPXE
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {

            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            switch (appSettings["DeviceRepository"]) {
                case "WDS":
                    WDSDeviceRepository wds = null;
                    if (String.IsNullOrEmpty(appSettings["WDS.RemoteInstallPath"]))
                    {
                        wds = new WDSDeviceRepository();
                        
                    }
                    else
                    {
                        string remoteInstallPath = appSettings["WDS.RemoteInstallPath"];
                        if (!Path.IsPathRooted(remoteInstallPath))
                        {
                            //Resolve relative paths from application root.
                            remoteInstallPath = Path.Combine(this.RootPathProvider.GetRootPath(), remoteInstallPath);
                        }
                        wds = new WDSDeviceRepository(remoteInstallPath);
                    }
                    container.Register<IDeviceRepository, WDSDeviceRepository>(wds);
                    break;
                case "AD":
                    container.Register<IDeviceRepository, ADDeviceRepository>(new ADDeviceRepository());
                    break;
                case "MDT":
                    container.Register<IDeviceRepository, MDTDeviceRepository>(new MDTDeviceRepository());
                    break;
                default:
                    throw new ConfigurationErrorsException("Please add a valid 'DeviceRepository' AppSetting value to the applications configuration file.");
            }
            
            base.ApplicationStartup(container, pipelines);
        }
    }
}