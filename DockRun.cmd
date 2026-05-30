docker build -t weatherapi -f .\Dockerfile .
docker run -d -p 5555:5555 --name CWeatherApi weatherapi:latest
docker container logs CWeatherApi -f