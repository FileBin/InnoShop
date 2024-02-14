@echo off

SET BASE_DIR="%~dp0"

cd "%BASE_DIR%/InnoShop.Presentation"
dotnet publish /t:PublishContainer -c "Release"

cd "%BASE_DIR%/InnoShop.UserManagerAPI"

dotnet publish /t:PublishContainer -c "Release"

cd "%BASE_DIR%/InnoShop.ProductManagerAPI"
dotnet publish /t:PublishContainer -c "Release"

echo "PUBLISH DONE"
