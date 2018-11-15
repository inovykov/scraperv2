//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using IntegrationBl.Services;
//using Moq;
//using Shared.Interfaces;
//using Shared.Models.Integration;
//using Shared.Services;
//using Xunit;

//namespace BusinessLogic.Tests
//{
//    public class UpdateServiceTests
//    {
//        private readonly IUpdateService _updateService;

//        private readonly Mock<IIntegrationDal> _integrationDal;

//        private readonly Mock<IIntegrationTaskFactory> _integrationSagaFactory;

//        private readonly Mock<ITvShowUpdateService> _tvShowUpdateService;

//        private readonly IntegrationSagaExtended _integrationSaga;

//        private readonly IntegrationItem _integrationItem;

//        private readonly CancellationToken _cancellationToken;

//        public UpdateServiceTests()
//        {
//            _integrationDal = new Mock<IIntegrationDal>();

//            var dateTimeService = new Mock<IDateTimeService>();

//            _integrationSagaFactory = new Mock<IIntegrationTaskFactory>();
            
//            _tvShowUpdateService = new Mock<ITvShowUpdateService>();

//            _cancellationToken = new CancellationToken();

//            var sagaId = new Guid("2aa0b175-7bcb-458a-9130-9b282af9f27b");

//            var dateTimeOfPreviousExecution = new DateTime(2018, 11, 6, 12, 01, 01);
            
//            _integrationItem = new IntegrationItem(1);
            
//            _integrationSaga = new IntegrationSagaExtended
//            {
//                Id = sagaId,
//                StartDate = dateTimeOfPreviousExecution,
//                IntegrationItems = new List<IntegrationItem>()
//                {
//                    new IntegrationItem(1),
//                    new IntegrationItem(2),
//                    new IntegrationItem(3)
//                }
//            };

//            _updateService = new UpdateService(
//                _integrationDal.Object, 
//                dateTimeService.Object,
//                _integrationSagaFactory.Object,
//                _tvShowUpdateService.Object
//                );
//        }

//        [Fact]
//        public async void Processing_Result_Not_Started_If_Nothing_To_Process()
//        {
//            _integrationDal.Setup(d => d.GetTaskInProgressAsync(_cancellationToken)).ReturnsAsync(() => _integrationSaga);

//            var processingResult = await _updateService.StartUpdateProcessAsync(_cancellationToken);
            
//            _integrationSagaFactory.Verify(f => f.CreateIntegrationTask(It.IsAny<IEnumerable<int>>(),It.IsAny<DateTime>()), Times.Never);

//            Assert.False(processingResult);
//        }

//        [Fact]
//        public async void Verify_New_Saga_Not_Created_When_Uncompleted_Exists()
//        {
//            _integrationDal.Setup(i => i.GetTaskInProgressAsync(_cancellationToken))
//                .ReturnsAsync(() => _integrationSaga);

//            var result = await _updateService.StartUpdateProcessAsync(_cancellationToken);

//            Assert.False(result);
//            _tvShowUpdateService.Verify(t => t.GetOutdatedTvShowInfosIdsAsync(_cancellationToken), Times.Never);
//        }

//        [Fact]
//        public async void Saga_Completed_When_No_More_Items_To_Process()
//        {
//            _integrationDal.Setup(i => i.GetTaskInProgressAsync(_cancellationToken))
//                .ReturnsAsync(() => _integrationSaga);

//            await _updateService.UpdateInfoAboutTvShowAsync(_cancellationToken);

//            _integrationDal.Verify(i => i.SetIntegrationTaskStateAsync(It.IsAny<Guid>(), It.Is<IntegrationTaskStates>(x => x == IntegrationTaskStates.Completed), _cancellationToken), Times.Once);
//        }

//        [Fact]
//        public async void Verify_Saga_Factory_Receives_Proper_Keys_For_Saga_Creation()
//        {
//            var listOfOutdatedShowInfos = new List<int> {1, 2, 3};
            
//            _tvShowUpdateService.Setup(t => t.GetOutdatedTvShowInfosIdsAsync(_cancellationToken))
//                .ReturnsAsync(listOfOutdatedShowInfos);

//            await _updateService.StartUpdateProcessAsync(_cancellationToken);

//            _integrationSagaFactory.Verify(s => s.CreateIntegrationTask(It.Is<IList<int>>(x => Equals(x, listOfOutdatedShowInfos)), It.IsAny<DateTime>()), Times.Once) ;
//        }

//        [Fact]
//        public async void Verify_Tv_Show_Update_Service_Called_For_Saga_Item()
//        {
//            await CallUpdateForSingleItem();

//            _tvShowUpdateService.Verify(t => t.CreateOrUpdateTvShowAsync(It.Is<IntegrationItem>(x => x == _integrationItem), _cancellationToken), Times.Once);
//        }

//        [Fact]
//        public async void Verify_Saga_Item_Deleted_After_Processing()
//        {
//            await CallUpdateForSingleItem();

//            _integrationDal.Verify(i => i.DeleteSagaItemByIdAsync(It.Is<int>(x => x == _integrationItem.Id), _cancellationToken), Times.Once);
//        }

//        private async Task CallUpdateForSingleItem()
//        {
//            _integrationDal.Setup(i => i.GetTaskInProgressAsync(_cancellationToken))
//                .ReturnsAsync(() => _integrationSaga);

//            _integrationDal.Setup(i => i.GetRandomSagaItemAsync(It.IsAny<Guid>(), _cancellationToken))
//                .ReturnsAsync(_integrationItem);

//            await _updateService.UpdateInfoAboutTvShowAsync(_cancellationToken);
//        }
//    }
//}
