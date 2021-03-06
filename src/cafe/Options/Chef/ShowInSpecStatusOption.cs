using System;
using cafe.Client;
using cafe.CommandLine;
using cafe.Shared;

namespace cafe.Options.Chef
{
    public class ShowInSpecStatusOption : ServerConnectionOption<IProductServer<ProductStatus>>
    {
        public ShowInSpecStatusOption(Func<IProductServer<ProductStatus>> inspecServerFactory) : base(
            inspecServerFactory, "shows inspec status")
        {
        }

        protected override string ToDescription(Argument[] args)
        {
            return "Showing InSpec Status";
        }

        protected override Result RunCore(IProductServer<ProductStatus> client, Argument[] args)
        {
            ShowChefStatusOption.ShowProductStatus(client.GetStatus().Result, "InSpec");
            return Result.Successful();
        }
    }
}