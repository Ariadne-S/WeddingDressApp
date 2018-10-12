using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Website.Filters
{
    public class TranscationHandler : IAsyncActionFilter
    {
        private readonly IDbConnection dbConnection;
        private readonly IDbTransaction dbTransaction;

        public TranscationHandler(IDbConnection dbConnection, IDbTransaction dbTransaction)
        {
            this.dbConnection = dbConnection;
            this.dbTransaction = dbTransaction;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (dbTransaction) {
                await next();
                dbTransaction.Commit();
            }
        }
    }
}
