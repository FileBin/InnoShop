#!/bin/bash
set -e

source /docker-entrypoint-initdb.d/innoshop_user.env

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
	CREATE USER $DB_USER WITH PASSWORD '$DB_USER_PASSWORD';

	CREATE DATABASE innoshop_users;
	ALTER DATABASE innoshop_users OWNER TO $DB_USER;
	GRANT ALL PRIVILEGES ON DATABASE innoshop_users TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;

    CREATE DATABASE innoshop_products;
	ALTER DATABASE innoshop_products OWNER TO $DB_USER;
    GRANT ALL PRIVILEGES ON DATABASE innoshop_products TO $DB_USER;
	GRANT USAGE, CREATE ON SCHEMA public TO $DB_USER;
EOSQL
