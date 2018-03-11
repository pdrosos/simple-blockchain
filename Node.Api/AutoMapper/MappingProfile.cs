using AutoMapper;
using Node.Api.Models;

namespace Node.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMaps();
        }

        protected void CreateMaps()
        {
            CreateMap<Transaction, TransactionSignatureDataModel>();
        }
    }
}
