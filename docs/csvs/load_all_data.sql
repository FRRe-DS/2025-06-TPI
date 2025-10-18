-- 
-- SCRIPT DE CARGA MASIVA PARA PROYECTO DE LOGÍSTICA
-- 
-- REQUISITOS:
-- 1. Estar posicionado en el directorio 'docs/csvs'.
-- 2. Conectarse a MySQL con el flag --local-infile.
-- 

USE apidepapas;

-- Deshabilita la verificación de FKs para permitir la carga en orden inverso
SET FOREIGN_KEY_CHECKS = 0; 

-- =========================================================
-- A. LIMPIEZA TOTAL (Evita errores 1701 y duplicados)
-- =========================================================

-- Tablas dependientes (Hijos)
TRUNCATE TABLE ProductQty;
TRUNCATE TABLE ShippingLog;
TRUNCATE TABLE Shippings;
TRUNCATE TABLE Travels;
TRUNCATE TABLE DistributionCenters;

-- Tablas base (Padres)
TRUNCATE TABLE TransportMethods;
TRUNCATE TABLE Addresses;
TRUNCATE TABLE Localities;
TRUNCATE TABLE __EFMigrationsHistory;

-- =========================================================
-- 1. CARGA DE LOCALITIES (TABLA BASE)
-- =========================================================

LOAD DATA LOCAL INFILE '_locality.csv' 
INTO TABLE Localities
FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(postal_code, locality_name, state_name, country, lat, lon); -- Coincide con DESCRIBE Localities

-- =========================================================
-- 2. CARGA DE TRANSPORT METHODS (CATÁLOGO)
-- =========================================================

-- TRUCK
LOAD DATA LOCAL INFILE '_transport_method_truck.csv'
INTO TABLE TransportMethods
FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(transport_id, transport_type, average_speed, available, max_capacity); -- Coincide con DESCRIBE TransportMethods

-- BOAT
LOAD DATA LOCAL INFILE '_transport_method_boat.csv'
INTO TABLE TransportMethods
FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(transport_id, transport_type, average_speed, available, max_capacity);

-- PLANE
LOAD DATA LOCAL INFILE '_transport_method_plane.csv'
INTO TABLE TransportMethods
FIELDS TERMINATED BY ';' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(transport_id, transport_type, average_speed, available, max_capacity);

-- =========================================================
-- 3. CARGA DE ADDRESSES (Depende de: Localities)
-- =========================================================

LOAD DATA LOCAL INFILE '_addresses.csv'
INTO TABLE Addresses
FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(address_id, street, number, postal_code, locality_name); -- Coincide con DESCRIBE Addresses

-- =========================================================
-- 4. CARGA DE DISTRIBUTION CENTERS (Depende de: Addresses)
-- =========================================================

LOAD DATA LOCAL INFILE '_distribution_centers.csv'
INTO TABLE DistributionCenters
FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(distribution_center_id, address_id); -- Coincide con DESCRIBE DistributionCenters

-- =========================================================
-- 5. CARGA DE TRAVELS (Depende de: DistributionCenters, TransportMethods)
-- =========================================================

LOAD DATA LOCAL INFILE '_travels.csv'
INTO TABLE Travels
FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(travel_id, departure_time, arrival_time, transport_method_id, distribution_center_id); -- Coincide con DESCRIBE Travels

-- =========================================================
-- 6. CARGA DE SHIPPINGS (Depende de: Addresses, Travels)
-- =========================================================

LOAD DATA LOCAL INFILE '_shipping.csv'
INTO TABLE Shippings
FIELDS TERMINATED BY ',' ENCLOSED BY '"' LINES TERMINATED BY '\n' IGNORE 1 ROWS
(shipping_id,order_id,user_id,delivery_address_id,status,travel_id,tracking_number,carrier_name,total_cost,currency,estimated_delivery_at,created_at,updated_at); 


-- =========================================================
-- 7. CARGA DE PRODUCT QUANTITY (TABLA HIJA DE SHIPPINGS)
-- =========================================================

LOAD DATA LOCAL INFILE '_productqty.csv'
INTO TABLE ProductQty
FIELDS TERMINATED BY ',' 
ENCLOSED BY '"'
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(id, ShippingDetailshipping_id, quantity); 

-- =========================================================
-- REINICIO DE CHEQUEOS DE INTEGRIDAD
-- =========================================================

SET FOREIGN_KEY_CHECKS = 1;