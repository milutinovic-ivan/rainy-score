using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Intefraces
{
    public interface IStadiumLocationService
    {
        Task<(decimal? latitude, decimal? longitude)> GetStadiumLocationAsync(string stadiumName);
    }
}
