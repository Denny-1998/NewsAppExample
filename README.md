# NewsAppExample
## asp.net
This project was made using c# with asp.net. It is running in dotnet 8.0. 
There is no Frontend besides the built in Swagger UI for testing. 

## Data model
For the data model, I decided to use a locally hosted Microsoft SQL server database. Mainly, because it was already installed and running. 
The project communicates with the database using Entity Framework (code first approach). 
![image](https://github.com/Denny-1998/NewsAppExample/assets/89900734/1869dc04-66a3-48c6-b036-1f0257e04814)

## Authentication
For authentication a simple login with username and password was created. 
The password gets hashed a few times before being stored in the database alongside the randomly generated salt. 

After login, a JWT Bearer token gets created and returned to the user. This is used to both verify that the user is logged in and to read the users role from the database. 

## Authorisation
A role based authorisation scheme was created. 
There are three main roles: 
- Guest
- Subscriber
- Editor

Guests have the same permissions as anonymous users, 
while subscribers do not get ads when reading articles and are able to create articles, edit their own articles and write comments. 
Editors are more privileged and have the permissions to edit and delete all users articles and comments. These could be considered the sites moderators. 

## Github codespaces
The project would be able to run in Github codespaces, but it does not have the config file with the database connectionstring and there is no local MS SQL server. 

## Running the project
This project needs a Microsoft SQL server database. For that to work, a config.json file needs to be located in the root directory of the project. 
```json
{
	"connectionString": "Server=localhost;Database=NewsAppExample;User Id=<UserID>;Password=<Password>;TrustServerCertificate=true;",
	"jwtKey": "Store jwt key here"
}
```
In a production system, this data should of course be stored somewhere more secure and be encrypted.

To get the database migration started, following commands are needed:
```bash
Add-Migration <MigrationName>
Update-Database
```
