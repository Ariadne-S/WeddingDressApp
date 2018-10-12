using Autofac;
using System.Data;
using System.Data.SqlClient;

namespace Website
{
    internal class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var connectionString = "Server=localhost\\SQLEXPRESS; Database=WeddingDressApp; Trusted_connection=true";
            builder
                .Register(x => {
                    var connection = new SqlConnection(connectionString);
                    connection.Open();
                    return connection;
                })
                .As<IDbConnection>()
                .InstancePerLifetimeScope();

            builder
                .Register(x => {
                    var dbConnection = x.Resolve<IDbConnection>();
                    return dbConnection.BeginTransaction();
                })
                .As<IDbTransaction>()
                .InstancePerLifetimeScope();
            
        }
    }
}