# DataEnhancementService
This service consumes telemetry from RabbitMQ, applies transformations (e.g., moving average, anomaly detection using z-score), and publishes enhanced data to another RabbitMQ queue. 
It demonstrates C# data processing and extensibility via dependency injection.