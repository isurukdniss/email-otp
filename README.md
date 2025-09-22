# Email OTP 

A clean architecture based .Net 8 backend solution for generating and verifying email based OTP with domain validation and security features. A simple Angular based frontend solution is also setup to test the backend APIs.

The Backend solution is located at `Backend/EmailOTP` folder.
The Frontend solution is located at `Frontend/email-otp-app` folder.

## Backend Application

The backend application follow the clean architecture principles with clear separation of concerns.

### Layers
- **API**: Controllers and API configurations
- **Application**: Command handlers (Mediator pattern with CQRS), application logic and application interfaces
- **Domain**: Core entities
- **Infrastructure**: Data persistance (EF Core with SQLite database), External services (Email sending) and cross-cutting concerns
- **Tests**: Unit tests with mocking.

### Assumptions
- Email sending is an external dependency. An `IEmailSender` interface will be defined in the Application layer and the concrete implementations will be defined in the Infrastructure layer. To simulate the email sending, I added a console based service called `ConsoleEmailSender`.
- As the HTTP is stateless, we cannot use `iostream` input as stated in the requirement. The serverside logic will be responsible for checking the timeout and retry limit.
- The `start` and `close` methods (object construction and disposal) stated in the requirement are handled by the EF Core. Therefore these those will not be explicitly implemented.
- Simplified Clean Architecture wis used to meet the separation of concern and to make the code clean, scalable and testable.
- CQRS with Mediator pattern is used to further enhance the separation of concerns.
- Database will be SQLite for easier accessibility.
- Frontend will be a simple Angular app to demonstrate how to use the backend APIs.

### How to setup and run the application locally

#### 1. Clone the Repository
```bash
git clone <repository-url>
cd Backend/EmailOTP
```

#### 2. Restore Dependencies
```bash
dotnet restore
```

#### 3. Update Database
```bash
dotnet ef database update --project Infrastructure --startup-project API
```

#### 4. Run the Application
```bash
dotnet run --project API
```

#### 5. Run Tests
```bash
dotnet test
```

## Frontend Application

The frontend application is a simple Angular app to demonstrate the usage of the APIs.

### How to run the frontend locally?

#### 1. Open solution folder
```bash
cd Frontend/email-otp-app
```

#### 2. Restore Dependencies
```bash
npm install
```

#### 3. Run
```bash
ng serve
```


