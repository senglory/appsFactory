using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreVueTypeScript.Dto
{
    public class WeatherMeasureDto
    {
        public string DateFormatted { get; set; }
        public double TemperatureC { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
    }
}
