using Autofac;
using System.Data;
using System.Data.SqlClient;

namespace Website
{
    internal class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = "Server=LIMITING-FACTOR\\SQLEXPRESS2017; Database=WeddingDressApp; Trusted_connection=true";
            builder
                .Register(x =>
                {
                    var connection = new SqlConnection(connectionString);
                    connection.Open();
                    return connection;
                })
                .As<IDbConnection>()
                .InstancePerLifetimeScope();
        }
    }
}