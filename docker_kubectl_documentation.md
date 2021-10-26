# Documentation

## Docker

#### Build the Docker Image locally
```
docker build -t <docker_hub_id>/<image_name> . --no-cache
```

#### Run the built Docker Image as an Container
```
docker run -p 8080:80 -d <docker_hub_id>/<image_name>
```

#### Show running Containers
```
docker ps
```

#### Start / Stop Docker Container
```
docker start <container_id>
docker stop <container_id>
```

#### Push Docker Image to Docker Hub Registry with default Tag (latest)
```
docker push <docker_hub_id>/<image_name>
```

### Setup local MSSQL Server
```
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<password>" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
```



## Kubernetes
#### Apply Deployment / Service etc.
```
kubectl apply -f <config_file_name>.yaml
kubectl apply -f <folder_name>
```

#### Show all Deployments / Pods / Services
```
kubectl get [deployments / pods / services]
```

### Deploying SQL Server
#### Persistent Volume Claim
Create a Claim to some storage

#### Create a Secret 
```
kubectl create secret generic mssql --from-literal=SA_PASSWORD="<password>"
```



## Entity Framework Core
### Create Migration
```
dotnet ef migrations add <migration_message>
```

### Update Datbase
```
dotnet ef database update
```