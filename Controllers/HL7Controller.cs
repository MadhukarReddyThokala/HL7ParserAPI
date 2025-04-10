using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/hl7")]
public class HL7Controller : ControllerBase
{
    private readonly IHL7ParserService _parserService;
    private readonly IDatabaseService _dbService;
    private readonly IRabbitMQService _mqService;    
    private readonly ILogger<HL7Controller> _logger;

    public HL7Controller(IHL7ParserService parserService, 
                         IDatabaseService dbService, 
                         IRabbitMQService mqService,                          
                         ILogger<HL7Controller> logger)
    {
        _parserService = parserService;
        _dbService = dbService;
        _mqService = mqService;        
        _logger = logger;
    }      

    // GET api/hl7/dbstatus
    [HttpGet("dbstatus")]
    public IActionResult CheckDatabaseConnection()
    {
        try
        {
            bool canConnect = _dbService.CanConnectToDatabase();
            _logger.LogInformation($"Database connection status: {(canConnect ? "Healthy" : "Unavailable")}");
            return Ok(new { DatabaseConnection = canConnect ? "Healthy - Available" : "Unavailable" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection failed");
            return StatusCode(500, new { Message = "Database connection failed", Error = ex.Message });
        }
    }

    // HL7Request model
    public class HL7Request
    {
        public string? Hl7Message { get; set; }
    }   

    // POST api/hl7/parse-hl7
    [HttpPost("parse-hl7")]
    public async Task<IActionResult> ParseHL7Async([FromBody] string request)
    {
        if (string.IsNullOrEmpty(request))
        {
            _logger.LogWarning("Empty HL7 message received");
            return BadRequest(new { error = "HL7 message is required" });
        }

        try
        {
            _logger.LogInformation("Starting HL7 message parsing");
            var parsedMessage = _parserService.Parse(request);
            var recordId = await _dbService.InsertHL7Message(parsedMessage);
            _mqService.Publish(recordId, parsedMessage);

            _logger.LogInformation("HL7 message parsed and published to queue");
            return Ok(new { id = recordId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing HL7 message");
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST api/hl7/parsetoxml
    [HttpPost("parsetoxml")]
    [Consumes("text/plain")]
    public async Task<IActionResult> ParseHL7Xml()
    {
        using var reader = new StreamReader(Request.Body);
        string requestBody = await reader.ReadToEndAsync();    

        if (string.IsNullOrEmpty(requestBody))
        {
            _logger.LogWarning("Empty XML message received");
            return BadRequest(new { error = "XML message is required" });
        }

        try
        {
            _logger.LogInformation("Starting HL7 XML parsing");
            var parsedMessage = _parserService.ParseXML(requestBody);
            var recordId = await _dbService.InsertHL7Message(parsedMessage);
            _mqService.Publish(recordId, parsedMessage);

            _logger.LogInformation("HL7 - XML parsed and published to queue");
            return Ok(parsedMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing HL7 - XML message");
            return BadRequest(new { error = ex.Message });
        }        
    }
}
