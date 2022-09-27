#!/bin/bash
host="localhost:27017"                                                                                                             
username=$MONGO_INITDB_ROOT_USERNAME
password=$MONGO_INITDB_ROOT_PASSWORD
db="Northwind"
collections=( "Categories" "Customers" "Employees" "Orders" "Products" "Shippers" "Suppliers" )

echo "Begin To Import"

if [ "$MONGO_INITDB_ROOT_USERNAME" ] && [ "$MONGO_INITDB_ROOT_PASSWORD" ]; then

	for c in ${collections[@]}
	do
		echo "importing $c .."
		mongoimport --host $host --username $username --password $password --authenticationDatabase admin --db $db --collection $c --file /docker-entrypoint-initdb.d/$c.json
	done

else

	for c in ${collections[@]}
	do
		echo "importing $c .."
		echo "mongoimport --host $host --username $username --password $password --authenticationDatabase admin --db $db --collection $c --file $c.json"
		mongoimport --host $host --db $db --collection $c --file /docker-entrypoint-initdb.d/$c.json
	done

fi

echo "Done."
