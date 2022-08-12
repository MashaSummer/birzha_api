﻿using Calabonga.OperationResults;
using PortfolioGrpc;
using PortfolioServiceGrpc;
using ProductGrpc;

namespace Facade.Web.GrpcServices.Portfolio.Aggregation
{
    public class PortfolioAggregator
    {
        public static GetPortfolioResponse AggregateProducts(GetPortfolioResponse portfolioResponse, 
            Google.Protobuf.Collections.RepeatedField<AssetArray.Types.Asset> assetsArray,
            Google.Protobuf.Collections.RepeatedField<ProductArray.Types.Product> productsArray)
        {
            double estimate, spent, earned, delta_abs, delta_rel;
            for (int i = 0; i < assetsArray.Count(); i++)
            {
                var product = productsArray.Where(p => p.Id == assetsArray[i].Id).First();
                var asset = assetsArray[i];
                estimate = asset.VolumeActive * product.BestAsk;
                spent = 50;
                earned = 50;
                delta_abs = estimate - spent;
                delta_rel = (estimate - spent) / spent;
                portfolioResponse.Portfolio.Products.Add(new PortfolioServiceGrpc.Portfolio.Types.Product
                {
                    Id = asset.Id,
                    Name = product.Name,
                    BestAsk = product.BestAsk,
                    Spent = spent,
                    Earned = earned,
                    Volume = asset.VolumeActive,
                    Estimate = estimate,
                    DeltaAbs = delta_abs,
                    DeltaRel = delta_rel
                });
            }

            return portfolioResponse;
        }

        public static GetPortfolioResponse AggregateTotal(GetPortfolioResponse portfolioResponse,
            Google.Protobuf.Collections.RepeatedField<PortfolioServiceGrpc.Portfolio.Types.Product> portfolio)
        {
            portfolioResponse.Portfolio.Total = new PortfolioServiceGrpc.Portfolio.Types.Total
            {
                Spent = portfolio.Sum(sp => sp.Spent),
                Earned = portfolio.Sum(er => er.Earned),
                Estimate = portfolio.Sum(est => est.Estimate),
                DeltaAbs = portfolio.Sum(da => da.DeltaAbs),
                DeltaRel = portfolio.Sum(dr => dr.DeltaRel)
            };

            return portfolioResponse;
        }
    }
}
