DROP DATABASE IF EXISTS citybike_test;
CREATE DATABASE citybike_test;
CREATE TABLE citybike_test.gps_data (
  id INT UNSIGNED NOT NULL AUTO_INCREMENT UNIQUE PRIMARY KEY,
  bikeId SMALLINT UNSIGNED NOT NULL,
  latitude DECIMAL(10,8) NOT NULL,
  longitude DECIMAL(11,8) NOT NULL,
  accuracy TINYINT UNSIGNED NOT NULL,
  queried TIMESTAMP NOT NULL DEFAULT NOW()
);

-- INSERT INTO citybike_test.gps_data (bikeId, latitude, longitude, accuracy)
--   VALUES (65535, 12.12345678, -12.12345678, 255);