# InnoShop

## Demo project of two web api interaction with angular front-end

The solution contains projects:
* InnoShop.Application - contains whole application logic such as commands, DTOs, queries, utils etc.
* InnoShop.Domain - contains abstract services, abstract data models and busyness logic such as entities for database
* InnoShop.Presentation - contains Angular Project with frontend part of project
* InnoShop.UserManagerAPI - REST API authorization server
* InnoShop.ProductManagerAPI - REST API product management server
(requires auth token from UserManagerAPI)

## 1. Requrements

* `dotnet` CLI

  to build backend

* `docker`

  to build this project you need docker installed

* `npm`

  to build frontend

  > make sure that all is installed properly and it is in $PATH environment variable
  
## 2. Setup

This project needs some security keys to be set up so I wrote simple bash script to generate all necessary data for project

> NOTE: If you using windows

To run setup script just execute 

On linux in bash

```shell
git clone https://github.com/FileBin/InnoShop
cd InnoShop

./setup.sh

./publish.sh
```


On windows use powershell or if doesn't work use git bash and execute **linux** commands

```ps1
git clone https://github.com/FileBin/InnoShop
cd InnoShop

.\setup.ps1

.\publish.bat
```

And input all necessary data (or use \[default\] option if available)

## 3. How to run:
* ### From command line
  To run application
  ```shell
  dotnet run ./InnoShop.Presentation/InnoShop.Presentation.csproj
  ```
  
  Or for debugging purposes
  ```shell
  dotnet watch ./InnoShop.Presentation/InnoShop.Presentation.csproj
  ```

* ### Using VSCode (for devs)
  In command pallette choose `Tasks: Run Task` and choose `Start All`