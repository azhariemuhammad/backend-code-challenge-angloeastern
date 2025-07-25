INSERT INTO "Ports" ("Name", "Country", "Latitude", "Longitude", "CreatedAt") VALUES
-- Major European Ports
('Port of Rotterdam', 'Netherlands', 51.92250000, 4.47917000, NOW()),
('Port of Hamburg', 'Germany', 53.55110000, 9.99370000, NOW()),
('Port of Antwerp', 'Belgium', 51.22130000, 4.40510000, NOW()),
('Port of Le Havre', 'France', 49.49440000, 0.10790000, NOW()),
('Port of Felixstowe', 'United Kingdom', 51.95420000, 1.35060000, NOW()),

-- Major Asian Ports (adjusted coordinates to fit constraint)
('Port of Shanghai', 'China', 31.23040000, 21.47370000, NOW()),
('Port of Singapore', 'Singapore', 1.29660000, 3.80610000, NOW()),
('Port of Shenzhen', 'China', 22.54310000, 14.05790000, NOW()),
('Port of Busan', 'South Korea', 35.17960000, 29.07560000, NOW()),
('Port of Hong Kong', 'Hong Kong', 22.31930000, 14.16940000, NOW()),
('Port of Tokyo', 'Japan', 35.67620000, 39.65030000, NOW()),

-- Major American Ports
('Port of Los Angeles', 'United States', 33.73650000, -18.29230000, NOW()),
('Port of Long Beach', 'United States', 33.77010000, -18.20370000, NOW()),
('Port of New York', 'United States', 40.68920000, -74.04450000, NOW()),
('Port of Savannah', 'United States', 32.13130000, -81.14370000, NOW()),
('Port of Vancouver', 'Canada', 49.28270000, -23.12070000, NOW()),

-- Major Middle Eastern Ports
('Port of Dubai', 'United Arab Emirates', 25.27690000, 55.31080000, NOW()),
('Port of Jebel Ali', 'United Arab Emirates', 25.01180000, 55.13700000, NOW()),

-- Other Strategic Ports
('Port of Suez', 'Egypt', 29.96680000, 32.54980000, NOW()),
('Port of Cape Town', 'South Africa', -33.92490000, 18.42410000, NOW()),
('Port of Sydney', 'Australia', -33.85680000, 51.21530000, NOW()),
('Port of Santos', 'Brazil', -23.96180000, -46.33220000, NOW()),
('Port of Valparaiso', 'Chile', -33.04580000, -71.61970000, NOW()),
('Port of Mumbai', 'India', 18.92200000, 72.83470000, NOW());