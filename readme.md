# Testcell Handshake 

This repo contains a sandbox implementation of the communication and handshake procedure with the Testcell.   
While the testcell initializes the the communication, the bulk of the logic is in the Linecontroller domain.  
Most importantly the Linecontroller (LC) is responsible for the following tasks:

 - querying ME for device information
 - setting device properties


## Communication 
Table for data exchange between Testcell and Linecontroller

|Order | Testcell     | Linecontroller    | Value |
|----|--------------|-----------|-------|
|1| ScannedData  | ->        | partNo + serialNo |
|2| ReqNewData   | ->        | true |
|3| <-           | DeviceID  | partNo + serialNo (query result from ME)|
|4| <-           | DeviceType| deviceType enum (map from query result from ME)|
|5| <-           | DeviceDest| deviceDest enum (map from query result from ME)|
|6| <-           | NewDataRec| true |
|7| ReqNewData   | ->        | false |
|8| <-           | NewDataRec| false |
