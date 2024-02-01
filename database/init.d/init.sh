#!/bin/bash
set -e

source /docker-entrypoint-initdb.d/innoshop_user.env

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
	CREATE USER $DB_USER WITH PASSWORD '$DB_USER_PASSWORD';

	CREATE DATABASE InnoShop_Users;
	ALTER DATABASE InnoShop_Users OWNER TO $DB_USER;
	GRANT ALL PRIVILEGES ON DATABASE InnoShop_Users TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;

    CREATE DATABASE InnoShop_Products;
	ALTER DATABASE InnoShop_Users OWNER TO $DB_USER;
    GRANT ALL PRIVILEGES ON DATABASE InnoShop_Products TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;
EOSQL