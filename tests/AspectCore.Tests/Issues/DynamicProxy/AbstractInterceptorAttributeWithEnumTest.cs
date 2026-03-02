using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspectCore.Tests.Issues.DynamicProxy
{
    public class AbstractInterceptorAttributeWithEnumTest
    {
        private readonly ITest _test;

        public AbstractInterceptorAttributeWithEnumTest()
        {
            // Arrange
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ITest, TestClass>();
            IServiceProvider serviceProvider = services.BuildDynamicProxyProvider();
            IServiceScope scope = serviceProvider.CreateScope();
            _test = scope.ServiceProvider.GetRequiredService<ITest>();
        }

        [Fact]
        public void TestMethod_WithArrayOfAllowedValues()
        {
            // Act
            Exception ex = Record.Exception(() => _test.TestMethodWithArrayOfAllowedValues(TestValue.Value1));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void TestMethod_WithSingleAllowedValue()
        {
            // Act
            Exception ex = Record.Exception(() => _test.TestMethodWithSingleAllowedValue(TestValue.Value1));

            // Assert
            Assert.Null(ex);
        }
    }

    public enum TestValue
    {
        Value1, Value2, Value3
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
    public sealed class TestAttribute : AbstractInterceptorAttribute
    {
        private readonly IEnumerable<TestValue> _testValues;

        public TestAttribute(TestValue testValue) : this(new[] { testValue })
        {
        }

        public TestAttribute(params TestValue[] testValues)
        {
            _testValues = testValues;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (!_testValues.Contains((TestValue)context.Parameters[0]))
            {
                throw new InvalidOperationException($"Parameter must be one of {string.Join(',', _testValues)}");
            }
            await next(context);
        }
    }

    public interface ITest
    {
        void TestMethodWithArrayOfAllowedValues(TestValue testValue);

        void TestMethodWithSingleAllowedValue(TestValue testValue);
    }

    public class TestClass : ITest
    {
        [Test(TestValue.Value1, TestValue.Value2)]
        public void TestMethodWithArrayOfAllowedValues(TestValue testValue)
        {
        }

        [Test(TestValue.Value1)]
        public void TestMethodWithSingleAllowedValue(TestValue testValue)
        {
        }
    }
}
