# TradingDataSimulator

It is a simple price simulator that generates new prices with +-2% movement for different stocks.
Program uses several communication methods
	* Rest API
	* SignalR
	* Tcp

## Setup instructions

Install .NET SDK 8.0
https://dotnet.microsoft.com/en-us/download/dotnet/8.0

Use any IDE: Visual Studio 2022 17.8+, JetBrains Rider, VS Code

Clone the repository using the command line or your IDE:

	git clone <URL of Repository>

## Run the application

Run the application using the IDE or command line:

Open the solution file `TradingDataSimulator.sln` in your IDE or use the command line to navigate to the solution directory.

IDE:

Set the `MainHost` project as the startup project.
Press F5 or click on the "Start" button in your IDE 

Command line:

	dotnet run --project MainHost


## Access the API

Open your web browser and navigate to the following URL to access the API documentation while MainHost is running:
	
	https://localhost:7137/swagger/index.html

The following endpoints are available:

	GET /api/prices - Get all prices
	GET /api/prices/{symbol} - Get last price for a specific stock symbol
	GET /api/prices/{symbol}/history - Get historical prices for a specific stock symbol

You can use a tool like Postman to interact with the API endpoints.

Or you can use the command line with `curl` to test the endpoints:

	curl https://localhost:7137/api/prices
	curl https://localhost:7137/api/prices/AAPL
	curl https://localhost:7137/api/prices/AAPL/history

All endpoints are documented in the Swagger UI.

## Run the Tcp and SignalR clients

To run the Tcp or SignalR clients, right-click on the `TcpClient` or `SignalRClient` project and select Debug -> Start without Debugging/Start new instance.

Or use the command line to navigate to the solution directory while MainHost is running and run commands:

	dotnet run --project TcpConsoleClient
	dotnet run --project SignalRConsoleClient

## Build formatter plugins

To build plugins right-click on `JsonFormatter` or `CsvFormatter` project in the Solution Explorer and select "Build" or "Rebuild".

It will automatically copy the output files to the `Plugins` directory in the `MainHost` project.

Or in the command line, navigate to the solution directory where `TradingDataSimulator.sln` is located and run the following command:

	dotnet build JsonFormatter
	dotnet build CsvFormatter

After building, the plugins will be available in the `bin/Debug/net8.0` directory of each project.

Move the built plugin files to the `Plugins` directory in the `MainHost` project.

## Run the tests

Run the tests using the IDE or command line:

IDE: 

Right-click on the solution and select "Run Tests"

Comand line : 

	dotnet test
