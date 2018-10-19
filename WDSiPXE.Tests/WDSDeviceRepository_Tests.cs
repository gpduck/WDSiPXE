using Xunit;
using WDSiPXE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.IO;

namespace WDSiPXE.Tests.Models {
  public class WDSDeviceRepository_Tests {
    private readonly IConfiguration validConfig;
    private readonly IConfiguration missingConfig;

    public WDSDeviceRepository_Tests() {
      validConfig = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "wds", "valid_wdssdc.json"))
        .Build();
      missingConfig = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "wds", "missing_wdssdc.json"))
        .Build();
    }

    [Fact]
    public void Ctr_MissingIniFile_ShouldThrow() {
      Assert.Throws<FileNotFoundException>(() => new WDSDeviceRepository(missingConfig));
    }

    [Theory]
    [InlineData("00-00-00-00-00-01","test")]
    [InlineData("00:00:00:00:00:02","test2")]
    public void GetDeviceById_ValidInputs_ReturnsCorrectResult(string id, string name) {
      WDSDeviceRepository repo = new WDSDeviceRepository(validConfig);
      Assert.Equal(name, repo.GetDeviceById(id)["WDS.Device.Name"]);
    }

    [Theory]
    [InlineData("12:34:56:78:90:12", "12-34-56-78-90-12")]
    [InlineData("12-34-56-78-90-12", "12-34-56-78-90-12")]
    //[InlineData("123456789012", "12-34-56-78-90-12")]  //not implemented yet
    public void NormalizeMacAddress(string input, string expected) {
      WDSDeviceRepository repo = new WDSDeviceRepository(validConfig);
      Assert.Equal(expected, repo.NormalizeMacAddress(input));
    }

    [Theory]
    [InlineData("ff-ff-ff-ff-ff-ff")]
    public void GetDeviceById_InvalidInputs_ShouldThrow(string id) {
      WDSDeviceRepository repo = new WDSDeviceRepository(validConfig);
      Assert.Throws<DeviceNotFoundException>(() => repo.GetDeviceById(id));
    }
  }
}