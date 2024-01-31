# InnoShop

## Demo project of web api interaction with angular front-end

The solution contains projects:
* InnoShop.Presentation - contains Angular Project with frontend part of project

## 1. Setup

This project needs some security keys to be set up so I wrote simple bash script to generate all nececcary data for project

NOTE: If you using windows use GitBash, WSL, or MinGW to execute this script

To run setup script just execute 

```shell
./setup.sh
```

And input all nececcary data or skip by pressing ENTER


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