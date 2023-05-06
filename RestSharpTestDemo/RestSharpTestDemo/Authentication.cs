using System.Net;
using FluentAssertions;
using GraphQLProductApp.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using Xunit.Abstractions;

namespace RestSharpTestDemo;

public class Authentication
{
    private readonly ITestOutputHelper _output;
    private RestClientOptions _restClientOptions;

    public Authentication(ITestOutputHelper testOutputHelper)
    {
        _output = testOutputHelper;
        _restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }; 
    }

    [Fact]
    public async Task AuthenticationTest()
    {
        var client = new RestClient(_restClientOptions);

        var request = new RestRequest("api/Authenticate/Login");

        // Using an anonymous object as the body of the request
        request.AddJsonBody(new
        {
            username = "KK",
            password = "123456"
        });

        var authResponse = client.PostAsync(request).Result.Content;

        // Extract the token from the response
        var token = JObject.Parse(authResponse!)["token"];
        _output.WriteLine($"Token: {token}");

        var productGetRequest = new RestRequest("Product/GetProductById/1");
        productGetRequest.AddHeader("Authorization", $"Bearer {token}");

        var productResponse = await client.GetAsync<Product>(productGetRequest);
        productResponse!.Name.Should().Be("Keyboard");

    }
    


    
    [Fact]
    public async Task FileUpload()
    {
        var token = GetToken();
        // Rest Client initialization
        var client = new RestClient(_restClientOptions);
        // Rest Request: Class to perform the actual request
        var request = new RestRequest("Product", Method.Post);
        request.AddFile("myFile", @"/Users/camilo.posadaa/Pictures/test.png", "multipart/form-data");
        request.AddHeader("Authorization", $"Bearer {token}");
        // Perform POST Operation
        var response = await client.ExecuteAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        // Verify http status code created
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private string GetToken()
    {
        var client = new RestClient(_restClientOptions);

        var request = new RestRequest("api/Authenticate/Login");

        // Using an anonymous object as the body of the request
        request.AddJsonBody(new
        {
            username = "KK",
            password = "123456"
        });

        var authResponse = client.PostAsync(request).Result.Content;

        // Extract the token from the response
        var token = JObject.Parse(authResponse!)["token"];

        return token!.ToString();
    }


    
    
}