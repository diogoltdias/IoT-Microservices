# IoT-Microservices
A C# microservices-based solution for IoT device connectivity, telemetry ingestion, data storage, and enhancement. Built with ASP.NET Core, MQTT, PostgreSQL, and Docker.

## Features
- Device registry for managing IoT device metadata.
- Telemetry ingestion via MQTT and HTTP.
- Time-series data storage using PostgreSQL (TimescaleDB).
- Data enhancement (e.g., anomaly detection, aggregation).
- REST API Gateway for client access.
- Containerized with Docker and CI/CD via GitHub Actions.

## Getting Started
See [docs/setup.md](docs/setup.md) for setup instructions.

## Architecture
See [docs/architecture.md](docs/architecture.md) for details.

[![CI](https://github.com/diogoltdias/IoT-Microservices/actions/workflows/ci.yml/badge.svg)](https://github.com/diogoltdias/IoT-Microservices/actions/workflows/ci.yml)
