using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WDSiPXE.Models
{
  public class ADDomainConfig {
    public String ConnectionString { get; set; }
    public String SearchBase { get; set; }
    public String Filter { get; set; }
  }
}