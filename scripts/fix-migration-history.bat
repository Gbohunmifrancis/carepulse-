@echo off
echo === Migration History Sync Script ===
echo This script will help sync your migration history with existing database schema
echo.

set DATA_PROJECT=C:\Users\FRANCIS GBOHUNMI\Desktop\PROJECTS\solve-station\solve-inventory-pharmacy\src\SolveStation.Data
set API_PROJECT=C:\Users\FRANCIS GBOHUNMI\Desktop\PROJECTS\solve-station\solve-inventory-pharmacy\src\SolveStation.PharmacyApi

echo Navigating to data project directory...
cd /d "%DATA_PROJECT%"

echo.
echo === OPTIONS ===
echo Choose an option:
echo 1. Check migration status
echo 2. Force sync migration history (if tables exist but migration history is missing)
echo 3. Drop and recreate database (CAUTION: This will delete all data)
echo 4. Create a new migration from current state
echo 5. Just run normal migration
echo 6. Exit
echo.

set /p choice="Enter your choice (1-6): "

if "%choice%"=="1" goto check_status
if "%choice%"=="2" goto force_sync
if "%choice%"=="3" goto recreate_db
if "%choice%"=="4" goto new_migration
if "%choice%"=="5" goto normal_migration
if "%choice%"=="6" goto exit
goto invalid_choice

:check_status
echo.
echo Checking current migration status...
dotnet ef database update --list --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
pause
goto exit

:force_sync
echo.
echo Force syncing migration history...
echo This will mark all existing migrations as applied without running them.
set /p confirm="Are you sure? (y/N): "
if /i "%confirm%"=="y" (
    echo Running migration update...
    dotnet ef database update --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
    echo Migration history sync completed!
) else (
    echo Operation cancelled.
)
pause
goto exit

:recreate_db
echo.
echo WARNING: This will delete ALL data in your database!
set /p confirm="Type 'DELETE' to confirm: "
if "%confirm%"=="DELETE" (
    echo Dropping database...
    dotnet ef database drop --force --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
    echo Database dropped.
    
    echo Creating and migrating database...
    dotnet ef database update --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
    echo Database recreated and migrated!
) else (
    echo Operation cancelled.
)
pause
goto exit

:new_migration
echo.
set /p migration_name="Enter migration name (e.g., 'SyncExistingSchema'): "
echo Creating migration '%migration_name%'...
dotnet ef migrations add "%migration_name%" --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
echo Migration '%migration_name%' created!
echo You can now run 'dotnet ef database update' to apply it.
pause
goto exit

:normal_migration
echo.
echo Running normal migration...
dotnet ef database update --project "%DATA_PROJECT%" --startup-project "%API_PROJECT%"
echo Migration completed!
pause
goto exit

:invalid_choice
echo Invalid choice. Please try again.
pause
goto exit

:exit
echo.
echo === DONE ===
echo You can now try running your application with 'dotnet run'
pause