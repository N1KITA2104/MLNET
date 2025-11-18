using Microsoft.AspNetCore.Mvc;
using StockPredictionWebApi.Models;
using StockPredictionWebApi.Services;

namespace StockPredictionWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PredictionController : ControllerBase
{
    private readonly OnnxPredictionService _predictionService;

    public PredictionController(OnnxPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost("predict")]
    public ActionResult<PredictionResponse> Predict([FromBody] PredictionRequest request)
    {
        try
        {
            if (request.High < request.Low)
            {
                return BadRequest(new { error = "High price must be greater than or equal to Low price" });
            }

            if (request.High > request.Open * 2 || request.Low < request.Open * 0.5)
            {
                return BadRequest(new { error = "Price values are too extreme. High/Low should be within reasonable range of Open price." });
            }

            var stockData = new StockData
            {
                Open = request.Open,
                High = request.High,
                Low = request.Low,
                Volume = request.Volume,
                HighLowSpread = request.High - request.Low,
                Close = 0f
            };

            var predictedClose = _predictionService.Predict(stockData);

            var warning = "";
            if (predictedClose < request.Low || predictedClose > request.High)
            {
                warning = "Warning: Predicted price is outside the High-Low range. This may indicate invalid input data or model limitations.";
            }

            var response = new PredictionResponse
            {
                PredictedClose = predictedClose,
                ModelType = _predictionService.ModelType
            };

            if (!string.IsNullOrEmpty(warning))
            {
                return Ok(new { 
                    predictedClose = response.PredictedClose, 
                    modelType = response.ModelType,
                    warning = warning
                });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", modelType = _predictionService.ModelType });
    }
}

