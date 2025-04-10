1. Build a API in .NET Core 7.0

2. The API should be accept a HL7 messages and return the parsed HL7 as json (There are libraries that support with parsing HL7 -> JSON)

3. Should be able to Insert this parsed JSON into a database of choice. Get the unique id for this inserted record.

4. Publish an event into rabbit message queue with the unique id in the header and the json as the message body.

5. Use the GCP lab environment and host the API in GCP or any other hosting environment of your choice.
