using AutoMapper;
using MongoDal.Models;
using Shared.Models.Integration;

namespace MongoDal.Configurations
{
    public class DalMappingProfile : Profile
    {
        public DalMappingProfile()
        {
            CreateMap<IntegrationTask, IntegrationTaskEntity>();

            CreateMap<IntegrationTaskEntity, IntegrationTask>();
        }
    }
}