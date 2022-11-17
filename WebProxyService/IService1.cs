using System;
using System.Collections.Generic;
using System.Linq;
//using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WebProxyService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContractAttribute]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "itinerary?w={param1}&x={param2}&y={param3}&z={param4}")]
        Task<List<Station>> GetNearestStationFromPoint(double param1, double param2, double param3, double param4);

        [OperationContractAttribute]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Bare, ResponseFormat = WebMessageFormat.Json, UriTemplate = "stations")]
        Task<List<Station>> GetStationsList();


    }

}
