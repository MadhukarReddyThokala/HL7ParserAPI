public interface IHL7ParserService
{
    string Parse(string hl7Message);
    string ParseXML(string hl7Message);
}
