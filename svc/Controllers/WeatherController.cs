using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

using NetCoreVueTypeScript.Dto;



namespace svc.Controllers
{
    [Route("api/[controller]")]
    public class WeatherDataController : Controller
    {
        private IMemoryCache weatherCache;


        public WeatherDataController(IMemoryCache cache)
        {
            this.weatherCache = cache;
            MemoryCacheEntryOptions cacheExpirationOptions = new MemoryCacheEntryOptions();
            cacheExpirationOptions.AbsoluteExpiration = DateTime.Now.AddHours(3);
            cacheExpirationOptions.Priority = CacheItemPriority.Normal;
            this.weatherCache.Set<string>("IDGKey", DateTime.Now.ToString(), cacheExpirationOptions);
        }

        async Task<WeatherDto> GetWeatherDtoInternal(string cityOrZip)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/forecast?q={cityOrZip},de&appid=fcadd28326c90c3262054e0e6ca599cd&units=metric");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                dynamic rawWeather = JsonConvert.DeserializeObject(stringResult);

                var res = new WeatherDto
                {
                    City = rawWeather.city.name,
                };
                foreach (var wr in rawWeather.list)
                {
                    var weather = new WeatherMeasureDto()
                    {
                        DateFormatted = DateTimeStringFromUnixString(Convert.ToString(wr.dt)),
                        TemperatureC = wr.main.temp,
                        Humidity = wr.main.humidity,
                        WindSpeed = wr.wind.speed,
                    };
                    res.Readings.Add(weather);
                }

                return res;
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> WeatherForecasts(string cityOrZip)
        {
            try
            {
                var key = cityOrZip + DateTime.Now.ToString("dd-MM-yyyy");

                var x = await GetOrSetValueSafeAsync(weatherCache, key, 
                    TimeSpan.FromDays(1),   
                     async () => {
                         var t1 = await GetWeatherDtoInternal(cityOrZip);
                         return t1;
                    });

                return Json( x);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting weather from OpenWeather: {ex.Message}");
            }
        }


        public static T GetOrSetValueSafeAsync<T>(IMemoryCache cache, string key, TimeSpan expiration, Func<T> valueFactory)
        {
            if (cache.TryGetValue(key, out Lazy<T> cachedValue))
                return cachedValue.Value;

            Func<ICacheEntry, Lazy<T> > factory = (ICacheEntry entry) =>
            {
                entry.SetSlidingExpiration(expiration);
                Lazy<T> r0 = new Lazy<T>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
                return r0;
            };

            var tt =  cache.GetOrCreate(key, factory);
            var lt = cache.Get<Lazy<T>>(key);
            return lt.Value;
        }

        string DateTimeStringFromUnixString(string unixDt)
        {
            double dt = Convert.ToDouble(unixDt);
            // Format our new DateTime object to start at the UNIX Epoch
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

            // Add the timestamp (number of seconds since the Epoch) to be converted
            dateTime = dateTime.AddSeconds(dt);
            return dateTime.ToString("dd-MM-yyyy HH:mm");
        }
    }
}
