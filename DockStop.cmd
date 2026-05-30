docker container stop CWeatherApi
docker container rm CWeatherApi
docker image rm weatherapi:latest

docker images -a
docker container list -a