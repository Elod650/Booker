# Booker

# TODO

- Tests
  - Unit tests
  - Integration tests
- SignalR
- Clean architecture
- Authentication and authorization
- Background services (for email sending)
- Caching 
- Row versioning for concurrency control
- Service edit functionality

# Projects

## Booker.Backend

This project is API for the application. 
It contains the implementation of the API controllers, services, and other business logic related to the application's functionality.

## Booker.Clients

### Booker.ApiCaller

This project is responsible for making HTTP requests to the backend API.
Its main purpose is to provide a clean and reusable interface for the rest of the application to interact with the backend API, abstracting away the details of making HTTP requests and handling responses. Usable for all .NET applications, not just Blazor.

### Blazor

#### Booker.Clients.Blazor.Server

This project is a Blazor Server application that serves as the frontend for the Booker application.

## Booker.Models

This project contains the data models used throughout the application.

## Booker.Repository

This project is responsible for the access and management of the database.
It contains the implementation of the repository pattern, which abstracts the data layer and provides a clean interface for the rest of the application to interact with the database.

# Models

- **Entities**: Used in the database and repository layer. They represent the structure of the data as it is stored in the database.
- **DTOs**: Used for transferring data between different layers in the backend.
- **Requests**: Used for transfering incoming data from the client to the backend. Has data annotation validation.
- **Responses**: Used for transfering outgoing data from the backend to the client.
- **ViewModels**: Used in the Blazor project for representing the data that is displayed in the UI. They may contain additional properties or methods that are specific to the UI layer and are not part of the core business logic.