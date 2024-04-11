# sprang-integration

dotnet new gitignore
dotnet new sln -n Sprang

mkdir src
mkdir tests

dotnet new webapi -n Sprang.Api -o src/Sprang.Api
dotnet new classlib -n Sprang.Core -o src/Sprang.Core
dotnet new xunit -n Sprang.Tests -o tests/Sprang.Tests

dotnet sln Sprang.sln add src/Sprang.Api/Sprang.Api.csproj -s src
dotnet sln Sprang.sln add src/Sprang.Core/Sprang.Core.csproj -s src
dotnet sln Sprang.sln add tests/Sprang.Tests/Sprang.Tests.csproj -s tests

dotnet add tests/Sprang.Tests/Sprang.Tests.csproj reference src/Sprang.Api/Sprang.Api.csproj
dotnet add tests/Sprang.Tests/Sprang.Tests.csproj reference src/Sprang.Core/Sprang.Core.csproj

dotnet add src/Sprang.Api/Sprang.Api.csproj reference src/Sprang.Core/Sprang.Core.csproj

dotnet test

git add .
git commit -m "Estrutura de Repository & Producer"
git push

#### https://github.com/aspnetboilerplate/aspnetboilerplate
#### https://github.com/referbruv/ContainerNinja.CleanArchitecture
#### https://github.com/iammukeshm/AspNetCoreHero-Boilerplate
#### https://github.com/lkurzyniec/netcore-boilerplate?tab=readme-ov-file
