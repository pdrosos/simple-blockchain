using AutoMapper;
using Infrastructure.Library.Helpers;

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
