# Data files folder
## WeatherCache.db*
This is the SQLite database file that stores cached weather data. 
It is used to improve performance by reducing the need for repeated API calls to fetch weather information. 
The application reads from and writes to this database to manage cached weather data efficiently.

File is created and managed by the application, and it should not be manually edited to avoid data corruption.