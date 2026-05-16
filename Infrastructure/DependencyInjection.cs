using Application.Intefraces;
using Application.Jobs;
using Domain.Interfaces;
using Infrastructure.ExternalServices.MatchHistory.FootballData;
using Infrastructure.ExternalServices.MatchLive.ApiFootball;
using Infrastructure.ExternalServices.Stadium.GooglePlaceApi;
using Infrastructure.ExternalServices.WeatherForecast.OpenMeteo;
using Infrastructure.ExternalServices.WeatherHistory.OpenMeteo;
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
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

            services.AddScoped(typeof(IRepository<>), typeof(ScoreRepository<>));

            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<IMatchHistoryService, FootballDataMatchHistoryService>();

            services.AddScoped<IWeatherHistoryService, OpenMeteoWeatherHistoryService>();

            services.AddScoped<IStadiumService, GooglePlaceStadiumService>();

            services.AddScoped<IWeatherForecastService, OpenMeteoWeatherForecastService>();

            //ApiFootball injection
            services.AddScoped<IMatchLiveService, ApiFootballMatchLiveService>();

            services.AddQuartz(options =>
            {
                var matchHistoryImportJobKey = new JobKey("MatchHistoryImportJob");
                var stadiumImportJobKey = new JobKey("StadiumImportJob");
                var weatherHistoryImportJobKey = new JobKey("WeatherHistoryImportJob");
                var matchLiveImportJobKey = new JobKey("MatchLiveImportJob");
                var matchOddsImportJobKey = new JobKey("MatchOddsImportJob");
                var weatherForecastImportJobKey = new JobKey("WeatherForecastImportJob");

                //StoreDurably if I want just to run job from api endpoint without scheduler
                options.AddJob<MatchHistoryImportJob>(matchHistoryImportJobKey, j => j.StoreDurably());
                options.AddJob<StadiumImportJob>(stadiumImportJobKey, j => j.StoreDurably());
                options.AddJob<WeatherHistoryImportJob>(weatherHistoryImportJobKey, j => j.StoreDurably());
                options.AddJob<MatchLiveImportJob>(matchLiveImportJobKey, j => j.StoreDurably());
                options.AddJob<MatchOddsImportJob>(matchOddsImportJobKey, j => j.StoreDurably());
                options.AddJob<WeatherForecastImportJob>(weatherForecastImportJobKey, j => j.StoreDurably());
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            return services;
        }
    }
}
