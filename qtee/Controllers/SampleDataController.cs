using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Qtee.Contracts.Processors;

namespace qtee.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        public SampleDataController(IQProcessorSwicher processorSwicher, IQTaskResult<decimal> qTaskResult)
        {
            QProcessorSwicher = processorSwicher;
            QTaskResult = qTaskResult;
        }

        private IQProcessorSwicher QProcessorSwicher { get; set; }
        private IQTaskResult<decimal> QTaskResult { get; set; }

        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();

            if (QProcessorSwicher is IQProcessorData<IQTaskResult<decimal>, decimal> counterProcessor)
            {
                counterProcessor.Add(QTaskResult);
                QProcessorSwicher.StartProcessing();

                var waitForState = QProcessorSwicher.WaitForState(QProcessorState.Wainting);
                waitForState.Wait(1000);
                if (waitForState.Result)
                {
                    QProcessorSwicher.StopProcessing();
                    var result = counterProcessor.GetLastOrNull();
                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = $"{Summaries[rng.Next(Summaries.Length)]} - {result}"
                    });
                }
            }

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get { return 32 + (int) (TemperatureC / 0.5556); }
            }
        }
    }
}