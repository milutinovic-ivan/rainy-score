using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Intefraces
{
    public interface IStadiumDataBuilder
    {
        public Task<StadiumData> BuildAsync(string teamName);
    }
}
