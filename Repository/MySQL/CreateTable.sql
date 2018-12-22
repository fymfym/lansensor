CREATE TABLE `sensorstate` (
  `id` double NOT NULL AUTO_INCREMENT,
  `DateTime` datetime DEFAULT NULL,
  `deviceId` varchar(45) DEFAULT NULL,
  `datatype` varchar(45) DEFAULT NULL,
  `datavalue` varchar(45) DEFAULT NULL,
  `deviceGroupId` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) AUTO_INCREMENT=360 DEFAULT ;
