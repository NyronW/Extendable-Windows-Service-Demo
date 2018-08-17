# Extendable-Windows-Service-Demo

This sample solution shows how to create a windows service that is easily extendable to support quite a few work loads, such as hosting Web API and SignalR endpoints, serving static web contents, running schedule jobs using [Quartz.net], and directory monitoring and file parsing.

Windows services are widely used to perfrom many backend processing in many software projects, from simply tasks such as sending notification to more complex tasks such as processing purchase order in a microservices based online ordering application. A lot of the times we may need to perform several tasks that are well suited for windows services and would require us creating a service for each task. This solution illustrates how to use one wndows service to host multiple work loads and remove the need to create a windows service per work load.

## Solution Overview
The demo is a collaborative todo application, which allow two or more users to manage a todo list simultaneously. Any update that is made by one user is immediately reflected on the other users browser . There is one console application (host service) which will be deployed as a windows service (using [TopShelf]), that acts as the shell for all the feature modules in the solution. [Structuremap] is used to create a plugin system that enables the host service to load all the feature modules and service libraries created from the other projects in the solution. Each class that will be loaded as a plugin must implement the [IServicePlugin] interface, which is used by StructureMap to identify all plugins to be registered by its container.

The projects in the solution can be grouped as follows:

  - **Core**: This project contains all contracts (interface & abstract class)  and related data models that is used and/or implemented by other projects.
  - **Module**: These projects represent the work loads that the windows service will support, such as hosting web api, task scheduling and file parsing.
  - **Services**: These projects contain classes that provide contract implementation for each work load, and also several cross cutting concerns services such as email notification and logging.
  - **Host**: This is the actual windows service that acts as a host for all the work load modules

### Modules

  - **Web**: This project host an asp.net web appplication which exposes two web api 2 endpoints, a signalr hub and static content serving middleware. The web app can be access at localhost:9099; this url can be change by modifyng the service host [app.config] file.
  - **Importer**: This project monitors a specific folder (which is set in the app.config file) for newly created text files containg a todo list. The file is parsed and its content are sent to a web api endpoint (in the web module) via http post.
  - **Scheduler**: This module uses Quartz.net to schedule and run a custom job that clears all file from the import directory (used by the importer module)

### Project List

The solution consists of the following projects:

* [WillCorp.Core] - This project contains all the contract definitions that are used throughout the solution. All the other projects adds a reference to this project.
* [WillCorp.HostService] - This is a console app that's used as a shell for the feature modules, [TopShelf] is used to enable the console app to run as a windows service. 
* [WillCorp.App.Importer] - This feature module provides file import and parsing capabilities, it uses the FileWatcher class to monitor a specific folder for text files to be parsed and sent to the web module.
* [WillCorp.App.Scheduler] - The purpose of this feature module is to execute scheduled tasks. It contains one job that clears all files from the import folder; The job uses a [cron expression] stored in the service host [app.config] file to determine when to run each job.
* [WillCorp.App.Web] - This is a web hosting module, which exposes two Web Api 2 endpoints, static files (such as html, css and images) and a SignalR hub. The [todo web api controller] accepts post requests, save the posted item into an in memory store, and then calls a method on the SignalR hub; The hub will push the item recieved from the api controller down to the html page which can loaded in the browser at http://localhost:9099.  
* [WillCorp.Services.Configuration] - This service project is used to access configuration entries from the app.config file; it allows a default to be specified if the configure element is missing from the file.
* [WillCorp.Services.Logging] - This service project uses [Serilog] to implement the [ILogger] interface that is used to perform application wide logging.
* [WillCorp.Services.Scheduling] - this service uses Quartz.net to implement the [IScheduledJob] and [ItriggerFactory] interfaces that are used to execute scheduled tasks, such as clearing the import folder
* [WillCorp.Services.Smtp] - This project provide email sending functionality and is a simple wrapper around the SmtpClient class found in the dot net framework.


[//]: # (These are reference links used in the body of this note and get stripped out when the markdown processor does its job. There is no need to format nicely because it shouldn't be seen. Thanks SO - http://stackoverflow.com/questions/4823468/store-comments-in-markdown-syntax)

   [WillCorp.Core]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.Core>
   [WillCorp.HostService]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.HostService>
   [WillCorp.App.Importer]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.App.Importer>
   [WillCorp.App.Web]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.App.Web>
   [WillCorp.App.Scheduler]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.App.Scheduler>
   [WillCorp.Services.Configuration]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.Services.Configuration>
   [WillCorp.Services.Logging]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.Services.Logging>
   [WillCorp.Services.Scheduling]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.Services.Scheduling>
   [WillCorp.Services.Smtp]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/tree/master/WillCorp.Services.Smtp>
   [TopShelf]: <http://topshelf-project.com/>
   
 [StructureMap]: <http://structuremap.github.io/>  
 
 [IServicePlugin]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.Core/IServicePlugin.cs>  
 
 [cron expression]: <https://docs.oracle.com/cd/E12058_01/doc/doc.1014/e12030/cron_expressions.htm>
 
  [app.config]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.HostService/App.config>
 
 [ILogger]: <https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.Core/Logging/ILogger.cs>
   
   [Serilog]: <https://serilog.net/>
   
   [IScheduledJob]:<https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.Core/Scheduling/IScheduledJob.cs>
   
   [ItriggerFactory]:<https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.Core/Scheduling/ITriggerFactory.cs>
   
   [Quartz.net]:<https://www.quartz-scheduler.net/>
   
   [todo web api controller]:<https://github.com/NyronW/Extendable-Windows-Service-Demo/blob/master/WillCorp.App.Web/Api/Controllers/TodosController.cs>
