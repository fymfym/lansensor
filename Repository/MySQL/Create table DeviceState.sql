CREATE TABLE IF NOT EXISTS `devicestate` (
  `DeviceGroipId` varchar(50) COLLATE latin1_general_ci DEFAULT NULL,
  `DeviceId` varchar(50) COLLATE latin1_general_ci DEFAULT NULL,
  `LastKnownDataValue` varchar(50) COLLATE latin1_general_ci DEFAULT NULL,
  `LastKnownDataValueDate` datetime DEFAULT NULL,
  `LastKnownKeepAlive` datetime COLLATE latin1_general_ci DEFAULT NULL,
  `LastExecutedKeepaliveCheckDate` datetime DEFAULT NULL,
  `LastKeepAliveAlert` datetime DEFAULT NULL
) 
