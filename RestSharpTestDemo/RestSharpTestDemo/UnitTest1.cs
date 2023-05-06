using System.Net;
using FluentAssertions;
using GraphQLProductApp.Data;
using RestSharp;
using RestSharp.Authenticators;
using Xunit.Abstractions;

namespace RestSharpTestDemo;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _output = testOutputHelper;
    }

    [Fact]
    public async Task Test1()
    {
        // Remove the ssl certification error in this case
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }; 
        // Rest Client initialization
        var client = new RestClient(restClientOptions);
        // Rest Request: Class to perform the actual request
        var request = new RestRequest("Product/GetProductById/1");
        // Perform GET Operation
        var response = await client.GetAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        response?.Name.Should().BeEquivalentTo("keyboard");
    }
    
    [Fact]
    public async Task GetQuerySegment()
    {
        // Remove the ssl certification error in this case
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }; 
        // Rest Client initialization
        var client = new RestClient(restClientOptions);
        // Rest Request: Class to perform the actual request
        var request = new RestRequest("Product/GetProductById/{id}");
        request.AddUrlSegment("id", 2);
        // Perform GET Operation
        var response = await client.GetAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        response?.Name.Should().BeEquivalentTo("Monitor");
        response?.Price.Should().Be(400);
    }
    
    [Fact]
    public async Task GetWithQueryParameter()
    {
        // Remove the ssl certification error in this case
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }; 
        // Rest Client initialization
        var client = new RestClient(restClientOptions);
        // Rest Request: Class to perform the actual request
        var request = new RestRequest("Product/GetProductByIdAndName");
        request.AddQueryParameter("id", 2);
        request.AddQueryParameter("name", "Monitor");
        // Perform GET Operation
        var response = await client.GetAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        response?.Name.Should().BeEquivalentTo("Monitor");
        response?.Price.Should().Be(400);
    }
    
    [Fact]
    public async Task PostJsonBody()
    {
        // Remove the ssl certification error in this case
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        }; 
        // Rest Client initialization
        var client = new RestClient(restClientOptions);
        // Rest Request: Class to perform the actual request
        var productName = "ProductName";
        var price = 209;
        var request = new RestRequest("Product/Create");
        // This will serialize the Product Object to JSON
        request.AddJsonBody(new Product()
        {
            Name = productName,
            Description = "Description f",
            ProductType = ProductType.EXTERNAL,
            Price = price
        });
        // Perform GET Operation
        var response = await client.PostAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        response?.Name.Should().BeEquivalentTo(productName);
        response?.Price.Should().Be(price);
    }
    
    [Fact]
    public async Task FileUpload()
    {
        // Remove the ssl certification error in this case
        var restClientOptions = new RestClientOptions()
        {
            BaseUrl = new Uri("https://localhost:5001"),
            RemoteCertificateValidationCallback = (_, _, _, _) => true,
            Authenticator = new JwtAuthenticator("bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2ODMzMjc3MjksImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzcxIiwiYXVkIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzNzEifQ.Vyehn0w5XdzbrvmlcAKEnn14x1vVipzrwjR3IJTeWYM")
        }; 
        // Rest Client initialization
        var client = new RestClient(restClientOptions);
        // Rest Request: Class to perform the actual request
        var request = new RestRequest("Product", Method.Post);
        request.AddFile("myFile", @"/Users/camilo.posadaa/Pictures/test.png", "multipart/form-data");
            
        // Perform GET Operation
        var response = await client.ExecuteAsync<Product>(request);
        // Assert
        _output.WriteLine("Doing assertions");
        // Verify http status code created
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    
}