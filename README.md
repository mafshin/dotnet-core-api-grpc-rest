# dotnet-core-api-grpc-rest
This is a simple .NET Core API representing how to add support for `gRPC` endpoint while keeping `REST` endpoint intact.

## What it is made of?
- A data repository which is shared for all endpoints
- A REST endpoint ([data models](https://github.com/mafshin/dotnet-core-api-grpc-rest/tree/master#rest-data-models) and [Controller](https://github.com/mafshin/dotnet-core-api-grpc-rest/blob/master/PortfolioAPI/PortfolioAPI/RestApi/CustomerPortfolioController.cs))
- A gRPC endpoint ([protobuf](https://github.com/mafshin/dotnet-core-api-grpc-rest/tree/master#protobuf-data-models) file and [Service](https://github.com/mafshin/dotnet-core-api-grpc-rest/blob/master/PortfolioAPI/PortfolioAPI/Services/CustomerPortfolioService.cs))

### REST Data Models
  - Request: [CustomerPortfolioRequest.cs](https://github.com/mafshin/dotnet-core-api-grpc-rest/blob/master/PortfolioAPI/PortfolioAPI/Models/Rest/CustomerPortfolioRequest.cs)
  - Response: [CustomerPortfolioResponse.cs](https://github.com/mafshin/dotnet-core-api-grpc-rest/blob/master/PortfolioAPI/PortfolioAPI/Models/Rest/CustomerPortfolioResponse.cs)

### protobuf Data Models

Here is our `customer-portfolio.proto`

```proto
syntax = "proto3";

option csharp_namespace = "PortfolioAPI.Grpc";

package portflio;

service CustomerPortfolioService {
	rpc GetCustomerPortflio(GetCustomerPortfolioRequest) returns (GetCustomerPortfolioResponse);
}

message GetCustomerPortfolioRequest {
	int32 customerId = 1;
}

message GetCustomerPortfolioResponse {
    CustomerPortfolio customerPortfolio = 1;
}

message CustomerPortfolio {
	string customerTitle = 1;
	repeated CustomerPortfolioItem items = 2;
}

message CustomerPortfolioItem {
	string stock = 1;
	int32 count = 2;
	sint64 profitLoss = 3;
}

```


## Performance Comparison (from Client Perspective)

| Service Type | Response Length (kb) | CPU (sec) |
---------------|:------------|:----------|
| REST |  323 | 83 |
| gRPC |  142 (**0.43x**)| 18 (**0.21x**)	|

As you can see from the above simple comparison, clients which connect to our `gRPC` endpoint will be served **5** times faster
while the consumed traffic to get data is roughly **half** in comparison to the `REST` endpoint. It's northworty that the 
actual difference of response length and processing time will depend on your actual data objects but because of nature of
`gRPC` proto buffers binary format, you will probably achieve similar results.
