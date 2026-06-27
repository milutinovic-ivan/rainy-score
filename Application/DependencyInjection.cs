using Application.Intefraces;
using Application.Jobs.Services;
using Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IStadiumService, StadiumService>();

            services.AddScoped<IJobExecutionsService, JobExecutionsService>();

            return services;
        }
    }
}
