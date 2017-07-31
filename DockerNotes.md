# Manual Launch

## Running the SQL Server

```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<password>" -p 1433:1433 --name sqlserver -d microsoft/mssql-server-linux
```

## Migrate the DB

```
ConnectionStrings__DefaultConnection="Server=localhost;Database=ConferencePlanner;User Id=sa;Password=<password>;MultipleActiveResultSets=true" dotnet ef database update
```

## Create a SQL Login/User with access to Conference Planner db

User Name: `backend`

Steps TODO :)

## Running the Back End

```
docker run --link sqlserver -e "ConnectionStrings__DefaultConnection=Server=sqlserver;Database=ConferencePlanner;User Id=backend;Password=<password>;MultipleActiveResultSets=true" -e "ASPNETCORE_ENVIRONMENT=Development" -p 5100:5100 --name backend anurseconferenceplanner.azurecr.io/backend:anurse-docker
```

## Running the Front End

```
docker run --link backend -e "ServiceUrl=http://backend:5100" -e "Authentication__ClientId=<clientid>" -e "Authentication__ClientSecret=<clientsecret>" -e "Authentication__Tenant=<tenant>" -e "ASPNETCORE_ENVIRONMENT=Development" -p 5101:5101 anurseconferenceplanner.azurecr.io/frontend:anurse-docker
```