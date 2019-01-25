using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreVueTypeScript.Dto
{
    public class WeatherDto
    {
        public string City { get; set; }
        public List <WeatherMeasureDto> Readings { get; set; }

        public WeatherDto()
        {
            Readings = new List<WeatherMeasureDto>();
        }
    }
}
