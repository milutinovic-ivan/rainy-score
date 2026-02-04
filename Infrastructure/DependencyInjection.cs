using Application.Intefraces;
using Application.Jobs;
using Domain.Interfaces;
using Infrastructure.ExternalServices.MatchHistory.FootballData;
using Infrastructure.ExternalServices.MatchLive.ApiFootball;
using Infrastructure.ExternalServices.Stadium.GooglePlaceApi;
using Infrastructure.ExternalServices.Weather.OpenMeteo;
using Infrastructure.ExternalServices.WeatherApi;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Reflection;

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

            services.AddScoped<IWeatherHistoryService, OpenMeteoWeatherService>();

            services.AddScoped<IStadiumService, GooglePlaceStadiumService>();

            //ApiFootball injection
            services.AddScoped<IMatchLiveService, ApiFootballMatchLiveService>();
            services.AddScoped<ILeagueService, ApiFootballMatchLiveService>();

            services.AddQuartz(options =>
            {
                var matchHistoryImportJobKey = new JobKey("MatchHistoryImportJob");
                var stadiumImportJobKey = new JobKey("StadiumImportJob");
                var weatherHistoryImportJobKey = new JobKey("WeatherHistoryImportJob");

                //StoreDurably if I want just to run job from api endpoint without scheduler
                options.AddJob<MatchHistoryImportJob>(matchHistoryImportJobKey, j => j.StoreDurably());
                options.AddJob<StadiumImportJob>(stadiumImportJobKey, j => j.StoreDurably());
                options.AddJob<WeatherHistoryImportJob>(weatherHistoryImportJobKey, j => j.StoreDurably());
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
