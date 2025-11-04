using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GPS_Server
{
	[ServiceContract]
	public interface IGPS
	{
        [OperationContract]
        [WebGet(
            UriTemplate = "/getItinerary?start={address1}&end={address2}",
            ResponseFormat = WebMessageFormat.Json)]
        double GetItinerary(string address1, string address2);

    }
}
