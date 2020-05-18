if "%~1" == "" (
    goto IncorrectNumberOfArguments
)

cd src/BackendApi
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../BackendWeb
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../JobLogger
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../TextRankCalc
start /wait dotnet publish --configuration Release
if %ERRORLEVEL% NEQ 0 (
    goto BuildError
)

cd ../..


mkdir "%~1"\BackendApi
mkdir "%~1"\BackendWeb
mkdir "%~1"\JobLogger
mkdir "%~1"\TextRankCalc
mkdir "%~1"\config

xcopy src\BackendApi\bin\Release\netcoreapp3.1\publish "%~1"\BackendApi\ /s /e
xcopy src\BackendWeb\bin\Release\netcoreapp3.1\publish "%~1"\BackendWeb\ /s /e
xcopy src\JobLogger\bin\Release\netcoreapp3.1\publish "%~1"\JobLogger\ /s /e
xcopy src\TextRankCalc\bin\Release\netcoreapp3.1\publish "%~1"\TextRankCalc\ /s /e
xcopy src\config "%~1"\config /s /e
xcopy start.cmd "%~1"
xcopy stop.cmd "%~1"

echo BUILD SUCCESS
exit /b 0

:IncorrectNumberOfArguments
    echo Incorrect number of arguments.
	echo Example: build.cmd <SemVer>
    exit /b 1

:BuildError
    echo Failed to build project.
    exit /b 1	