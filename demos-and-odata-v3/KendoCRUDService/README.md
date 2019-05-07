## Instructions for local setup

### Setup the database

1. Download the Northwind database from https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs
2. Add the database as `Sample.mdf` in `KendoCRUDService/App_Data/`

This is necessary for local setup only, the live environment at https://demos.telerik.com/kendo-ui/service/ does not need the database (it is generated from a `.sql` script with specific permissions).
