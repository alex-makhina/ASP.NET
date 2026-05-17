import { useState, useEffect } from "react";
import "./Weather.css";

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

const API_BASE = import.meta.env.VITE_API_URL;

function Weather() {
  const [forecasts, setForecasts] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchWeather = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await fetch(`${API_BASE}/weatherforecast`);
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data: WeatherForecast[] = await response.json();
        setForecasts(data);
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to fetch weather data"
        );
      } finally {
        setLoading(false);
      }
    };

    fetchWeather();
  }, []);

  if (loading) {
    return <div className="weather-loading">Loading weather data...</div>;
  }

  if (error) {
    return <div className="weather-error">Error: {error}</div>;
  }

  if (forecasts.length === 0) {
    return <div className="weather-empty">No weather data available.</div>;
  }

  return (
    <div className="weather-container">
      <h2>Weather Forecast</h2>
      <table className="weather-table">
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp (°C)</th>
            <th>Temp (°F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((forecast, index) => (
            <tr key={index}>
              <td>{forecast.date}</td>
              <td>{forecast.temperatureC}</td>
              <td>{forecast.temperatureF}</td>
              <td>{forecast.summary}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default Weather;
