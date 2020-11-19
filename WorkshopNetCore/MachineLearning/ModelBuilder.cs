using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WorkshopNetCore.MachineLearning
{
    public static class ModelBuilder
    {
        private static MLContext _mlContext = new MLContext();

        private static string _connectionString = "Data Source=146.59.229.11;Initial Catalog=Workshop;User ID=admin;Password=EPSIworkshop2020*";

        public static List<FeuForecast> FeuForecasts;


        public static void CreateModel(int idFeu, int numWeek)
        {
            string rootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            string modelPath = Path.Combine(rootDir, "MachineLearning", "Data", "MLModel.zip");

            string query = "SELECT f.matricule AS Feu, CAST(e.nbPassant as REAL) AS NbPassants, CAST(e.jour as REAL) AS Jour, CAST(e.numWeek as REAL) as Semaine " +
                "FROM etat e " +
                "INNER JOIN feu f ON e.idFeu = f.idFeu " +
                $"WHERE e.idFeu = {idFeu} AND e.numWeek = {numWeek}";

            DatabaseLoader loader = _mlContext.Data.CreateDatabaseLoader<ModelInput>();
            DatabaseSource databaseSource = new DatabaseSource(SqlClientFactory.Instance, _connectionString, query);

            IDataView dataView = loader.Load(databaseSource);
            IDataView firstWeekData = _mlContext.Data.FilterRowsByColumn(dataView, "Jour", upperBound: 1);
            IDataView nextWeekData = _mlContext.Data.FilterRowsByColumn(dataView, "Jour", lowerBound: 1);

            var forecastingPipeline = _mlContext.Forecasting.ForecastBySsa(
                outputColumnName: "ForecastedPassants",
                inputColumnName: "NbPassants",
                windowSize: 24,
                seriesLength: 1440,
                trainSize: 10080,
                horizon: 24,
                confidenceLevel: 0.95f,
                confidenceLowerBoundColumn: "LowerBoundPassants",
                confidenceUpperBoundColumn: "UpperBoundPassants");

            SsaForecastingTransformer forecaster = forecastingPipeline.Fit(firstWeekData);

            var forecastEngine = forecaster.CreateTimeSeriesEngine<ModelInput, ModelOutput>(_mlContext);
            forecastEngine.CheckPoint(_mlContext, modelPath);

            FeuForecasts = Forecast(nextWeekData, 24, forecastEngine, _mlContext);
        }

        static List<FeuForecast> Forecast(IDataView testData, int horizon, TimeSeriesPredictionEngine<ModelInput, ModelOutput> forecaster, MLContext mlContext)
        {
            ModelOutput forecast = forecaster.Predict();

            IEnumerable<FeuForecast> forecastOutput = mlContext.Data.CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
                .Take(horizon)
                .Select((ModelInput passants, int index) =>
                {
                    string jour = Enum.GetName(typeof(DayOfWeek), (int)passants.Jour);
                    float semaine = passants.Semaine;
                    float actualPassants = passants.NbPassants;
                    float lowerEstimate = Math.Max(0, forecast.LowerBoundPassants[index]);
                    float estimate = forecast.ForecastedPassants[index];
                    float upperEstimate = forecast.UpperBoundPassants[index];
                    FeuForecast feu = new FeuForecast(jour, semaine, actualPassants, lowerEstimate, estimate, upperEstimate);

                    return feu;
                });

            return forecastOutput.ToList();
        }
    }
}
