using System;
using Microsoft.Extensions.DependencyInjection;

namespace AspectCore.Extensions.DependencyInjection
{
    public class DynamicProxyServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private ServiceProviderOptions _serviceProviderOptions;
        private Func<ServiceDescriptor, bool> _serviceValidator;

        public DynamicProxyServiceProviderFactory()
            : this(null, null)
        {
        }

        public DynamicProxyServiceProviderFactory(Func<ServiceDescriptor, bool> shouldConsiderForAopValidator)
            : this(null, shouldConsiderForAopValidator)
        {
        }

        public DynamicProxyServiceProviderFactory(bool validateScopes, Func<ServiceDescriptor, bool> shouldConsiderForAopValidator)
            : this(new ServiceProviderOptions() {ValidateScopes = validateScopes}, shouldConsiderForAopValidator)
        {
        }

        public DynamicProxyServiceProviderFactory(ServiceProviderOptions serviceProviderOptions, Func<ServiceDescriptor, bool> serviceValidator)
        {
            _serviceProviderOptions = serviceProviderOptions;
            _serviceValidator = serviceValidator;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            if (_serviceProviderOptions == null)
            {
                return _serviceValidator == null
                    ? containerBuilder.BuildDynamicProxyProvider()
                    : containerBuilder.BuildDynamicProxyProvider(_serviceValidator);
            }

            return containerBuilder.BuildDynamicProxyProvider(
                _serviceProviderOptions,
                _serviceValidator
            );
        }
    }
}