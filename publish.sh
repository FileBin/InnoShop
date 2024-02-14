#!/usr/bin/env bash

# change directory to script location
BASE_DIR="${0%/*}"

if [ ! -z "$BASE_DIR" ]; then

cd "$BASE_DIR/InnoShop.Presentation"
dotnet publish /t:PublishContainer -c Release

cd "$BASE_DIR/InnoShop.UserManagerAPI"
dotnet publish /t:PublishContainer -c Release

cd "$BASE_DIR/InnoShop.ProductManagerAPI"
dotnet publish /t:PublishContainer -c Release

else

echo 'Script failed to execute because $BASE_DIR was empty'

fi