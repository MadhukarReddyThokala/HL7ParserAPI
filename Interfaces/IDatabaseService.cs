
public interface IDatabaseService
{
    bool CanConnectToDatabase();
    Task<Guid> InsertHL7Message(String parsedMessage);
}
