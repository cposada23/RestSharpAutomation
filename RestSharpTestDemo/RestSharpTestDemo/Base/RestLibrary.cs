using RestSharp;

namespace RestSharpTestDemo.Base;

public interface IRestLibrary
{
    RestClient RestClient { get; }
}

public class RestLibrary : IRestLibrary
{

    
    public RestLibrary()
    {
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        };

        RestClient = new RestClient(restClientOptions);
    }
    
    public RestClient RestClient { get; }

    
}
