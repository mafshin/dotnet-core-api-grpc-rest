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

## Response Length
| Service Type | Response Length (kb) |
---------------|:------------|
| REST |  323 |
| gRPC |  142 (**0.43x**)|

gRPC uses binary protocol for data transfer so response is nearly half the length of REST response in our sample.

## Performance Comparison (Client)

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19577
Intel Core i7-6700 CPU 3.40GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT


```
|  Method | count |       Mean |    Error |   StdDev | Ratio |      Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|-------- |------ |-----------:|---------:|---------:|------:|-----------:|----------:|----------:|----------:|
| RESTApi |  2000 | 1,908.0 ms | 35.96 ms | 33.63 ms |  1.00 | 21000.0000 | 7000.0000 | 2000.0000 | 117.08 MB |
| GrpcApi |  2000 |   194.9 ms |  9.69 ms | 28.27 ms |  **0.10** |  5000.0000 | 2000.0000 |         - |  24.94 MB |

