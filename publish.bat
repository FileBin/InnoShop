@echo off

cd "InnoShop.Presentation"
dotnet publish /t:PublishContainer -c "Release"

cd "../InnoShop.UserManagerAPI"

dotnet publish /t:PublishContainer -c "Release"

cd "../InnoShop.ProductManagerAPI"
dotnet publish /t:PublishContainer -c "Release"

echo "PUBLISH DONE"
