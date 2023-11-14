## API summary

GET /1/ping?workload=Workload
 - 200 returns "pong"
 - 422 returns "workload" is required

GET /1/images?name=Name&size=Size
 - 200 queued processing fake file with NullProcessor

POST /1/images
 - 200 queued processing uploaded jpg or png file with ImageProcessor
 - 422 returns "file" is required

Swagger:
- https://app.swaggerhub.com/apis/S4YSERBIA/AZ-204/1.0.0#/default

## Dotnet Core Web App

Repo:
 - https://github.com/akv-demo/az-appservice-dotne

URL:
 - https://az-appservice-dotnet.azurewebsites.net


github pipeline (this repo):
 - https://github.com/akv-demo/az-appservice-dotnet/blob/main/.github/workflows/main_az-appservice-dotnet.yml

Development app configuration: local appsettings.json (not present in repo)

Demo app configuraion: Azure Portal Web App Configuration

## Go Web App (docker)

Only `/1/ping` endpoint is implemented.

repo:
 - https://github.com/akv-demo/az-appservice-go

URL (DockerHub)
 - https://az-appservice-go-dockerhub.azurewebsites.net

URL (Azure Container Registry)
 - https://az-appservice-go-azureregistry.azurewebsites.net


Github pipeline (deploys dockers to DockerHub and Azure Container Registry) 
 - https://github.com/akv-demo/az-appservice-go/blob/main/.github/workflows/ci.yml
