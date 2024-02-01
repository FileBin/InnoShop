#!/bin/bash
set -e

source /docker-entrypoint-initdb.d/innoshop_user.env

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
	CREATE USER $DB_USER WITH PASSWORD '$DB_USER_PASSWORD';

	CREATE DATABASE innoShop_users;
	ALTER DATABASE innoShop_users OWNER TO $DB_USER;
	GRANT ALL PRIVILEGES ON DATABASE innoShop_users TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;

    CREATE DATABASE innoShop_products;
	ALTER DATABASE innoShop_products OWNER TO $DB_USER;
    GRANT ALL PRIVILEGES ON DATABASE innoShop_products TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;
EOSQL
