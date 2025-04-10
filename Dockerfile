# Use the official .NET 7 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HL7ParserAPI.csproj", "./"]
RUN dotnet restore "./HL7ParserAPI.csproj"
COPY . .
RUN dotnet publish "./HL7ParserAPI.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
CMD ["dotnet", "HL7ParserAPI.dll"]
