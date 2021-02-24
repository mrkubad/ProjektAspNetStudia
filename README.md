# Very Simple Messagner

### Used Technologies:
- [Bootstrap 4.3.1](https://getbootstrap.com/)
- JavaScript
- [ASP.NET Core MVC](https://docs.microsoft.com/pl-pl/aspnet/core/mvc/overview?view=aspnetcore-5.0)
- [SignalR](https://dotnet.microsoft.com/apps/aspnet/signalr)
- [Entity Framework Core 5](https://docs.microsoft.com/pl-pl/ef/core/)

# Short Description
This is my project to pass the course. I didn't have an idea what to make, because firstly I expected to work in group, but my teammates decided to go for different method and here I am with my `Very Simple Messagner`! :D


# Features
- Very fast messaging thanks to SignalR technology
- Ability to create user accounts
- Ability to create chat rooms with all users registered users
- Multilanguage (not working yet, but I've tried)
- All necessary data is saved in Db.


# Setup
To run this project:
1. Download source code, to your local machine.
2. Open the solution file with `Visual Studio 2019`.
3. You need to apply database migration. [What is migration?](https://docs.microsoft.com/pl-pl/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli)
    - Open `Package Manager Console` in VS.
    - Use `Add-Migration <migration_name>` command to create migration.
    - Use `Update-Database` command to apply migration to your DB.
4. Run project.
5. Create your first account via Registry From.
6. Log in to created account.
7. You are good to go!