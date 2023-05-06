using System.Net;
using FluentAssertions;
using GraphQLProductApp.Controllers;
using GraphQLProductApp.Data;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharpTestDemo.Base;
using Xunit.Abstractions;

namespace RestSharpTestDemo;

public class BaseTests
{
    private readonly ITestOutputHelper _output;
    private readonly IRestFactory _restFactory;
    private readonly string _token;

    public BaseTests(ITestOutputHelper testOutputHelper, IRestFactory restFactory)
    {
        _output = testOutputHelper;
        _restFactory = restFactory;
        _token = GetToken();
    }


    [Fact]
    public async Task AuthenticationTest()
    {
        var productResponse = await _restFactory.Create()
            .WithRequest("Product/GetProductById/1")
            .WithHeader("Authorization", $"Bearer {_token}")
            .WithGet<Product>();

        productResponse!.Name.Should().Be("Keyboard");
    }
    
    [Fact]
    public async Task FileUpload()
    {
        var response = await _restFactory.Create()
            .WithRequest("Product")
            .WithHeader("Authorization", $"Bearer {_token}")
            .WithFile("myFile", @"/Users/camilo.posadaa/Pictures/test.png", "multipart/form-data")
            .WithPost();
        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private string GetToken()
    {
        var authResponse = _restFactory.Create()
            .WithRequest("api/Authenticate/Login")
            .WithBody(new
            {
                username = "KK",
                password = "123456"
            })
            .WithPost()
            .Result
            .Content;

        // Extract the token from the response
        var token = JObject.Parse(authResponse!)["token"];
        _output.WriteLine($"Token: {token}");
        return token!.ToString();
    }

}