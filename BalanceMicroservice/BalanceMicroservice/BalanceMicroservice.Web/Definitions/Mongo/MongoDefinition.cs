﻿using BalanceMicroservice.Web.Definitions.Base;
using BalanceMicroservice.Web.MongoService;
using BalanceMicroservice.Web.MongoService.ViewModels;

namespace BalanceMicroservice.Web.Definitions.Mongo
{
    public class MongoDefinition : AppDefinition
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BalanceStoreDatabaseSettings>(configuration.GetSection("BalanceStoreDatabase"));
            services.AddSingleton<MongoBalanceService>();
        }
    }
}
