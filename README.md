# SantApi

## Description
This application exposes an API that allows retrieving top stories from Hacker News. The main endpoints are:

- `GET /api/stories?n={n}` - Retrieves a list of the top stories. The number `n` specifies how many stories to retrieve. The default value is 10.


## How to Run the Application

To run the application locally, follow these steps:

1. **Clone the repository:**
   ```bash
   git clone https://github.com/grubyKarlos/SantApi.git
##

   ## Assumptions ##
1. Data Sorting: It is assumed that the endpoint /v0/beststories.json returns data that is sorted by score in descending order (highest score first).

2. Default Values:
- The default number of stories (n) for the /api/stories?n={n} endpoint is 10.
- The maximum number of stories (n) that can be requested is 200, which is defined in the appsettings.json file.

3. Caching is applied to optimize performance. The cache expiration time is set to 5 minutes, as specified in the application settings.

## Changes and Enhancements ## 
- Expand existing unit and integration tests to cover more scenarios, such as invalid requests, no data response from the external API, or issues with the cache.
- Add logs

## Technologies Used ##
- ASP.NET Core 8.0 – Framework for building web applications.
- InMemoryCache – Used for caching results to improve performance.
- xUnit – Framework for writing unit and integration tests.
- Moq – Library for mocking dependencies in unit tests.
- Swagger – API documentation.
  
## Known Issues and Limitations ##
- Currently, the application does not handle server errors in a detailed manner – all errors result in a 500 status code response.
