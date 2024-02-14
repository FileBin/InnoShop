#!/usr/bin/env bash

set -e

# change directory to script location
cd "${0%/*}"

cd "InnoShop.Presentation"
dotnet publish /t:PublishContainer -c Release

cd "../InnoShop.UserManagerAPI"
dotnet publish /t:PublishContainer -c Release

cd "../InnoShop.ProductManagerAPI"
dotnet publish /t:PublishContainer -c Release

echo "PUBLISH DONE"