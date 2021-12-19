# Wooden Workshop

## Setup Guid

### Environment Setup
At first, you need to setup the follwing:
* Azure SQL or SQL Server
* Redis

To set up it using docker follow the next steps:
1. Install and run docker engine
2. Pull Azure SQL image using the following command
```
docker pull mcr.microsoft.com/azure-sql-edge:latest
```
3. Pull redis image using the following command
```
docker pull redis
```
4. Create volume using to store Azure SQL files
```
docker volume create sql-storage
```
5. Create and run container
```
docker run -d -v sql-storage --name azure-sql -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=PTP4ptp-' -p 1433:1433 mcr.microsoft.com/azure-sql-edge
```
6. Create and run redis container
```
docker run --name redis-server -p 6379:6379 -d redis
```

### Running the App
Run app :)
