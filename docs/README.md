# Booker

# TODO
- Logging
- Error handling
  - Global error handling middleware
  - Custom error responses
  - ToastR
- Tests
  - Unit tests
  - Integration tests
- Http files

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