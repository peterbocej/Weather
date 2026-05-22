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
* #### Weather.API
This is the main project of the solution, which contains the API implementation.
* #### Weather.Application
This project contains the application logic and services used by the API.
* #### Weather.Domain
This project contains the domain models and entities used in the application.
* #### Weather.Infrastructure
This project contains the infrastructure code, such as database access and external API integrations.
## Prepare
To prepare the project for development, you need to have .NET 10.0 SDK installed on your machine.

1. Open Weather.slnx in Visual Studio (2026 version or later).
1. Restore NuGet packages.
1. Rebuild solution to restore all dependencies and ensure that the project is set up correctly.

## Run
Run _Weather.API_ project