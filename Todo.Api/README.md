# TodoApi (backend)

## Requirements
- .NET 10 SDK

```
dotnet restore
dotnet build
```
- applicationUrl is set in [launchSettings.json](/Properties/launchSettings.json)
- CORS is enabled for http://localhost:4200 in development.


## TODO
- write unit tests
- may be some domain exception like ForbiddenOperation, ItemDoesNotExist
- Exception Middleware
- Check If cookie is working properly (Add More logs)
- cross user mark done?
