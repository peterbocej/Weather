docker build -t weatherapi .
docker run -d -p 23232:8080 -p 23233:8081 --name CWeatherApi weatherapi:latest -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=23233 -v $env:APPDATA\microsoft\UserSecrets\:C:\Users\ContainerUser\AppData\Roaming\microsoft\UserSecrets -v $env:USERPROFILE\.aspnet\https:C:\Users\ContainerUser\AppData\Roaming\ASP.NET\Https

docker images -a
docker container list -a
docker container logs CWeatherApi -f