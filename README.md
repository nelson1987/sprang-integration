# sprang-integration

## UserCase
* Abrir Conta
#### Será persistido, enviado para a área de análise, recebido pela área de cadastro e alterado o cadastro com status.


dotnet new gitignore
dotnet new sln -n Sprang

mkdir src
mkdir tests

dotnet new webapi -n Sprang.Api -o src/Sprang.Api
dotnet new classlib -n Sprang.Application -o src/Sprang.Application
dotnet new classlib -n Sprang.Infrastructure -o src/Sprang.Infrastructure
dotnet new xunit -n Sprang.Tests -o tests/Sprang.Tests

dotnet sln Sprang.sln add src/Sprang.Api/Sprang.Api.csproj -s Source/WebApi
dotnet sln Sprang.sln add src/Sprang.Application/Sprang.Application.csproj -s Source/Core
dotnet sln Sprang.sln add src/Sprang.Infrastructure/Sprang.Infrastructure.csproj -s Source/Infrastructure
dotnet sln Sprang.sln add tests/Sprang.Tests/Sprang.Tests.csproj -s Tests

dotnet add tests/Sprang.Tests/Sprang.Tests.csproj reference src/Sprang.Api/Sprang.Api.csproj
dotnet add tests/Sprang.Tests/Sprang.Tests.csproj reference src/Sprang.Application/Sprang.Application.csproj

dotnet add src/Sprang.Api/Sprang.Api.csproj reference src/Sprang.Application/Sprang.Application.csproj

dotnet test

git add .
git commit -m "Estrutura de Repository & Producer"
git push

#### https://github.com/aspnetboilerplate/aspnetboilerplate
#### https://github.com/referbruv/ContainerNinja.CleanArchitecture
#### https://github.com/iammukeshm/AspNetCoreHero-Boilerplate
#### https://github.com/lkurzyniec/netcore-boilerplate?tab=readme-ov-file
#### https://github.com/rramname/KTCDocker