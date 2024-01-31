#!/usr/bin/env bash

gen_random_string() {
    cat /dev/urandom | base64 -w 0 | head -c$1
}

# change directory to script location
cd "${0%/*}"

if [ ! -d '.private' ]; then
    mkdir .private
fi

# get variables from cache file
DATABASE_FILE='.private/database.env'
CACHE_FILE='.private/cache.sh'

if [ -f "$CACHE_FILE" ]; then
    source "$CACHE_FILE"
fi

# init variables if are not present in cache
if [ -z "$SECRETS_ID" ]; then
    SECRETS_ID="$(uuidgen)"
    echo "SECRETS_ID=\"$SECRETS_ID\"" >> "$CACHE_FILE"
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
    fi
    echo "DB_ROOT_PASSWORD=\"$DB_ROOT_PASSWORD\"" >> "$CACHE_FILE"
fi

if [ -z "$DB_USER" ]; then
    echo "Enter database user name [user]:"
    read DB_USER
    if [ -z "$DB_USER" ]; then
        DB_USER="user"
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

# init same secrets for both projects
dotnet user-secrets init --project InnoShop.UserManagerAPI --id "$SECRETS_ID"
dotnet user-secrets init --project InnoShop.ProductManagerAPI --id "$SECRETS_ID"

# set secrets content
cat <<EOF | dotnet user-secrets set --id "$SECRETS_ID"
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=$DB_USER;password=$DB_PASSWORD;"
  },
  "JwtSecurityKey": "$SECURITY_KEY"
}
EOF

# write same passwords into database.env file
cat <<EOF | tee "$DATABASE_FILE"
MYSQL_ROOT_PASSWORD=$DB_ROOT_PASSWORD
MYSQL_USER=$DB_USER
MYSQL_PASSWORD=$DB_PASSWORD
EOF