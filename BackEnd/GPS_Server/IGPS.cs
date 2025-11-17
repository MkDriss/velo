using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Json;

namespace GPS_Server
{
	[ServiceContract]
	public interface IGPS
	{
        [OperationContract]
        [WebGet(
            UriTemplate = "/getItinerary?start={address1}&end={address2}",
            ResponseFormat = WebMessageFormat.Json)]
        string GetItinerary(string address1, string address2);


        [OperationContract]
        [WebGet(
        UriTemplate = "/ThrowBikeSeine?start={address1}",
        ResponseFormat = WebMessageFormat.Json)]
        string ThrowBikeSeine(string address1);

        [OperationContract]
        [WebGet(
        UriTemplate = "/getWalk?start={address1}&end={address2}",
        ResponseFormat = WebMessageFormat.Json)]
        string GetWalk(string address1, string address2);

    }
}
