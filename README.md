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

## Authorisation
A role based authorisation scheme was created. 
There are three main roles: 
- Guest
- Subscriber
- Editor

Guests have the same permissions as anonymous users, 
while subscribers do not get ads when reading articles and are able to create articles, edit their own articles and write comments. 
Editors are more privileged and have the permissions to edit and delete all users articles and comments. These could be considered the sites moderators. 


