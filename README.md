# UserManagement

This project was intended for a interview I was planning to have with a company.

> [!WARNING]
> Projects is still W.I.P.

## Requirements

- CRUD for users and groups
- Assign users to groups
- Search and pagination
- Clean error handling
- Documentation

## Architecture

The app is built on the .net stack and contains an aspire apphost, a backend api and the frontend web app.

The aspire is for linking and ease of development.

The backend api is desinged in a monolithic style. Reason was mostly to keep things simple and development time low.

Frontend was developed ontop of a blazor wasm standalone template. Mostly because I am not overly exeperienced with other frontend languages/frameworks.

## Running

To Run the application just type the

```bash
dotnet run
```

command inside the apphost directory. This should start up the application.
