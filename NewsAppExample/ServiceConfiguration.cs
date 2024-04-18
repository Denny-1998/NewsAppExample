using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NewsAppExample.Helper;

public static class ServiceConfiguration
{
    

    public static string GetConnectionString()
    {
        return ConfigReader.getConnectionString();
    }
}
