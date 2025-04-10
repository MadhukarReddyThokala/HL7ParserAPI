using NHapi.Base.Parser;
using NHapi.Model.V25.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System;
using NHapi.Base.Model;

public class HL7ParserService : IHL7ParserService
{
    private readonly ILogger<HL7ParserService> _logger;

    public HL7ParserService(ILogger<HL7ParserService> logger)
    {
        _logger = logger;
    }

    public string Parse(string hl7Message)
    {
        try
        {
            _logger.LogInformation("Started parsing HL7 message.");

            var pipeParser = new PipeParser();
            IMessage parsedMessage = pipeParser.Parse(hl7Message);

            // Prevent self-referencing loops in JSON serialization
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string jsonOutput = JsonConvert.SerializeObject(parsedMessage, Formatting.Indented, settings);
            JObject jsonObject = JObject.Parse(jsonOutput);

            var firstTwoObjects = new JObject();
            int count = 0;
            foreach (var prop in jsonObject.Properties())
            {
                if (count < 2)
                {
                    firstTwoObjects[prop.Name] = prop.Value;
                    count++;
                }
                else
                {
                    break;
                }
            }

            _logger.LogInformation("HL7 message parsed successfully.");
            return firstTwoObjects.ToString(Formatting.Indented);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error parsing HL7 message: {ex.Message}");
            throw new ApplicationException("Failed to parse HL7 message", ex);
        }
    }

    public string ParseXML(string hl7Message)
    {
        try
        {
            _logger.LogInformation("Started parsing HL7 message to XML.");

            var pipeParser = new PipeParser();
            IMessage parsedMessage = pipeParser.Parse(hl7Message);
            XMLParser xmlParser = new DefaultXMLParser();
            string xmlOutput = xmlParser.Encode(parsedMessage);

            _logger.LogInformation("HL7 message parsed successfully to XML.");
            return xmlOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error parsing HL7 message to XML: {ex.Message}");
            throw new ApplicationException("Failed to parse HL7 message to XML", ex);
        }
    }
}
