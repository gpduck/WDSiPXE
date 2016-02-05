using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using System.Web.Razor;


/*
 * /ipxe/00-11-22-33-44-55?TemplateProperty=OperatingSystem
 * /ks/00-11-22-33-44-55?Template=RH56
 */

namespace WDSiPXE
{
    public class DeviceTemplateModule : NancyModule
    {
        private IDeviceRepository _repository;

        public DeviceTemplateModule(IDeviceRepository repository)
        {
            _repository = repository;
            Get["/{TemplateFolder}/{DeviceId}"] = parameters =>
                {
                    String templateFolder = parameters.TemplateFolder;
                    String deviceId = parameters.DeviceId;

                    Device device = _repository.GetDeviceById(deviceId);

                    String view = templateFolder;

                    if (this.Request.Query["Template"])
                    {
                        view += "/" + this.Request.Query["Template"];
                    }
                    else if (this.Request.Query["TemplateProperty"])
                    {
                        String templateProperty = this.Request.Query["TemplateProperty"].ToString();
                        if (!device.ContainsKey(templateProperty))
                        {
                            String error = "The device did not contain a value for " + templateProperty;
                            return Negotiate
                                .WithModel(device)
                                .WithStatusCode(HttpStatusCode.InternalServerError)
                                .WithReasonPhrase(error);
                                
                        }
                        view += "/" + device[templateProperty];
                    }
                    view += ".cshtml";

                    Uri uri = this.Request.Url;
                    String baseUrl = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).ToString() + System.Web.HttpRuntime.AppDomainAppVirtualPath;
                    this.ViewBag.BaseUrl = baseUrl;

                    return Negotiate
                        .WithStatusCode(HttpStatusCode.OK)
                        .WithModel(device)
                        .WithView(view);
                };
        }
    }
}