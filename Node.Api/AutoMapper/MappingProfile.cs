using AutoMapper;
using Node.Api.Helpers;
using Node.Api.Models;

namespace Node.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        private readonly IDateTimeHelpers dateTimeHeplers;

        public MappingProfile()
        {
            CreateMaps();
        }

        protected void CreateMaps()
        {
        }
    }
}
