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

