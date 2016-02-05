using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WDSiPXE
{
    public interface IDeviceRepository
    {
        Device GetDeviceById(String id);
    }
}