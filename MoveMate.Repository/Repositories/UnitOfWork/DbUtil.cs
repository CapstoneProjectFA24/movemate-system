using Microsoft.Extensions.Configuration;

namespace MoveMate.API.Utils;

public class DbUtil
{
    public static String getConnectString()
    {
        var cnn = Environment.GetEnvironmentVariable("MDB");
        var finalConnectionString = "";

        if (!string.IsNullOrEmpty(cnn))
        {
            finalConnectionString = cnn;
        }
        else
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            finalConnectionString = configuration.GetConnectionString("MyDB");
        }

        return finalConnectionString;
    }
}