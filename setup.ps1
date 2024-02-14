# Define a function to generate a random string
function gen_random_string {
    $randomString = [Convert]::ToBase64String((Get-Random -Count $args[0] -InputObject (0..255)))
    return $randomString
}

# Change directory to script location
Set-Location $PSScriptRoot

# Create a directory if it does not exist
if (-not (Test-Path '.private')) {
    New-Item -ItemType Directory -Name '.private'
}

$SECRETS_ENV='.private\secrets.env'
$DATABASE_FILE = '.private\database.env'
$CACHE_FILE = '.private\cache.sh'

# Get variables from cache file
if (Test-Path $CACHE_FILE) {
  Get-Content $CACHE_FILE | foreach {
    $name, $value = $_.split('=')
    if ([string]::IsNullOrWhiteSpace($name) || $name.Contains('#')) {
      continue
    }
    Set-Content env:\$name $value
  }
}

# Initialize variables if not present in cache
if (-not $env:SECRETS_ID) {
    $env:SECRETS_ID = [guid]::NewGuid().ToString()
    Add-Content -Path $CACHE_FILE -Value "SECRETS_ID=`"$env:SECRETS_ID`""
}

if (-not $env:ADMIN_PASSWORD) {
    $env:ADMIN_PASSWORD = Read-Host "Enter admin user password [autogenerate]:"
    if (-not $env:ADMIN_PASSWORD) {
        $env:ADMIN_PASSWORD = gen_random_string 12
        Write-Output "admin user password is: $env:ADMIN_PASSWORD"
    }
    Add-Content -Path $CACHE_FILE -Value "ADMIN_PASSWORD=`"$env:ADMIN_PASSWORD`""
}

if (-not $env:SECURITY_KEY) {
    $env:SECURITY_KEY = gen_random_string 256
    Add-Content -Path $CACHE_FILE -Value "SECURITY_KEY=`"$env:SECURITY_KEY`""
}

if (-not $env:DB_ROOT_PASSWORD) {
    $env:DB_ROOT_PASSWORD = Read-Host "Enter database root password [autogenerate]:"
    if (-not $env:DB_ROOT_PASSWORD) {
        $env:DB_ROOT_PASSWORD = gen_random_string 18
        Write-Output "DB_ROOT_PASSWORD password is: $env:DB_ROOT_PASSWORD"
    }
    Add-Content -Path $CACHE_FILE -Value "DB_ROOT_PASSWORD=`"$env:DB_ROOT_PASSWORD`""
}

if (-not $env:DB_USER) {
    $env:DB_USER = Read-Host "Enter database user name [innoshop]:"
    if (-not $env:DB_USER) {
        $env:DB_USER = "innoshop"
    }
    Add-Content -Path $CACHE_FILE -Value "DB_USER=`"$env:DB_USER`""
}

if (-not $env:DB_PASSWORD) {
    $env:DB_PASSWORD = Read-Host "Enter database password [autogenerate]:"
    if (-not $env:DB_PASSWORD) {
        $env:DB_PASSWORD = gen_random_string 18
    }
    Add-Content -Path $CACHE_FILE -Value "DB_PASSWORD=`"$env:DB_PASSWORD`""
}

if (-not $env:SMTP_USER) {
    $env:SMTP_USER = Read-Host "Enter SMTP user (or email):"
    Add-Content -Path $CACHE_FILE -Value "SMTP_USER=`"$env:SMTP_USER`""
}

if (-not $env:SMTP_PASSWORD) {
    $env:SMTP_PASSWORD = Read-Host "Enter SMTP user password:"
    Add-Content -Path $CACHE_FILE -Value "SMTP_PASSWORD=`"$env:SMTP_PASSWORD`""
}

# Initialize same secrets for both projects
dotnet user-secrets init --project InnoShop.UserManagerAPI --id $env:SECRETS_ID
dotnet user-secrets init --project InnoShop.ProductManagerAPI --id $env:SECRETS_ID

# Set secrets content
@"
{
  "Database:User": $env:DB_USER,
  "Database:Password": $env:DB_PASSWORD,
  "JwtSecurityKey": $env:SECURITY_KEY,
  "SMTP:User": $env:SMTP_USER,
  "SMTP:Password": $env:SMTP_PASSWORD,
  "AdminDefaultPassword": $env:ADMIN_PASSWORD
}
"@ | dotnet user-secrets set --id $env:SECRETS_ID

@"
Database__User="$env:DB_USER"
Database__Password="$env:DB_PASSWORD"
JwtSecurityKey="$env:SECURITY_KEY"
SMTP__User="$env:SMTP_USER"
SMTP__Password="$env:SMTP_PASSWORD"
AdminDefaultPassword="$env:ADMIN_PASSWORD"
"@ | Set-Content -Path $SECRETS_ENV

# Write passwords into database.env file
@"
POSTGRES_PASSWORD=$env:DB_ROOT_PASSWORD
"@ | Set-Content -Path $DATABASE_FILE

# Write passwords into innoshop_user.env file
@"
DB_USER=$env:DB_USER
DB_USER_PASSWORD=$env:DB_PASSWORD
"@ | Set-Content -Path ".\database\init.d\innoshop_user.env"