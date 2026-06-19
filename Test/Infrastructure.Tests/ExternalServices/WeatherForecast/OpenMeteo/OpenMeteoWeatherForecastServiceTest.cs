using Infrastructure.ExternalServices.WeatherForecast.OpenMeteo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Infrastructure.Tests.ExternalServices.WeatherForecast.OpenMeteo
{
    public class OpenMeteoWeatherForecastServiceTest
    {
        private readonly ILogger<OpenMeteoWeatherForecastService> _logger;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public OpenMeteoWeatherForecastServiceTest()
        {
            _logger = Mock.Of<ILogger<OpenMeteoWeatherForecastService>>();

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient());

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Weather:Forecast:OpenMeteoForecast:BaseUrl"] = "https://test.com",
                    ["Weather:Forecast:OpenMeteoForecast:AnalyzeLastHours"] = "2"
                })
                .Build();
        }

        [Fact]
        public void PharseWeatherForecastResponseTest()
        {
            //Arrange
            var jsonWeather = @"{""latitude"":7.768014,""longitude"":-72.21463,""generationtime_ms"":1.9708871841430664,""utc_offset_seconds"":0,""timezone"":""GMT"",""timezone_abbreviation"":""GMT"",""elevation"":1052.0,""hourly_units"":{""time"":""iso8601"",""temperature_2m"":""°C"",""dew_point_2m"":""°C"",""precipitation"":""mm"",""cloud_cover"":""%"",""cloud_cover_low"":""%"",""wind_speed_10m"":""km/h"",""sunshine_duration"":""s"",""weather_code"":""wmo code""},""hourly"":{""time"":[""2026-06-06T00:00"",""2026-06-06T01:00"",""2026-06-06T02:00"",""2026-06-06T03:00"",""2026-06-06T04:00"",""2026-06-06T05:00"",""2026-06-06T06:00"",""2026-06-06T07:00"",""2026-06-06T08:00"",""2026-06-06T09:00"",""2026-06-06T10:00"",""2026-06-06T11:00"",""2026-06-06T12:00"",""2026-06-06T13:00"",""2026-06-06T14:00"",""2026-06-06T15:00"",""2026-06-06T16:00"",""2026-06-06T17:00"",""2026-06-06T18:00"",""2026-06-06T19:00"",""2026-06-06T20:00"",""2026-06-06T21:00"",""2026-06-06T22:00"",""2026-06-06T23:00""],""temperature_2m"":[18.5,18.2,18.1,18.1,18.1,18.0,18.1,17.9,17.6,17.4,17.4,17.4,17.6,18.8,19.2,19.8,20.3,19.9,19.8,19.7,19.9,19.5,19.6,19.1],""dew_point_2m"":[18.5,18.2,18.1,18.1,18.0,17.9,18.0,17.6,17.5,17.4,17.3,17.2,17.3,17.1,17.6,18.3,18.7,19.0,19.4,19.3,19.4,19.3,19.2,19.0],""precipitation"":[0.30,0.10,0.00,0.00,0.00,0.00,0.00,0.00,0.10,0.20,0.20,0.10,0.10,0.10,0.50,1.30,1.00,1.20,1.80,1.10,0.80,0.60,0.20,0.20],""cloud_cover"":[100,100,100,100,100,100,100,98,100,100,100,100,99,100,100,99,100,99,100,100,100,100,100,100],""cloud_cover_low"":[98,98,100,98,100,95,100,73,95,99,96,77,71,53,61,83,90,82,75,100,100,100,100,100],""wind_speed_10m"":[5.8,5.5,4.6,4.5,3.9,3.9,2.3,2.1,2.3,3.2,3.9,5.0,5.7,7.4,7.5,8.5,8.8,8.2,8.0,6.6,7.8,6.8,3.4,2.5],""sunshine_duration"":[0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,0.00,3600.00,3600.00,3600.00,3600.00,3600.00,3600.00,3600.00,1666.27,156.66,0.00,0.00],""weather_code"":[51,51,3,3,3,3,3,3,51,51,51,51,51,51,53,61,55,55,61,55,53,53,51,51]}}";
            var matchTime = new TimeOnly(19, 30);

            //Act
            var service = new OpenMeteoWeatherForecastService(_httpClientFactoryMock.Object, _logger, _configuration);
            var responseData = service.PharseWeatherForecastResponse(jsonWeather, matchTime);

            //Assert
            Assert.NotNull(responseData);
            Assert.Equal(19.9m, responseData.Temperature2m);
            Assert.Equal(19.4m, responseData.DewPoint2m);
            Assert.Equal(5.5m, responseData.Precipitation);
            Assert.Equal(100, responseData.CloudCover);
            Assert.Equal(100, responseData.CloudCoverLow);
            Assert.Equal(7.8m, responseData.WindSpeed10m);
            Assert.Equal(0, responseData.SunshineDuration);
            Assert.Equal(53, responseData.WeatherCode);
        }
    }
}
