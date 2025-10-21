USE apidepapas;
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE ProductQty;
TRUNCATE TABLE ShippingLog;
TRUNCATE TABLE Shippings;
TRUNCATE TABLE Travels;
TRUNCATE TABLE DistributionCenters;
TRUNCATE TABLE TransportMethods;
TRUNCATE TABLE Addresses;
TRUNCATE TABLE Localities;
TRUNCATE TABLE __EFMigrationsHistory;

-- Carga usando la ruta del contenedor de la API
LOAD DATA LOCAL INFILE '/app/csvs/_locality.csv' INTO TABLE Localities FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (postal_code, locality_name, state_name, country, lat, lon);
LOAD DATA LOCAL INFILE '/app/csvs/_transport_method_truck.csv' INTO TABLE TransportMethods FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (transport_id, transport_type, average_speed, available, max_capacity);
LOAD DATA LOCAL INFILE '/app/csvs/_transport_method_boat.csv' INTO TABLE TransportMethods FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (transport_id, transport_type, average_speed, available, max_capacity);
LOAD DATA LOCAL INFILE '/app/csvs/_transport_method_plane.csv' INTO TABLE TransportMethods FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (transport_id, transport_type, average_speed, available, max_capacity);
LOAD DATA LOCAL INFILE '/app/csvs/_addresses.csv' INTO TABLE Addresses FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (address_id, street, number, postal_code, locality_name);
LOAD DATA LOCAL INFILE '/app/csvs/_distribution_centers.csv' INTO TABLE DistributionCenters FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (distribution_center_id, address_id);
LOAD DATA LOCAL INFILE '/app/csvs/_travels.csv' INTO TABLE Travels FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (travel_id, departure_time, arrival_time, transport_method_id, distribution_center_id);
LOAD DATA LOCAL INFILE '/app/csvs/_shipping.csv' INTO TABLE Shippings FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (shipping_id,order_id,user_id,delivery_address_id,status,travel_id,tracking_number,carrier_name,total_cost,currency,estimated_delivery_at,created_at,updated_at); 
LOAD DATA LOCAL INFILE '/app/csvs/_productqty.csv' INTO TABLE ProductQty FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS (id, ShippingDetailshipping_id, quantity); 

SET FOREIGN_KEY_CHECKS = 1;