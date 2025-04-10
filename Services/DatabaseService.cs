using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class DatabaseService : IDatabaseService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(AppDbContext context, ILogger<DatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool CanConnectToDatabase()
    {
        return _context.Database.CanConnect();
    }

    public async Task<Guid> InsertHL7Message(string parsedMessage)
    {
        try
        {
            var record = new HL7Message { Id = Guid.NewGuid(), Data = parsedMessage };
            _context.test2Tbl.Add(record);

            _logger.LogInformation($"Inserting HL7 message with ID {record.Id} into the database.");

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Successfully inserted HL7 message with ID {record.Id}.");
            return record.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting HL7 message into the database: {ex.Message}");
            throw new ApplicationException("Failed to insert HL7 message into the database.", ex);
        }
    }
}
