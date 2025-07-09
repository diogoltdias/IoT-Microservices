# Architecture

## Overview
This project implements a microservices architecture for IoT, with:
- **Device Registry Service**: Manages device metadata (MongoDB).
- **Telemetry Ingestion Service**: Receives data via MQTT, forwards to RabbitMQ.
- **Data Storage Service**: Stores time-series data (PostgreSQL/TimescaleDB).
- **Data Enhancement Service**: Processes data (e.g., anomaly detection).
- **API Gateway**: Exposes REST APIs for clients.

## Diagram
(TODO: Embed a diagram using Mermaid.js or link to an image.)

## Communication
- MQTT for device-to-cloud.
- RabbitMQ for inter-service events.
- REST for client interactions.