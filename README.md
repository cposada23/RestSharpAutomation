# RestSharp

> Application used in this document: https://github.com/executeautomation/GraphQL.NET. Download it and run it.

- Create a new xUnit project
- Install the dependency for RestSharp from NuGet: `RestSharp``
- Install the dependency  to do the assertions: `FluentAssertions`
- Test it works
```c#
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
        var response = await client.GetAsync(request);
        // Assert
        _output.WriteLine("Helloooo");
        response.Should().NotBeNull();
    }

```
## GET

### Type cast the response to our own type

In this case we are working with products, this is the model for the product:
> Create a new folder `Models` and create a new class -> Product

Product Class:
```c#
namespace RestSharpTestDemo.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public ICollection<Components> Components { get; set; }

    public ProductType ProductType { get; set; }
}

public enum ProductType
{
    CPU,
    MONITOR,
    PERIPHARALS,
    EXTERNAL,
    PROCESSOR,
    MEMORY
}
```

Components Class:

```c#
namespace RestSharpTestDemo.Models;

public class Components
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int? ProductId { get; set; }
    public Product Product { get; set; }
}
```

This way we can cast the response with the product type like this `var response = await client.GetAsync<Product>(request);`

```c#
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
```

### URL Segments

Instead of passing the url to the the request object like this `var request = new RestRequest("Product/GetProductById/1");` we can use url segments to pass variables to the request ( In this case the id of the product ) like this:

```c#
var request = new RestRequest("Product/GetProductById/{id}");  
request.AddUrlSegment("id", 1);
```

### Query Parameters
`https://localhost:5001/Product/GetProductByIdAndName?Id=2&Name=Monitor`
```c#
var request = new RestRequest("Product/GetProductByIdAndName");  
request.AddQueryParameter("id", 2);  
request.AddQueryParameter("name", "Monitor");
```


## POST

#### Post json body


```c#
        var request = new RestRequest("Product/Create");
        // This will serialize the Product Object to JSON
        request.AddJsonBody(new Product()
        {
            Name = productName,
            Description = "Description f",
            ProductType = ProductType.EXTERNAL,
            Price = price
        });

```


#### Multipart/form-data

```c#

var request = new RestRequest("Product", Method.Post);
request.AddFile("myFile", @"/Users/camilo.posadaa/Pictures/test.png", "multipart/form-data");
// Perform POST Operation
var response = await client.ExecuteAsync<Product>(request);
```

#### Bearer Token

`request.AddHeader("Authorization", $"Bearer {token}");`


#### IClassFIxture

extend IClassFixture interface: `public class BaseTests : IClassFixture<RestLibrary>`
add the parameter to be injected in the constructor: `public BaseTests(ITestOutputHelper testOutputHelper, RestLibrary restLibrary)`


#### Dependency Injection
Is recommended to use interfaces, this will allow DI

> xunit dependency injection: https://github.com/pengweiqhca/Xunit.DependencyInjection

Create an interface of the class you want to inject, In this case I'll create an interface for the RestLibrary.cs

```c#
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

```

And now register the Interface, with the help of the xUnit dependency injection:
> Intall Xunit.DependencyInjection package

Startup.cs file:
```c#
using Microsoft.Extensions.DependencyInjection;
using RestSharpTestDemo.Base;

namespace RestSharpTestDemo;

public class Startup
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IRestLibrary, RestLibrary>();
    }
}

```


### Using the BUILDER PATTER

Basically a method that will return the same instance of the class so you can chain operations

```c#
public IRestBuilder WithRequest(string request)  
{  
RestRequest = new RestRequest(request);  
return this;  
}  
  
public IRestBuilder WithHeader(string request)  
{  
RestRequest = new RestRequest(request);  
return this;  
}
```

then I can do something like this: _restBuilder.WithRequest("bla").WithHeader("bla");



So the IRestBuilder class now looks like this:

```c#

using RestSharp;

namespace RestSharpTestDemo.Base;

public interface IRestBuilder
{
    IRestBuilder WithRequest(string request);
    IRestBuilder WithHeader(string name, string value);
    IRestBuilder WithQueryParameter(string name, string value);
    IRestBuilder WithUrlSegment(string name, string value);
    IRestBuilder WithBody(object body);
    Task<T?> WithGet<T>();
    Task<T?> WithPost<T>();
    Task<T?> WithPut<T>();
    Task<T?> WithDelete<T>();
    Task<T?> WithPatch<T>();
}

public class RestBuilder : IRestBuilder
{
    private readonly IRestLibrary _restLibrary;

    private RestRequest RestRequest { get; set; } = null!;

    public RestBuilder(IRestLibrary restLibrary)
    {
        _restLibrary = restLibrary;
    }

    public IRestBuilder WithRequest(string request)
    {
        RestRequest = new RestRequest(request);
        return this;
    }

    public IRestBuilder WithHeader(string name, string value)
    {
        RestRequest.AddHeader(name, value);
        return this;
    }

    public IRestBuilder WithQueryParameter(string name, string value)
    {
        RestRequest.AddQueryParameter(name, value);
        return this;
    }

    public IRestBuilder WithUrlSegment(string name, string value)
    {
        RestRequest.AddUrlSegment(name, value);
        return this;
    }

    public IRestBuilder WithBody(object body)
    {
        RestRequest.AddJsonBody(body);
        return this;
    }

    public async Task<T?> WithGet<T>()
    {
        return await _restLibrary.RestClient.GetAsync<T>(RestRequest);
    }
    
    public async Task<T?> WithPost<T>()
    {
        return await _restLibrary.RestClient.PostAsync<T>(RestRequest);
    }
    
    public async Task<T?> WithPut<T>()
    {
        return await _restLibrary.RestClient.PutAsync<T>(RestRequest);
    }
    
    public async Task<T?> WithDelete<T>()
    {
        return await _restLibrary.RestClient.DeleteAsync<T>(RestRequest);
    }
    
    public async Task<T?> WithPatch<T>()
    {
        return await _restLibrary.RestClient.PatchAsync<T>(RestRequest);
    }

}
```


#### Factory Pattern

We need a way to create the RequestBuilder, so we need a class that has that in the costructor and we use dependecy injection to create it

```c#
namespace RestSharpTestDemo.Base;

public interface IRestFactory
{
    IRestBuilder Create();
}

public class RestFactory : IRestFactory
{
    private readonly IRestBuilder _restBuilder;

    public RestFactory(IRestBuilder restBuilder)
    {
        _restBuilder = restBuilder;
    }

    public IRestBuilder Create()
    {
        return _restBuilder;
    }

}
```


> Don't forget to register all the services for dependency injection:

```c#

namespace RestSharpTestDemo;

public class Startup
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IRestLibrary, RestLibrary>()
            .AddScoped<IRestFactory, RestFactory>()
            .AddScoped<IRestBuilder, RestBuilder>();
    }
}
```


And then using the builder and the factory patters, the test code looks like this (much more cleaner)


```c#

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
```