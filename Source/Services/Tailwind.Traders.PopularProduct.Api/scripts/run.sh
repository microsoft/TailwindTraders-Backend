cat create-db.sql | envsubst > _createdb.sql
cat create-tables.sql | envsubst > _create-tables.sql

echo "Creating db"
/opt/mssql-tools/bin/sqlcmd -S $dbserver -U $dbuser -P $dbpassword  -i _createdb.sql

echo "Creating schema"
/opt/mssql-tools/bin/sqlcmd -S $dbserver -U $dbuser -P $dbpassword  -d $dbcatalog -i _create-tables.sql