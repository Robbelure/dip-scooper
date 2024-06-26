# DipScooper

DipScooper is a tool for analyzing stock data, helping you scoop up the dips in the stock market by calculating various financial indicators.

![image](images/screenshotDipScooper2.PNG)

## Installation

Follow these steps to set up the project locally:

1. Clone the repository:
    ```sh
    git clone https://github.com/username/dip-scooper.git
    ```
2. Navigate to the project directory:
    ```sh
    cd dip-scooper
    ```
3. Open the solution in your favorite IDE (e.g., Visual Studio).
4. Build the solution to install the necessary packages.

## Usage

1. Start the application from your IDE.
2. Enter the stock ticker symbol you want to analyze in the search box.
3. Select the desired date range.
4. Click "Search" to fetch and display the stock data.

## Structure

The project is divided into four main parts:

- **DipScooper.UI**: The user interface and entry point of the application.
- **DipScooper.BLL**: The business logic that handles calculations and stock analysis.
- **DipScooper.DAL**: The data access layer that handles communication with the database and external APIs.
- **DipScooper.Models**: Models used in the application, including data and API response objects.

## Data source

DipScooper retrieves stock data from the Polygon.io API. You will need an API key from Polygon.io to fetch the data. Add your API key in the ApiClient-class.

### Getting a Polygon.io API Key

1. Go to [Polygon.io](https://polygon.io/).
2. Sign up for a free account.
3. Navigate to the API keys section and generate a new API key.
4. Add this API key to ApiClient.cs in the DipScooper.DAL project.

## Database

DipScooper uses MongoDB to store stock data. Ensure you have MongoDB installed and running on your machine. 
Update the connection string in the configuration file of the DipScooper.DAL project to point to your MongoDB instance.

## Limitations

Currently, DipScooper has the following limitations:

- Lack of real-time data fetching.
- No support for automated trading.
- Limited to daily and weekly data granularity.
- No alert system for market dips yet.
- Using the free version of the Polygon.io API, which limits access to only 2 years of historical data per stock and allows only 5 requests per minute.

## Future plans/Considerations

Develop an algorithm that will alert users to any "dip" in the stock market that is considered profitable. 
This is a challenging task, as it involves many factors and decisions. Key considerations include:

- **Profitability Assessment**: What criteria will determine that a dip is profitable? This could involve analyzing historical data, market trends, company performance, and other indicators.
- **Control Mechanisms**: How will the algorithm be controlled and fine-tuned to improve accuracy and reliability? Regular backtesting and adjustments will be necessary.
- **Timeframe Expectations**: What is the expected timeframe for a stock to recover and become profitable after a dip? Users need to know how long they _might have to hold a stock before seeing a return on investment.
- **Defining a Bad Dip**: What constitutes a "bad" dip? This could be a dip that does not recover within the expected timeframe or results in a loss,
  or a dip that occurs due to fundamental issues in the company, such as poor earnings reports or significant changes in management.
  RSI values can sometimes give false signals in such cases, as they might indicate oversold conditions despite the underlying issues.

