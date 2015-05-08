using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;

namespace WDSiPXE
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        private String _remoteInstallPath;

        public Bootstrapper() : this("c:\\temp30\\wds")
        {

        }

        public Bootstrapper(String remoteInstallPath)
        {
            _remoteInstallPath = remoteInstallPath;
        }

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            container.Register<IDeviceRepository, WDSDeviceRepository>(new WDSDeviceRepository(_remoteInstallPath));
            base.ApplicationStartup(container, pipelines);
        }
    }
}