using System;
using IntegrationBl.Configurations;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using Shared.Exceptions;

namespace IntegrationBl.Factories
{
    public class PolicyFactory : IPolicyFactory
    {
        private readonly PoliciesConfig _policiesConfig;
        
        public PolicyFactory(IOptions<PoliciesConfig> policiesConfig)
        {
            _policiesConfig = policiesConfig?.Value ?? throw new ArgumentNullException(nameof(PoliciesConfig));
        }


        public Policy CreateRetryPolicyWithExponentialBackoff(Action onRetry)
        {
            return Policy.Handle<TooManyRequestsException>()
                .WaitAndRetry(_policiesConfig.RetryCount, 
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) => {
                        onRetry();
                    });

        }

        public PolicyWrap CreateUpdateTaskPolicies(Action onRetry)
        {
            return Policy.WrapAsync(new IAsyncPolicy[] { CreateRetryPolicyWithExponentialBackoff(onRetry) });
        }
    }
}