using Xunit;
using WDSiPXE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace WDSiPXE.Tests.Models {
  public class ADDeviceRepository_Tests {
    private readonly IConfiguration noadconfig;
    private readonly IConfiguration fulladconfig;

    public ADDeviceRepository_Tests() {
      noadconfig = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "ad", "no_adconfig.json"))
        .Build();
      fulladconfig = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "ad", "full_adconfig.json"))
        .Build();
    }

    [Theory]
    [InlineData("no_adconfig.json")]
    [InlineData("full_adconfig.json")]
    public void Ctr_ShouldSucceed(string ConfigFile) {
      var config = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "ad", ConfigFile))
        .Build();
      Assert.NotNull(new ADDeviceRepository(config));
    }
  }
}