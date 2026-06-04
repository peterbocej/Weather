# Weather API project

## Description
This project is a simple Weather API built using ASP.NET Core 10. 

It provides endpoints to retrieve weather information for cities (_Bratislava, Praha, Budapest, Vienna_). 
The API is designed to be lightweight and easy to use, 
making it ideal for developers who want to integrate weather data into their applications.

## Mission
The mission of this project is to provide accurate and up-to-date weather information 
to users in a simple and efficient manner.

[net-zadanie-2026.pdf](./net-zadanie-2026.pdf): in slovak language, contains the task description and requirements for the project.

## Get
To get the project, you can clone the repository from GitHub using the following command:
```bash
git clone https://github.com/peterbocej/Weather.git
```
## Solution items
![Solution projects](./img/solution.png)
* ### docker-compose
This project contains the Docker configuration for running the API in a containerized environment.
* #### Weather.API
This is the main project of the solution, which contains the API implementation.
* #### Weather.Application
This project contains the application logic and services used by the API.
* #### Weather.Domain
This project contains the domain models and entities used in the application.
* #### Weather.Infrastructure
This project contains the infrastructure code, such as database access and external API integrations.
## Prepare
To prepare the project for development, you need to have .NET 8.0 SDK installed on your machine.

1. Goto the root directory of the project in the terminal and run the following command to restore NuGet packages and build the solution:
2. Restore NuGet packages.
```
dotnet restore .\Weather.slnx
```
3. Build the solution.
```
dotnet build .\Weather.slnx
```

## Setup
1. Register for a free API key at [Weather API](https://www.weatherapi.com/) to access weather data.
2. In _appsettings.json_ file of the Weather.API project, replace the placeholder API_KEY in _AppSettings->WeatherApiServer->ApiKey_ with your actual Weather API key.
3. In _appsettings.json_ file of the Weather.API project, replace the placeholder SECRET_KEY in _AppSettings->Jwt->Key_ with a secure key of your choice for JWT authentication.
4. Set _AppSettings->Cache->Mode_ to "None", "Memory", or "Database" based on your preference.
## Run Locally
1. Run _Weather.API_ project (with http, https or Container settings) to start the API server.
```
dotnet run --project .\Weather.API\Weather.API.csproj
```
2. Goto [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html) in your web browser to access the Swagger UI for the API.
## Run with Docker
1. Go to the root directory of the project in the terminal.
2. Compose project into a Docker container by running the following command in the terminal from the root directory of the project:
```bash
docker compose up --build
```
3. Goto [https://localhost:56566/swagger](https://localhost:56566/swagger) in your web browser to access the Swagger UI for the API.
4. Follow steps 2-5 from the **_Interact with the API using Swagger UI_** section to interact with the API using the Docker container.
## Interact with the API
1. Register new user by _api/Auth/register_. Use role *User* or _Administrator_.
2. Authenticate by sending a POST request to _/api/auth/login_ endpoint with the user credentials in the request body.
3. Use response from the login endpoint to obtain a JWT token, which will be used for authenticated requests to the API.
4. Get weather information for a city by sending a GET request to _/api/temperature/cityId_ endpoint, replacing cityId with the number of the desired city. Include the JWT token in the Authorization header of the request.
