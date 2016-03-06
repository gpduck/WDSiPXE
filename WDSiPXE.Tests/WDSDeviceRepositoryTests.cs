using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using WDSiPXE;

namespace WDSiPXE.Tests
{
    [TestClass]
    public class WDSDeviceRepositoryTests : BaseDeviceRepository
    {
        protected override System.Collections.Generic.IList<System.Collections.Generic.KeyValuePair<string,object>> TestValues
        {
	        get {
                return new List<KeyValuePair<String, Object>>(
                    new KeyValuePair<String, Object>[] {
                        new KeyValuePair<String, Object>("WDS.Device.Name", "test"),
                        new KeyValuePair<String, Object>("WDS.Device.ID", "[00-00-00-00-00-01]")
                    }
                );
            }
        }

        protected override IDeviceRepository GetRepository()
        {
            return new WDSDeviceRepository(Directory.GetCurrentDirectory());
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void WDSSDCDoesNotExist()
        {
            WDSDeviceRepository wds = new WDSDeviceRepository("c:\\pathdoesnotexist");
            wds.GetDeviceById("234");
        }
    }
}
