# InnoShop

## Demo project of two web api interaction with angular front-end

The solution contains projects:
* InnoShop.Application - contains whole application logic such as commands, DTOs, queries, utils etc.
* InnoShop.Domain - contains abstract services, abstract data models and busyness logic such as entities for database
* InnoShop.Presentation - contains Angular Project with frontend part of project
* InnoShop.UserManagerAPI - REST API authorization server
* InnoShop.ProductManagerAPI - REST API product management server
(requires auth token from UserManagerAPI)

## 1. Setup

This project needs some security keys to be set up so I wrote simple bash script to generate all necessary data for project

NOTE: If you using windows use GitBash, WSL, or MinGW to execute this script

To run setup script just execute 

```shell
./setup.sh
```

And input all necessary data (or use \[default\] option if available)

## 2. How to run:
* ### From command line

  make sure that dotnet is installed properly and it is in $PATH environment variable

  To run application
  ```shell
  dotnet run ./InnoShop.Presentation/InnoShop.Presentation.csproj
  ```
  
  Or for debugging purposes
  ```shell
  dotnet watch ./InnoShop.Presentation/InnoShop.Presentation.csproj
  ```

* ### Using VSCode

  In command pallette choose `Tasks: Run Task` and choose `Watch InnoShop.Presentation`