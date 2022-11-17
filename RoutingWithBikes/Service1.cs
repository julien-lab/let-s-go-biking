using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace RoutingWithBikes
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {

        public Service1()
        {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        
        public async Task<List<JsonOpenRouteService>> GetItinerary(float param1, float param2, float param3, float param4)
        {
            List<Station> stationsPositions = await GetNearestStationFromPoint(param1,  param2,  param3,  param4);
            SetStats(stationsPositions);
            List<JsonOpenRouteService> aaa = new List<JsonOpenRouteService>();
            if (stationsPositions == null ) return null;
            if (stationsPositions.Count == 0) return null;
            GeoCoordinate g1 = new GeoCoordinate(param2, param1);
            GeoCoordinate g2 = new GeoCoordinate(param4, param3);
            if (stationsPositions[0].number == stationsPositions[1].number ||
                (g1.GetDistanceTo(g2)*4).CompareTo((new GeoCoordinate(stationsPositions[0].position.latitude, stationsPositions[0].position.longitude))
                .GetDistanceTo(new GeoCoordinate(stationsPositions[1].position.latitude, stationsPositions[1].position.longitude))) < 0)
            {
                JsonOpenRouteService jsonOpenRouteService1 = await GetRoutes(param1, param2, param3, param4);
                aaa.Add(jsonOpenRouteService1);
            }
            else
            {
                JsonOpenRouteService jsonOpenRouteService1 = await GetRoutes(param1, param2, stationsPositions[0].position.longitude, stationsPositions[0].position.latitude);
                JsonOpenRouteService jsonOpenRouteService2 = await GetRoutes(stationsPositions[0].position.longitude, stationsPositions[0].position.latitude, stationsPositions[1].position.longitude, stationsPositions[1].position.latitude);
                JsonOpenRouteService jsonOpenRouteService3 = await GetRoutes(stationsPositions[1].position.longitude, stationsPositions[1].position.latitude, param3, param4);
                jsonOpenRouteService1.features[0].geometry.coordinates.AddRange(jsonOpenRouteService2.features[0].geometry.coordinates);
                jsonOpenRouteService1.features[0].geometry.coordinates.AddRange(jsonOpenRouteService3.features[0].geometry.coordinates);
                aaa.Add(jsonOpenRouteService1);
            }
            return aaa;
        }

        public async Task<JsonOpenRouteService> GetRoutes(float param1, float param2, float param3, float param4)
        {
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync("https://api.openrouteservice.org/v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62481b45e6cc4dc346cbae234875d4c2a29e&start=" + param1.ToString(CultureInfo.InvariantCulture) + "," + param2.ToString(CultureInfo.InvariantCulture) + "&end=" + param3.ToString(CultureInfo.InvariantCulture) + "," + param4.ToString(CultureInfo.InvariantCulture));
            if (response == null) throw new InvalidOperationException("ouch");
            if (response.Length == 0) throw new InvalidOperationException("ouille");
            JsonOpenRouteService jsonOpenRouteService = JsonConvert.DeserializeObject<JsonOpenRouteService>(response);
            if (jsonOpenRouteService != null) return jsonOpenRouteService;
            else throw new InvalidOperationException("damn"); 
        }
     
         public async Task<List<Station>> GetNearestStationFromPoint(float param1, float param2, float param3, float param4)
         {
            HttpClient client = new HttpClient();
            try
            {
                string response = await
                    client.GetStringAsync("http://localhost:8733/Design_Time_Addresses/WebProxyService/host/rest/itinerary?w=" + param1.ToString(CultureInfo.InvariantCulture) + "&x=" + param2.ToString(CultureInfo.InvariantCulture) + "&y=" + param3.ToString(CultureInfo.InvariantCulture) + "&z=" + param4.ToString(CultureInfo.InvariantCulture));
                List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(response);
                return stations;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
            }
            return null;
         }  
        
        public async Task<List<Station>> GetStations()
        {
            HttpClient client = new HttpClient();
            try
            {
                string response = await client.GetStringAsync("http://localhost:8733/Design_Time_Addresses/WebProxyService/host/rest/stations");
                List<Station> stations = JsonConvert.DeserializeObject<List<Station>>(response);
                return stations;

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);
            }
            return null;
            
        }

        public static List<List<string>> excelBiking = null;

        public async void SetStats(List<Station> stationsUsed)
        {
            if (excelBiking == null)
            {
                excelBiking = new List<List<string>>();
                List<Station> stations = await GetStations();
                foreach (Station station in stations)
                {
                    List<string> stringList = new List<string>
                    {
                        station.number.ToString()
                    };
                    excelBiking.Add(stringList);
                }
                SetStats(stationsUsed);
            }
            else
            {
                foreach (List<string> stationList in excelBiking)
                {
                    if (stationList[0] == stationsUsed[0].number.ToString())
                    {
                        DateTime now = DateTime.Now;
                        stationList.Add(now.ToString());
                    }
                    else if (stationList[0] == stationsUsed[1].number.ToString())
                    {
                        DateTime now = DateTime.Now;
                        stationList.Add(now.ToString());
                    }
                }
            }          
        }

        public List<List<string>> GetStats()
        {
            if (excelBiking == null) return null;
            return excelBiking;
        }
    }
}
