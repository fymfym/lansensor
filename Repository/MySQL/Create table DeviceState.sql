CREATE TABLE `devicestate` (
  `DeviceGroipId` varchar(50) CHARACTER SET latin1 COLLATE latin1_general_ci DEFAULT NULL,
  `DeviceId` varchar(50) CHARACTER SET latin1 COLLATE latin1_general_ci DEFAULT NULL,
  `LastKnownDataValue` varchar(50) CHARACTER SET latin1 COLLATE latin1_general_ci DEFAULT NULL,
  `LastKnownDataValueDate` datetime DEFAULT NULL,
  `LastKnownKeepAliveDate` datetime DEFAULT NULL,
  `LastExecutedKeepaliveCheckDate` datetime DEFAULT NULL,
  `LastKeepAliveAlert` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
