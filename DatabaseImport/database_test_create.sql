DROP DATABASE IF EXISTS citybike_test;
CREATE DATABASE citybike_test;
CREATE TABLE citybike_test.gps_data (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT UNIQUE PRIMARY KEY,
  bikeId SMALLINT UNSIGNED NOT NULL,
  latitude DECIMAL(10,8) NOT NULL,
  longitude DECIMAL(11,8) NOT NULL,
  accuracy TINYINT UNSIGNED NOT NULL,
  queried DATETIME NOT NULL
);

CREATE TABLE citybike_test.stations (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT UNIQUE PRIMARY KEY,
  station_name varchar(20) NOT NULL,
  latitude DECIMAL(10,8) NOT NULL,
  longitude DECIMAL(11,8) NOT NULL
);

-- INSERT INTO citybike_test.gps_data (bikeId, latitude, longitude, accuracy, queried)
--   VALUES (65535, 12.12345678, -12.12345678, 255, '9999-12-31 23:59:59');