using System;
using Xunit;
using WDSiPXE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.DirectoryServices;
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

    public static IConfiguration LoadConfigFile(string Name) {
      var config = new ConfigurationBuilder()
        .AddJsonFile(Path.Combine(".", "testdata", "ad", Name))
        .Build();
      return config;
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

    [Theory]
    [InlineData("LDAP://test.local", "OU=searchbase", "LDAP://test.local/OU=searchbase")]
    [InlineData("LDAP://test.local", "", "LDAP://test.local")]
    public void BuildDirectorySearcher_SearchRoot_ReturnsCorrectResult(string ConnectionString, string SearchBase, string ExpectedSearchRoot) {
      var config = new ConfigurationBuilder().Build();
      var repo = new ADDeviceRepository(config);

      ADDomainConfig adconfig = new ADDomainConfig() {
        ConnectionString = ConnectionString,
        SearchBase = SearchBase
      };
      DirectorySearcher searcher = repo.BuildDirectorySearcher(adconfig, "12-34-56-78-90-23");
      Assert.Equal<string>(ExpectedSearchRoot, searcher.SearchRoot.Path);
    }

    [Theory]
    [InlineData("12-34-56-78-90-12")]
    [InlineData("00-00-00-00-00-00")]
    public void BuildDirectorySearcher_Filter_ReturnsDefaultFilter(string Id) {
      var config = new ConfigurationBuilder().Build();
      var repo = new ADDeviceRepository(config);

      ADDomainConfig adconfig = new ADDomainConfig();
      DirectorySearcher searcher = repo.BuildDirectorySearcher(adconfig, Id);
      Assert.Equal<string>(String.Format(ADDeviceRepository.DEFAULT_FILTER, Id), searcher.Filter);
    }

    [Theory]
    [InlineData("12-34-56-78-90-12")]
    [InlineData("00-00-00-00-00-00")]
    public void BuildDirectorySearcher_Filter_ReturnsConfiguredFilter(string Id) {
      var config = new ConfigurationBuilder().Build();
      var repo = new ADDeviceRepository(config);

      ADDomainConfig adconfig = new ADDomainConfig() {
        Filter = "text{0}text"
      };
      DirectorySearcher searcher = repo.BuildDirectorySearcher(adconfig, Id);
      Assert.Equal<string>(String.Format("text{0}text", Id), searcher.Filter);
    }
  }
}