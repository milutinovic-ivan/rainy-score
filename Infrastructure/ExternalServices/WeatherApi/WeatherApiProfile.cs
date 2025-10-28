using AutoMapper;
using Domain.Entities;

namespace Infrastructure.ExternalServices.WeatherApi
{
    public class WeatherApiProfile : Profile
    {
        public WeatherApiProfile()
        {
            CreateMap<WeatherApiCurrentResponseDto, WeatherData>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.location.name))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.location.country))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.location.lat))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.location.lon))
                .ForMember(dest => dest.LocalTime, opt => opt.MapFrom(src => src.location.localtime))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.current.last_updated))
                .ForMember(dest => dest.TempC, opt => opt.MapFrom(src => src.current.temp_c))
                .ForMember(dest => dest.IsRain, opt => opt.MapFrom(src => src.current.condition.text.Contains("rain")))
                .ForMember(dest => dest.IsRain, opt => opt.MapFrom(src => src.current.condition.text.Contains("snow")))
                .ForMember(dest => dest.WindKph, opt => opt.MapFrom(src => src.current.wind_kph))
                .ForMember(dest => dest.PressureMb, opt => opt.MapFrom(src => src.current.pressure_mb))
                .ForMember(dest => dest.PrecipMm, opt => opt.MapFrom(src => src.current.precip_mm))
                .ForMember(dest => dest.Humidity, opt => opt.MapFrom(src => src.current.humidity))
                .ForMember(dest => dest.Cloud, opt => opt.MapFrom(src => src.current.cloud));
        }
    }
}
