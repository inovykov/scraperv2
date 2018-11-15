using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Scheduler.Configurations;
using Shared.Extensions;
using Shared.Services;

namespace Scheduler.Clients
{
    public class IntegrationClient : IIntegrationClient
    {
        private readonly HttpClient _httpClient;
        private readonly IntegrationClientConfig _integrationClientConfig;
        private readonly IUrlFormatService _urlFormatService;


        public IntegrationClient(HttpClient httpClient, 
            IOptions<IntegrationClientConfig> integrationClientConfig, 
            IUrlFormatService urlFormatService)
        {
            _integrationClientConfig = integrationClientConfig.Value;

            _httpClient = httpClient;
            _urlFormatService = urlFormatService;
            _httpClient.BaseAddress = new Uri(_integrationClientConfig.BaseAddress);
        }
        
        public async Task StartUpdateProcessAsync(CancellationToken cancellationToken)
        {
            var httpResponseMessage = 
                await _httpClient.PostAsync(_urlFormatService.FormatUrlComponent(_integrationClientConfig.StartUpdateProcessUrl), null, cancellationToken);

            httpResponseMessage.UnwindHttpExceptions();
        }

        public async Task UpdateInfoAboutTvShowAsync(CancellationToken cancellationToken)
        {
            var httpResponseMessage = 
                await _httpClient.PostAsync(_urlFormatService.FormatUrlComponent(_integrationClientConfig.UpdateInfoAboutTvShowAsync), null, cancellationToken);

            httpResponseMessage.UnwindHttpExceptions();
        }
    }
}
