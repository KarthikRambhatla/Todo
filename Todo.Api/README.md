# TodoApi (backend)

## Requirements
- .NET 10 SDK

```
dotnet restore
dotnet build
```
- applicationUrl is set in [launchSettings.json](/Properties/launchSettings.json)
- CORS is enabled for http://localhost:4200 in development.


## TODO API

This API exposes end points 

GET
/api/Todo

POST
/api/Todo
`{"Title":"Todo Task Title"}`

GET
/api/Todo/{id}

DELETE
/api/Todo/{id}

PATCH
/api/Todo/{id}/done 
