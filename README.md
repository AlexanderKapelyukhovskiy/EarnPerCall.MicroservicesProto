# EarnPerCall.Microservices.Proto

Prototype of Microservices which uses Actor Service for unpause chat session

# Problem and solution description

## 1) describe initial problem:
Currently we initiate pause of that chat session for recharge on client side. 
We are doing it according to timer we start when customer join chat channel.
We also start timer on advisor side and when available for chat time would finish we show Session Paused for Recharge modal window.

It is possible that timers on customer and advisor sides would be mistimed by dev tool debugger or passing App to background mode.
As result users can be in incorrect state and it could lead to bad user experience.

## 2) how to resolve this problem
To resolve such issue we need to control when to pause chat session on server side.
To do that we need run timer for each session once chat was started.

We can't implement such logic in ASP.NET Web Api Application like EarnPerCall.ExternalServices because Application pool could be recycled at any time and all background tasks will be cleared.

## 3) solution 1 (windows service)

We could introduce windows service and implement logic for running timers for each chat session by ourselves.
However this approach contains few downsides:
- we introduce new dependency to our system and need adjust our deployment process to support windows service
- we need monitor status of windows service
- when we run several instances of EPC services we need decide how to coordinate requests to windows services

## 4) solution 2 (Azure Service Fabric)
As we already use Azure for wordpress website hosting we probably could rely on Azure for hosting new services.

Azure Service Fabric is a distributed systems platform that makes it easy to package, deploy, and manage scalable and reliable microservices and containers. 

Service Fabric powers many Microsoft services today, including Azure SQL Database, Azure Cosmos DB, Cortana, Microsoft Power BI, Microsoft Intune, Azure Event Hubs, Azure IoT Hub, Dynamics 365, Skype for Business, and many core Azure services.

## 5) ErnPercall.Microservices Prototype Demo

With latest version of Visual Studio we have new Project Template called "Service Fabric Application" (A project template for creating an always-on, scalable, distributed application with Microsoft Azure Service Fabric). To run Service Fabric Application we need Service Fabric SDK to be installed on machine.

Service Fabric Application project adds mostly configuration for packaging, deploying and scalability of services we would add into Service Fabric Application.

We can add into Service Fabric Application following projects: Stateless Service, Statefull Service and Actor Service.

(Reliable Actors is a Service Fabric application framework based on the Virtual Actor pattern. The Reliable Actors API provides a single-threaded programming model built on the scalability and reliability guarantees provided by Service Fabric. 
An actor is an isolated, independent unit of compute and state with single-threaded execution.)

For our case Actor service looks like nice option. For each instance of Actor class we can setup Reminder and call code after some period of time.

To make Actor service accessible for our existing Solution we need to add Stateless Web Api project. Api will be able communicate with instances of Actor services with ActorProxy class which uses .NET Remoting under the hood.
