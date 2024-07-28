# A simple weather reporting app for a magical device

## Requirements

1. #### Retrieve weather status from 3rd party api
1. #### Store latest report in DB
1. #### make no assumptions about the input or output, do not modify the data
1. #### application will be written once and never maintained
1. #### request timeouts are 5 seconds, either fetch latest report or latest db value
1. #### application api is called alot
1. ### application will go to space and will be out of reach
1. ### Keep it Simple Stupid

## Implementation

- Used a simple 2 layer arch with the application layer and the api presentation layer
- the application layer uses HTTP client factory with poly resilience for api calls
- the application layer use EF core with a very simple setup for db calls
- the application also uses a third party package to do 'Upsert' commands with EF Core

### Note
> the application makes no assumption about the input query parameters and passes them directly to the underlying api and returns the exact json response it recieves from the api
  it also stores the exact response as a binary field to support every dependent change from the api for over 20 years while allowing the query or the response to be cahnged

The Identity of the resulting report is determined by the query parameters passed to the api
the api generates a unique key from these parameters and stores it inside the database

Since the report must return within a near 5 second limit with every query a parallel db call will be initiated
if the weather api returns in time the db call will be cancelled and ignored however due to parallelism this will be a best effort to avoid redundant db calls

If the weather api return in time and successfully, the new report that will be stored will be published in an event driven way, allowing the main thread to continue and send the respond to the client
to maintain performance, the event is sent to a background service that will upsert the value into the database concurrently

the application is also designed to have an overall 'best effort' behaviour to keep its uptime, for example it can work without a database but the telemetry needs futher config to detect these problems when it is deployed
### Features
Since the application is expected to be deployed on a space shuttle and out of reach
the app might need the change configurations over the next 20 years with no downtime
to do this I have implemented a dynamic reconfiguration service and endpoints that will update the config 
values used by the database or the api at runtime

___

Notes:
- To implement request timeouts I avoided the native ASPNETCORE "RequestTimeouts" middleware as it has its own complications in this specific senario
- Since the project will ever only be written and deployed once No unit tests were provided
- we also did not need to use 3-layer or onion-layered arch since those will be an overhead when there will be no maintainance

