using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Vaquinha.Repository.Context;

namespace Vaquinha.Unit.Tests.Fixture
{
    public class DependencyInjectionFixture
    {
        public readonly IServiceCollection services;

        public DependencyInjectionFixture()
        {
            services = new ServiceCollection()
                .AddDbContext<VaquinhaOnlineDBContext>(opt => opt.UseInMemoryDatabase("VaquinhaOnLineDIOTest"));

            #region IConfiguration
            services.AddTransient<IConfiguration>((_services) =>
                new ConfigurationBuilder()

                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", true, true)
                .Build()
            );
            #endregion
        }
    }
}