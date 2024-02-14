#!/usr/bin/env bash

gen_random_string() {
    cat /dev/urandom | base64 -w 0 | head -c$1
}

# change directory to script location
cd "${0%/*}"

if [ ! -d '.private' ]; then
    mkdir .private
fi

SECRETS_ENV='.private/secrets.env'
DATABASE_FILE='.private/database.env'
CACHE_FILE='.private/cache.sh'

# get variables from cache file
if [ -f "$CACHE_FILE" ]; then
    source "$CACHE_FILE"
fi

# init variables if are not present in cache
if [ -z "$SECRETS_ID" ]; then
    SECRETS_ID="$(uuidgen)"
    echo "SECRETS_ID=\"$SECRETS_ID\"" >> "$CACHE_FILE"
fi

if [ -z "$ADMIN_PASSWORD" ]; then
    echo "Enter admin user password [autogenerate]:"
    read ADMIN_PASSWORD
        if [ -z "$ADMIN_PASSWORD" ]; then
        ADMIN_PASSWORD="$(gen_random_string 12)"
        echo "admin user password is: $ADMIN_PASSWORD"
    fi
    echo "ADMIN_PASSWORD=\"$ADMIN_PASSWORD\"" >> "$CACHE_FILE"
fi

if [ -z "$SECURITY_KEY" ]; then
    SECURITY_KEY="$(gen_random_string 256)"
    echo "SECURITY_KEY=\"$SECURITY_KEY\"" >> "$CACHE_FILE"
fi

if [ -z "$DB_ROOT_PASSWORD" ]; then
    echo "Enter database root password [autogenerate]:"
    read DB_ROOT_PASSWORD
    if [ -z "$DB_ROOT_PASSWORD" ]; then
        DB_ROOT_PASSWORD="$(gen_random_string 18)"
        echo "DB_ROOT_PASSWORD is: $DB_ROOT_PASSWORD"
    fi
    echo "DB_ROOT_PASSWORD=\"$DB_ROOT_PASSWORD\"" >> "$CACHE_FILE"
fi

if [ -z "$DB_USER" ]; then
    echo "Enter database user name [innoshop]:"
    read DB_USER
    if [ -z "$DB_USER" ]; then
        DB_USER="innoshop"
    fi
    echo "DB_USER=\"$DB_USER\"" >> "$CACHE_FILE"
fi

if [ -z "$DB_PASSWORD" ]; then
    echo "Enter database password [autogenerate]:"
    read DB_PASSWORD
    if [ -z "$DB_PASSWORD" ]; then
        DB_PASSWORD="$(gen_random_string 18)"
    fi
    echo "DB_PASSWORD=\"$DB_PASSWORD\"" >> "$CACHE_FILE"
fi

if [ -z "$SMTP_USER" ]; then
    echo "Enter SMTP user (or email):"
    read SMTP_USER
    echo "SMTP_USER=\"$SMTP_USER\"" >> "$CACHE_FILE"
fi

if [ -z "$SMTP_PASSWORD" ]; then
    echo "Enter SMTP user password:"
    read SMTP_PASSWORD
    echo "SMTP_PASSWORD=\"$SMTP_PASSWORD\"" >> "$CACHE_FILE"
fi

# init same secrets for both projects
dotnet user-secrets init --project InnoShop.UserManagerAPI --id "$SECRETS_ID"
dotnet user-secrets init --project InnoShop.ProductManagerAPI --id "$SECRETS_ID"

# set secrets content
cat <<EOF | dotnet user-secrets set --id "$SECRETS_ID"
{
  "Database:User": "$DB_USER",
  "Database:Password": "$DB_PASSWORD",
  "JwtSecurityKey": "$SECURITY_KEY",
  "SMTP:User": "$SMTP_USER",
  "SMTP:Password": "$SMTP_PASSWORD",
  "AdminDefaultPassword": "$ADMIN_PASSWORD"
}
EOF

cat <<EOF | tee "$SECRETS_ENV"
Database__User="$DB_USER"
Database__Password="$DB_PASSWORD"
JwtSecurityKey="$SECURITY_KEY"
SMTP__User="$SMTP_USER"
SMTP__Password="$SMTP_PASSWORD"
AdminDefaultPassword="$ADMIN_PASSWORD"
EOF

# write same passwords into database.env file
cat <<EOF | tee "$DATABASE_FILE"
POSTGRES_PASSWORD="$DB_ROOT_PASSWORD"
EOF


cat <<EOF | tee "./database/init.d/innoshop_user.env"
DB_USER="$DB_USER"
DB_USER_PASSWORD="$DB_PASSWORD"
EOF