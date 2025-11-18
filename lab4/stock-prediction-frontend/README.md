# Stock Prediction Frontend

A simple Next.js TypeScript frontend for the Stock Price Prediction API.

## Features

- Input form for stock data (Open, High, Low, Volume)
- Real-time prediction from ML.NET backend
- Beautiful UI with Tailwind CSS
- TypeScript for type safety

## Setup

1. Install dependencies:
```bash
npm install
```

2. Configure API URL (optional):
   - The default API URL is `http://localhost:5000`
   - Update `.env.local` if your backend runs on a different port

3. Run the development server:
```bash
npm run dev
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser

## Usage

1. Enter stock data:
   - Open Price: Opening price of the stock
   - High Price: Highest price during the day
   - Low Price: Lowest price during the day
   - Volume: Trading volume

2. Click "Predict Closing Price" to get the prediction

3. View the predicted closing price and model type

## Backend Requirements

Make sure the backend API is running on `http://localhost:5000` (or update the API URL in `.env.local`).

The backend should have CORS enabled to allow requests from the frontend.

## CORS Configuration

If you encounter CORS errors, add CORS configuration to your ASP.NET Core backend:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors("AllowFrontend");
```
