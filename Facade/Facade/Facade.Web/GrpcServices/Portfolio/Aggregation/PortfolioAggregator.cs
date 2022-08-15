using Calabonga.OperationResults;
using PortfolioGrpc;
using PortfolioServiceGrpc;
using ProductGrpc;
using Orders;

namespace Facade.Web.GrpcServices.Portfolio.Aggregation
{
    public class PortfolioAggregator
    {
        public static GetPortfolioResponse AggregateProducts(GetPortfolioResponse portfolioResponse, 
            Google.Protobuf.Collections.RepeatedField<AssetArray.Types.Asset> assetsArray,
            Google.Protobuf.Collections.RepeatedField<ProductArray.Types.Product> productsArray,
            Google.Protobuf.Collections.RepeatedField<UserProductInfo> userProductInfo)
        {
            double estimate, spent, earned, delta_abs, delta_rel;
            for (int i = 0; i < assetsArray.Count(); i++)
            {
                var asset = assetsArray[i];
                var product = productsArray.Where(p => p.Id == asset.Id).First();
                var ordersPerProduct = userProductInfo.Where(upi => upi.ProductId == asset.Id).First();
                
                estimate = asset.VolumeActive * ordersPerProduct.BestAsk;
                spent = ordersPerProduct.Spent;
                earned = ordersPerProduct.Earned;
                delta_abs = estimate - spent;
                delta_rel = (estimate - spent) / spent;
                portfolioResponse.Portfolio.Products.Add(new PortfolioServiceGrpc.Portfolio.Types.Product
                {
                    Id = asset.Id,
                    Name = product.Name,
                    BestAsk = ordersPerProduct.BestAsk,
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
                Spent = portfolio.Sum(product => product.Spent),
                Earned = portfolio.Sum(product => product.Earned),
                Estimate = portfolio.Sum(product => product.Estimate),
                DeltaAbs = portfolio.Sum(product => product.DeltaAbs),
                DeltaRel = portfolio.Sum(product => product.DeltaRel)
            };

            return portfolioResponse;
        }
    }
}
