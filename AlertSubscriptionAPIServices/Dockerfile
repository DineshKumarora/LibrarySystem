FROM mcr.microsoft.com/dotnet/core/aspnet:2.1

COPY bin/Release/netcoreapp2.1/publish/ App/
WORKDIR /App
ENTRYPOINT ["dotnet", "AlertSubscriptionService.dll"]
