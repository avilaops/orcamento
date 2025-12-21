@echo off
echo ========================================
echo Limpando cache do Visual Studio...
echo ========================================

cd /d "%~dp0"

echo.
echo [1/5] Deletando pasta .vs...
if exist ".vs" (
    rmdir /s /q ".vs"
    echo OK - Pasta .vs deletada
) else (
    echo OK - Pasta .vs nao existe
)

echo.
echo [2/5] Deletando bin e obj do Roncav_Budget.winui...
cd Roncav_Budget.winui
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo OK - bin e obj deletados
cd ..

echo.
echo [3/5] Deletando bin e obj do Roncav_Budget...
cd Roncav_Budget
if exist "bin" rmdir /s /q "bin"
if exist "obj" rmdir /s /q "obj"
echo OK - bin e obj deletados
cd ..

echo.
echo [4/5] Restaurando pacotes NuGet...
dotnet restore Roncav_Budget.sln
echo OK - Pacotes restaurados

echo.
echo [5/5] Buildando projeto WinUI (Debug x64)...
dotnet build "Roncav_Budget.winui\Roncav_Budget.winui.csproj" -c Debug /p:Platform=x64
echo OK - Build concluido

echo.
echo ========================================
echo CONCLUIDO!
echo ========================================
echo.
echo Agora:
echo 1. Abra o Visual Studio
echo 2. Abra a solucao Roncav_Budget.sln
echo 3. Na barra de ferramentas, selecione: Debug ^| x64
echo 4. Clique com botao direito em Roncav_Budget.winui
echo 5. Selecione "Set as Startup Project"
echo 6. Pressione F5 para executar
echo.
pause
