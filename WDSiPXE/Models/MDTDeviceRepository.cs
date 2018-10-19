using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;
using IniParser;
using IniParser.Model;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;

namespace WDSiPXE.Models
{
    public class MDTDeviceRepository : IDeviceRepository
    {
        private String _connectionString;
        private DbProviderFactory _dbFactory;

        public MDTDeviceRepository(IConfiguration config, DbProviderFactory dbProviderFactory) {
            _connectionString = config.GetConnectionString("MDT");
            _dbFactory = dbProviderFactory;
         }

        public String MacToMDTFormat(string id)
        {
            PhysicalAddress macAddress = PhysicalAddress.Parse(id.Replace(" ", "").Replace(":", ""));
            return String.Join<String>(":", macAddress.GetAddressBytes().ToList<byte>().ConvertAll<String>(
                x => String.Format("{0:X2}", x)
            ));
        }

        public Device GetDeviceById(string id)
        {
            using (DbConnection connection = _dbFactory.CreateConnection())
            {
                connection.ConnectionString = this._connectionString;
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"
                        SELECT * 
                            FROM ComputerIdentity LEFT JOIN Settings ON ComputerIdentity.ID = Settings.ID
                            WHERE Settings.Type = 'C' AND ComputerIdentity.MacAddress = @MacAddress
                            ORDER BY ComputerIdentity.ID DESC;
                    ";

                    DbParameter p = _dbFactory.CreateParameter();
                    p.ParameterName = "MacAddress";
                    try
                    {
                        p.Value = MacToMDTFormat(id);
                    }
                    catch
                    {
                        p.Value = id;
                    }
                    command.Parameters.Add(p);

                    connection.Open();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Device d = new Device();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string key = reader.GetName(i);
                                object value = reader.GetValue(i);
                                d[key] = value;
                            }
                            connection.Close();
                            return d;
                        }
                        else
                        {
                            throw new DeviceNotFoundException(String.Format("Unable to locate a device with id '{0}'", id));
                        }
                    }
                }
            }
        }
    }
}