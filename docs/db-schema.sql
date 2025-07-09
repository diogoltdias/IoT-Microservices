CREATE TABLE telemetry (
    timestamp TIMESTAMPTZ NOT NULL,
    device_id VARCHAR(50) NOT NULL,
    value DOUBLE PRECISION NOT NULL,
    type VARCHAR(50) NOT NULL
);
SELECT create_hypertable('telemetry', 'timestamp');