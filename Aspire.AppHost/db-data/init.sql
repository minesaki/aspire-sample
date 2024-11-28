-- Create WeatherForecasts table
CREATE TABLE IF NOT EXISTS "WeatherForecasts"
(
    "Id" uuid NOT NULL,
    "Date" date NOT NULL,
    "TemperatureC" integer NOT NULL,
    "Summary" text,
    CONSTRAINT weatherforecast_pkey PRIMARY KEY ("Id")
);

-- Insert some sample data into WeatherForecasts table
INSERT INTO "WeatherForecasts" ("Id", "Date", "TemperatureC", "Summary")
VALUES
    ('6555da29-89f1-4207-b20d-bd8d507e7c32', '2024-03-25', 22, 'Warm'),
    ('b3d43a4a-c877-4580-89af-2e26ed7e68e2', '2024-03-22', 9, 'Cold'),
    ('b57deef9-1d55-496d-a4ac-1ac88c93a4e2', '2024-03-23', 14, 'Little bit Cold'),
    ('c84c8347-e6f4-44da-a27d-9f5c0f6ca3dd', '2024-03-21', 7, 'Cold'),
    ('ee4f05a1-2435-44b6-b868-5434cc87bcfa', '2024-03-24', 2, 'TOOOOO Cold')
ON CONFLICT DO NOTHING;