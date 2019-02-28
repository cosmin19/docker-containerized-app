
# ###################################### PRODUCTION START ######################################
# Sample contents of Dockerfile
# Stage 1
FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /source

# caches restore result by copying csproj file separately
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

# Stage 2
FROM microsoft/dotnet
WORKDIR /app
COPY --from=builder /app .

# ENTRYPOINT ["dotnet", "Enviroself.dll"]
# ###################################### PRODUCTION END ######################################


# ###################################### PRODUCTION COMMANDS ######################################
# docker build -f Dockerfile -t enviroself-server-cdn .
# docker run -it -d --rm -p 5001:56199 enviroself-server-cdn

# ###################################### DATABASE COMMANDS ######################################
# RUN: docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=Agent007@' --name SqlServer -p 1401:1433 -d microsoft/mssql-server-linux:2017-latest
# CONNECTION STRING: Server=tcp:10.0.75.1,1401;Initial Catalog=EnviroselfDB;Persist Security Info=False;User ID=sa;Password=Agent007@;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;Connection Timeout=30;
# MS-SQL: Server:localhost,1401 / 10.0.75.1,1401    Login: sa   Password: Agent007@
