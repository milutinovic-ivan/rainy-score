using Microsoft.EntityFrameworkCore;
using Quartz;
using Application.Intefraces;
using Application.Jobs;
using Domain.Interfaces;
using Infrastructure.ExternalServices.FootballData;
using Infrastructure.ExternalServices.WeatherApi;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.ExternalServices.Stadium.ChatGpt;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ScoreDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(ScoreRepository<>));

            services.AddScoped<IWeatherService, WeatherApiWeatherService>();

            services.AddScoped<IMatchHistoryService, FootballDataMatchHistoryService>();

            services.AddScoped<IStadiumService, ChatGptStadiumService>();

            services.AddQuartz(options =>
            {
                var matchHistoryImportJobKey = new JobKey("MatchHistoryImportJob");
                var stadiumImportJobKey = new JobKey("StadiumImportJob");

                //StoreDurably if I want just to run job from api endpoint without scheduler
                options.AddJob<MatchHistoryImportJob>(matchHistoryImportJobKey, j => j.StoreDurably());
                options.AddJob<StadiumImportJob>(stadiumImportJobKey, j => j.StoreDurably());
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            return services;
        }
    }
}
