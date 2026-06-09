FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/SmoothLingua.Server/SmoothLingua.Server.csproj", "SmoothLingua.Server/"]
COPY ["src/SmoothLingua/SmoothLingua.csproj", "SmoothLingua/"]
RUN dotnet restore "SmoothLingua.Server/SmoothLingua.Server.csproj"
COPY src/ .
WORKDIR "/src/SmoothLingua.Server"
RUN dotnet build "SmoothLingua.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SmoothLingua.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Mount a volume at /app/data for the model file and conversation store
VOLUME ["/app/data"]
ENV SmoothLingua__ModelPath=/app/data/model.zip
ENV SmoothLingua__StoreDirectory=/app/data/conversation-store
ENTRYPOINT ["dotnet", "SmoothLingua.Server.dll"]
