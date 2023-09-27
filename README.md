# PLCnext Technology - eUAClientConfigurator

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Web](https://img.shields.io/badge/PLCnext-Website-blue.svg)](https://www.phoenixcontact.com/plcnext)
[![Community](https://img.shields.io/badge/PLCnext-Community-blue.svg)](https://www.plcnext-community.net)

This procedure describes the basic steps for creating an OPC UA method.

In this example, the OPC UA method will be used to call a PLCnext Engineer function block instance from an OPC UA client.

The procedure uses a custom OPC UA "Information Model", which in this case is generated using the UaModeler tool from Unified Automation. This technique can also be applied when using [standard OPC UA information models](https://opcfoundation.org/developer-tools/specifications-opc-ua-information-models) for various industries and applications.

## I. Project details

|Description   | Value      |
|--------------|------------|
|Created       | 05.09.2023 |
|Last modified | 05.09.2023 |
|Controller    | AXC F 1152; AXC F 2152; AXC F 3152 |
|Firmware      | 2023.0 LTS |

## II. Background reading

- [Unified Architecture](https://opcfoundation.org/about/opc-technologies/opc-ua/)
- [OPC UA information model](https://www.plcnext.help/te/Service_Components/OPC_UA_Server/OPCUA_information_models.htm)

## III. Prerequisites for this example

- Two PLCnext Control devices with firmware 2023.0.0 or later, and at least one Axioline I/O module.
- PLCnext Engineer version 2023.0.0 or later.
- Microsoft Visual Studio 2022

## 1. Quickstart
You need the following Hardware and Software Configuration:

- AXC F 2152/3152 with IP: 192.168.1.10 and Firmware >= 2023
- AXC F 2152/3152 with IP: 192.168.1.11 and Firmware >= 2023
- PC with PLCnext Engineer Version >= 2023.6

Use the Sample Projects in the Folder .\Simple Client Sample. In this Sample there is a AXC F 2152 (192.168.1.10) serving as OPC UA Server and a AXC F 3152 (192.168.1.11) serving as Client. Adjust the Sample Project to your controller hardware if needed. Connect to your controllers and load the sample projects into them.

Now open the eUA Client Configurator, switch to the 'Servers' tab and configure your controllers as follows:

Name: AXC F 2152  
URL: opc.tcp://192.168.1.10:4840  
User Name: <admin>  
Password: <controller passwort for admin>  
Security Mode: SignAndEncrypt  
Security Policy: Aes256_Sha256_RsaPss  

Name: AXC F 3152  
URL: opc.tcp://192.168.1.11:4840  
User Name: <admin>  
Password: <controller passwort for admin>  
Security Mode: SignAndEncrypt  
Security Policy: Aes256_Sha256_RsaPss  

The configuration should now look like in the following figure:

![Configuration of the simple Client Sample](README/quickstart_00.png)

Now switch to the 'Groups' tab to configure the variable groups. In the drop down list 'PLCnext with eUA Client' select the AXC F 3152 (or the controller which serves as client in your configuration). Add two groups, one with group type 'subscribe from server' and the other one with group type 'write to server'. With a click on the browse button now select the following variables:

|Group     | Local Variable (Client)  | Remote Variable (Server) |
|----------|--------------------------|--------------------------|
|Subscribe | iSubscribeVar            | iSubscribedVar           |
|Write     | iWriteVar                | iWrittenVar              |

The configuration should now look like in the following figure:

![Variable configuration of the simple Client Sample](README/quickstart_01.png)

Now use the export button to export the configuration in a .xml file. 

Use a tool like WinSCP to connect to the file system of the AXC F 3152 (or the controller which serves as client in your configuration). Browse to the following folder: /opt/plcnext/projects/Default/Services/OpcUA/Modules/Client/Configs/ and copy the exported .xml file to here. 

Now restart the controller through power Off/On. After startup your client configuration should work.

## 2. Description of the client configuration file

In this section the client configuration file, which can be created and exported with the eUA Client Configurator is described. The following figure shows the file exported from the simple client sample configuration mentioned in section 1 - Quickstart.

![Config File of the simple client sample](README/config file_V00.PNG)

The file is seperated into the two nodes <ServerConnections> and <VariableGroups>. In the node <ServerConnections> you can find the informations from the 'Servers' tab of the eUA Client Configurator. In the node <VariableGroups> you can find the informations from the 'Groups' tab of the eUA Client Configurator. 

## 3. UI Description

In this section the user interface of the eUA Client Configurator is described in detail, but not the engineering workflow. Therefere go to section 1 - Quickstart.

### 3.1. Connections

First, at least two endpoints must be created that can communicate with each other.

![Overview of existing servers](README/eUAClient Configurator UI_00.png)

1.) Already added servers can be tested via the test button. The application then tries to establish a connection to the server.

2.) With the add server button more servers can be added.

![Add server](README/eUAClient Configurator UI_01.png)

1.) Name of the server.

2.) URL of the server in the format 'opc.tcp://192.168.1.10:4840'

3.) User name for the server authentication.

4.) Password for the server authentication.

5.) Security mode.

6.) Security policy.

7.) Cancel and discard the server configuration or apply the server configuration and add the configured server to the server list.

### 3.2. Client configuration

Now a variable group with variables can be added. For this purpose, variables of a controller that acts as a client are linked with variables of one or more servers.

![Add variable group](README/eUAClient Configurator UI_02.png)

1.) Name of the variable group configuration.

2.) With a click on the new button a new variable group configuration will be created. The present variable group configuration will be discarded.

3.) With a click on the load button a present variable group configuration can be loaded into the application.

4.) With a click on the save button the variable group configuration can be saved in a file in JSON format.

5.) With a click on the export button the variable group configuration can be exported in the OPC client readable XML format.

6.) The controller which serves as OPC client must be selected here.

7.) With a click on the add group button a new variable group can be added to the configuration.

![Add variables](README/eUAClient Configurator UI_03.png)

1.) Type of the variable group can be 'subscribe from server' or 'write to server'.

2.) Cycle time.

3.) Local variables at client end.

4.) Remote variables which are linked to the corresponding local variables 

5.) With a click on the add variable button a new local/remote pair of variables is added to the group.

![Variable mapping](README/eUAClient Configurator UI_06.png)

With a click on the browse button the selection windows for local (Client) and remote variables (Server) opens.

![Local variable selection](README/eUAClient Configurator UI_04.png)  

Select window for local variables on client end.

![Remote variable selection](README/eUAClient Configurator UI_05.png)  

Select window for remote variables. The server can be selected via the selection above.

## 4. Configuration on the file system of the controller

This section describes where in the file system of the controller OPC UA client configurations can be found and where the OPC UA client configuration generated with the eUA Client Configurator have to be stored. 

The file system of the PLCnext Controller can be entered using a tool like WinSCP. The following figure shows how to establish a connection to the controller.

![Using WinSCP](README/WinSCP_V00.PNG){width=40%}  

1.) IP-Address of the PLCnext controller.

2.) Username of the PLCnext controller.

3.) Password of the PLCnext controller.

The generated client configuration has to be stored to /opt/plcnext/projects/Default/Services/OpcUA/Modules/Client/Configs/ see the following figure.

![Client configuration on the file system of the controller](README/PLCnext Config_02.PNG)

However there are some more paths which are also relevant for OPC UA client configuration, but not necessary in combination with the eUA Client Configurator.

In the following path a configuration file for the general client settings can be stored. If there is no file here and there is also no configuration in the PLCnext Engineer project on the controller, then the default settings are used:

/opt/plcnext/projects/Default/Services/OpcUA/Modules/Client/

This path contains the configuration file for the general client settings from the PLCnext Engineer project:

/opt/plcnext/projects/PCWE/Services/OpcUA/Modules/Client/

This path contains the configuration file for the client connections configuration from the PLCnext Engineer project:

/opt/plcnext/projects/PCWE/Services/OpcUA/Modules/Client/Configs/

## IV. Problems?

- [Check the Output.log file](https://pxc1.esc-eu-central-1.empolisservices.com/service-express/portal/project1_p/document/iu-45-85e4a3ef-5699-4c4f-b7b9-4a04246e53d3?context=%7B%7D) on the PLC for messages from the OPC UA server.
- Ask for help in the [PLCnext Community Forum](https://www.plcnext-community.net/en/discussions-2-offcanvas/forums.html).

If you find a mistake in this procedure, or if you would like to suggest improvements or new features, please [open an issue](https://github.com/PLCnext/OpcUaMethods/issues).

## V. License

Copyright (c) Phoenix Contact GmbH & Co KG. All rights reserved.

Licensed under the [MIT](/LICENSE) License.
