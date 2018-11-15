using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MongoDal.Models;
using Shared.Models.Integration;

namespace MongoDal.Configurations
{
    public class DalMappingProfile : Profile
    {
        public DalMappingProfile()
        {
            CreateMap<IntegrationSaga, IntegrationSagaEntity>();

            CreateMap<IntegrationSagaEntity, IntegrationSaga>();

            CreateMap<IntegrationSagaItemEntity, IntegrationItem>();
                
            CreateMap<IntegrationItem, IntegrationSagaItemEntity>()
                .ForMember(d => d.SagaId, opt => opt.Ignore());
                
            CreateMap<IntegrationSagaExtended, IEnumerable<IntegrationSagaItemEntity>>()
                .ConvertUsing<IntegrationSagaItemConverter>();
        }
        
        public class IntegrationSagaItemConverter : ITypeConverter<IntegrationSagaExtended, IEnumerable<IntegrationSagaItemEntity>>
        {

            public IEnumerable<IntegrationSagaItemEntity> Convert(IntegrationSagaExtended source, IEnumerable<IntegrationSagaItemEntity> destination, ResolutionContext context)
            {
                return source.IntegrationItems.Select(sourceIntegrationItem =>
                    new IntegrationSagaItemEntity
                    {
                        Id = sourceIntegrationItem.Id,
                        SagaId = source.Id
                    });
            }
        }
    }
}