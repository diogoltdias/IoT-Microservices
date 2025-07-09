# DataStorageService
This service consumes telemetry messages from RabbitMQ, stores them in a PostgreSQL database with the TimescaleDB extension for time-series data, and exposes REST endpoints to query telemetry by device ID and time range.
It uses Entity Framework Core (EF Core) with Npgsql.EntityFrameworkCore.PostgreSQL for database access.