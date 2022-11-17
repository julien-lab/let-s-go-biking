using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;

namespace WebProxyService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public async Task<List<Station>> GetNearestStationFromPoint(double param1, double param2, double param3, double param4)
        {
            GeoCoordinate targetCoordinate = new GeoCoordinate(param2, param1);
            GeoCoordinate targetCoordinate2 = new GeoCoordinate(param4, param3);
            List<Station> stations = await GetStationsList();
            if (stations == null)
            {
                return null;
            }
            else if (stations.Count == 0)
            {
                return null;
            }
            else
            {
                stations.Sort((station1, station2) =>
                {
                    GeoCoordinate g1 = new GeoCoordinate(station1.position.latitude, station1.position.longitude);
                    GeoCoordinate g2 = new GeoCoordinate(station2.position.latitude, station2.position.longitude);
                    return g1.GetDistanceTo(targetCoordinate).CompareTo(g2.GetDistanceTo(targetCoordinate));
                });
                Station ClosestStationToStartingPoint = null;
                int i = 0;
                while (i < stations.Count)
                {
                    if (stations[i].totalStands.availabilities.bikes > 0)
                    {
                        ClosestStationToStartingPoint = stations[0];
                        break;
                    }
                    else if (i == stations.Count) return null; 
                    i++;
                }
                stations.Sort((station1, station2) =>
                {
                    GeoCoordinate g1 = new GeoCoordinate(station1.position.latitude, station1.position.longitude);
                    GeoCoordinate g2 = new GeoCoordinate(station2.position.latitude, station2.position.longitude);
                    return g1.GetDistanceTo(targetCoordinate2).CompareTo(g2.GetDistanceTo(targetCoordinate2));
                });
                i = 0;
                Station ClosestStationToEndingPoint = null;
                while (i < stations.Count)
                {
                    if (stations[i].totalStands.availabilities.bikes > 0)
                    {
                        ClosestStationToEndingPoint = stations[0];
                        break;
                    }
                    else if (i == stations.Count) return null;
                    i++;
                }
                List<Station> StationsList = new List<Station>
                {
                    ClosestStationToStartingPoint,
                    ClosestStationToEndingPoint
                };
                return StationsList;
            }
        }

        ObjectCache cache = MemoryCache.Default;

        public void SetStationsInCache(List<Station> stationList)
        {
            var cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(86400.0),

            };
            cache.Add("stationList", stationList, cacheItemPolicy);            
        }

        public async Task<List<Station>> GetStationsList()
        {
            if (cache.Get("stationList") != null) return (List<Station>) cache.Get("stationList");
            HttpResponseMessage response;
            HttpClient client = new HttpClient();
            string API_KEY = "3fa830fae00b65ede1fe5344601cf0734f2394c4";
            try
            {
                response = await client.GetAsync("https://api.jcdecaux.com/vls/v3/stations/" + "?apiKey=" + API_KEY);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(responseBody);
                SetStationsInCache(stations);
                return stations;
            }
            catch (HttpRequestException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Http error : ");
                Console.WriteLine(ex);
                Console.ResetColor();
            }
            return null;
        }

    }
}
