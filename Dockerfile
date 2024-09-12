FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Image-guesser.csproj", "src/"]
RUN dotnet restore "src/Image-guesser.csproj"
COPY . .
WORKDIR "/src/src/"
RUN dotnet build "Image-guesser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Image-guesser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY src/Infrastructure/Data/ImageGuesser.db /app/Infrastructure/Data/
ENTRYPOINT [ "dotnet", "Image-guesser.dll" ]
