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


## Project Structure
Currently the project is split into 3 layers: web, application and infrastructure.  
The infrastructure layer contains the communication logic and the application layer contains the business logic.  
The web layer is only necessary for the standalone version of the application and is not used in the production environment. 

Furhermore, the project includes its own simple Mqtt broker. This is to limit the outisde dependencies and to make the project more portable.

![ProjectStructure](/assets/TestcellHandshakeProject.png)

## Dependencies 
The project has a dependency to Kepware.  
A Keppware server is required to run the project, the software must be downloaded and installed. 
Then the .opf [file](/ProjectDependencies/TestCellHandshakeKepSetup.opf) must be imported into the server, to replicate the setup. 


## How to run 

1) Install and setup Kepware as described in the [Dependencies](#Dependencies) section. 
2) Run the dotnet application.
3) Use the http-file or the swagger API to trigger the handshake commands. 

