# Clean Architecture - A Comprehensive Guide

## Table of Contents
1. [Introduction to Clean Architecture](#introduction-to-clean-architecture)
2. [Layers of Clean Architecture](#layers-of-clean-architecture)
3. [Detailed Folder Structure and Purpose](#detailed-folder-structure-and-purpose)
4. [Common Packages Used in Clean Architecture](#common-packages-used-in-clean-architecture)
5. [Project References - Purpose and Implementation](#project-references---purpose-and-implementation)
6. [Step-by-Step Guide to Set Up a Web API with Clean Architecture](#step-by-step-guide-to-set-up-a-web-api-with-clean-architecture)

## Introduction to Clean Architecture

### What is Clean Architecture?
Clean Architecture is a software design philosophy introduced by Robert C. Martin (Uncle Bob) that emphasizes separation of concerns through a set of concentric layers. Each layer has a specific responsibility and communicates with other layers through well-defined interfaces.

### Core Principles
1. **Independence of frameworks**: The architecture doesn't depend on the existence of some library or framework. This allows frameworks to be used as tools rather than forcing the system to adapt to their constraints.

2. **Testability**: Business rules can be tested without UI, database, web server, or any external element.

3. **Independence of UI**: The UI can change easily without changing the rest of the system.

4. **Independence of database**: Your business rules don't depend on a specific database, allowing you to swap databases without affecting the business logic.

5. **Independence of any external agency**: Business rules don't know anything about interfaces to the outside world.

### Why Use Clean Architecture?

1. **Maintainability**: With proper separation of concerns, the codebase becomes more maintainable as each layer has a distinct responsibility.

2. **Scalability**: New features can be added more easily as the system's complexity is managed through well-defined boundaries.

3. **Testability**: Business logic is isolated from external dependencies, making it easier to write unit tests.

4. **Flexibility**: Frameworks, UI, databases can be replaced with minimal impact on the core business logic.

5. **Future-proofing**: The architecture protects the business logic from technological changes, extending the lifespan of the application.

6. **Delayed decisions**: It allows teams to delay technical decisions, as the core business logic doesn't depend on specific implementations.

## Layers of Clean Architecture

Clean Architecture consists of concentric layers, each with specific responsibilities. From the innermost to the outermost layer:

### 1. Domain Layer (Enterprise Business Rules)
- **Purpose**: Contains enterprise-wide business rules and entities.
- **Characteristics**: 
  - Most stable layer that rarely changes
  - Has no dependencies on other layers or external frameworks
  - Pure business logic with no infrastructure concerns
- **Key Components**: Entities, Value Objects, Domain Events, Domain Exceptions

### 2. Application Layer (Application Business Rules)
- **Purpose**: Contains application-specific business rules and orchestrates the flow of data to and from the domain entities.
- **Characteristics**:
  - Depends only on the Domain layer
  - Implements use cases that orchestrate the domain objects
  - Acts as a boundary between domain and outer layers
- **Key Components**: Use Cases, Commands, Queries, Application Services, DTOs, Interfaces for infrastructure services

### 3. Infrastructure Layer (Interface Adapters)
- **Purpose**: Contains adapters that convert data from the format most convenient for use cases and entities to the format most convenient for external services.
- **Characteristics**:
  - Implements interfaces defined in the Application layer
  - Contains specific technology implementations
  - Adapts external frameworks to interface with the inner layers
- **Key Components**: Repositories, ORM Configurations, API Clients, File System, Email Services

### 4. Presentation Layer (Frameworks & Drivers)
- **Purpose**: Contains frameworks and tools like the UI, web, database, devices, etc.
- **Characteristics**:
  - Most volatile layer that changes frequently with technology updates
  - Depends on all inner layers but no inner layer depends on it
  - Entry point for user interactions
- **Key Components**: Controllers, Views, ViewModels, API Endpoints, WebSockets

### The Dependency Rule
In Clean Architecture, dependencies must point inward. Inner layers should not know anything about outer layers. This is achieved through the Dependency Inversion Principle, where outer layers depend on abstractions defined in inner layers.

## Detailed Folder Structure and Purpose

Let's break down the typical folder structure within each layer and explain their purposes:

### Domain Layer Project Structure

```
Domain/
├── Entities/
├── ValueObjects/
├── Enums/
├── Exceptions/
├── Events/
├── Constants/
├── Specifications/
├── Interfaces/
```

- **Entities/**: Contains enterprise-wide business entities representing core business concepts.
  - Files: User.cs, Product.cs, Order.cs
  - Purpose: Define the core business objects with behavior and properties

- **ValueObjects/**: Contains immutable objects that represent descriptive aspects of the domain.
  - Files: Address.cs, Money.cs, PersonName.cs
  - Purpose: Encapsulate attributes that don't have an identity of their own

- **Enums/**: Contains enumeration classes that represent a fixed set of values.
  - Files: OrderStatus.cs, UserRole.cs
  - Purpose: Define type-safe constants for domain concepts

- **Exceptions/**: Contains domain-specific exceptions.
  - Files: DomainException.cs, InvalidOrderStateException.cs
  - Purpose: Define custom exceptions for domain-specific error scenarios

- **Events/**: Contains domain events triggered when specific changes occur.
  - Files: OrderPlacedEvent.cs, UserCreatedEvent.cs
  - Purpose: Enable loose coupling between domain objects

- **Constants/**: Contains domain-specific constants.
  - Files: SystemConstants.cs, BusinessRuleConstants.cs
  - Purpose: Define constants used across the domain

- **Specifications/**: Contains business rule specifications.
  - Files: ActiveUserSpecification.cs, CompletedOrderSpecification.cs
  - Purpose: Encapsulate business rules in reusable components

- **Interfaces/**: Contains core domain interfaces.
  - Files: IAggregateRoot.cs, IEntity.cs
  - Purpose: Define contracts for domain objects

### Application Layer Project Structure

```
Application/
├── Common/
│   ├── Behaviors/
│   ├── Exceptions/
│   ├── Interfaces/
│   ├── Models/
│   └── Mappings/
├── Features/
│   ├── Users/
│   │   ├── Commands/
│   │   └── Queries/
│   ├── Products/
│   │   ├── Commands/
│   │   └── Queries/
│   └── Orders/
│       ├── Commands/
│       └── Queries/
└── Services/
```

- **Common/**: Contains shared application components.
  - **Behaviors/**: Contains pipeline behaviors for cross-cutting concerns.
    - Files: ValidationBehavior.cs, LoggingBehavior.cs, CachingBehavior.cs
    - Purpose: Implement cross-cutting concerns like validation, logging, and caching

  - **Exceptions/**: Contains application-specific exceptions.
    - Files: NotFoundException.cs, ValidationException.cs
    - Purpose: Define exceptions specific to application logic

  - **Interfaces/**: Contains application service interfaces.
    - Files: IDateTime.cs, ICurrentUserService.cs, IEmailService.cs
    - Purpose: Define contracts for services needed by application layer

  - **Models/**: Contains DTOs and view models.
    - Files: UserDto.cs, ProductViewModel.cs
    - Purpose: Define data transfer objects for communication between layers

  - **Mappings/**: Contains mapping profiles.
    - Files: MappingProfile.cs, UserMappings.cs
    - Purpose: Define mappings between domain entities and DTOs

- **Features/**: Contains application features organized by domain concepts.
  - **Users/Commands/**: Contains user-related commands.
    - Files: CreateUserCommand.cs, UpdateUserCommand.cs, DeleteUserCommand.cs
    - Purpose: Define commands to modify user data

  - **Users/Queries/**: Contains user-related queries.
    - Files: GetUserQuery.cs, GetUsersQuery.cs, GetUserByEmailQuery.cs
    - Purpose: Define queries to retrieve user data

  - **Products/Commands/**: Contains product-related commands.
    - Files: CreateProductCommand.cs, UpdateProductCommand.cs
    - Purpose: Define commands to modify product data

  - **Products/Queries/**: Contains product-related queries.
    - Files: GetProductQuery.cs, GetProductsQuery.cs
    - Purpose: Define queries to retrieve product data

  - **Orders/Commands/**: Contains order-related commands.
    - Files: PlaceOrderCommand.cs, CancelOrderCommand.cs
    - Purpose: Define commands to modify order data

  - **Orders/Queries/**: Contains order-related queries.
    - Files: GetOrderQuery.cs, GetOrdersQuery.cs
    - Purpose: Define queries to retrieve order data

- **Services/**: Contains application services.
  - Files: UserService.cs, OrderProcessingService.cs
  - Purpose: Implement application-specific business logic

### Infrastructure Layer Project Structure

```
Infrastructure/
├── Persistence/
│   ├── Configurations/
│   ├── Repositories/
│   ├── Migrations/
│   └── Context/
├── Identity/
├── Messaging/
├── FileStorage/
├── Email/
├── Cache/
├── Logging/
├── DependencyInjection/
└── Services/
```

- **Persistence/**: Contains database-related components.
  - **Configurations/**: Contains entity configurations.
    - Files: UserConfiguration.cs, ProductConfiguration.cs
    - Purpose: Define how entities are mapped to database tables

  - **Repositories/**: Contains repository implementations.
    - Files: UserRepository.cs, ProductRepository.cs
    - Purpose: Implement data access logic defined in application interfaces

  - **Migrations/**: Contains database migrations.
    - Files: Initial.cs, AddUserTable.cs
    - Purpose: Track database schema changes

  - **Context/**: Contains database contexts.
    - Files: ApplicationDbContext.cs, ReadOnlyDbContext.cs
    - Purpose: Provide entry points to the database

- **Identity/**: Contains authentication and authorization components.
  - Files: JwtTokenService.cs, IdentityService.cs
  - Purpose: Implement identity-related services

- **Messaging/**: Contains messaging infrastructure.
  - Files: RabbitMQService.cs, KafkaProducer.cs
  - Purpose: Implement message queue integration

- **FileStorage/**: Contains file storage infrastructure.
  - Files: LocalFileStorage.cs, S3FileStorage.cs
  - Purpose: Implement file storage services

- **Email/**: Contains email infrastructure.
  - Files: SmtpEmailService.cs, SendGridEmailClient.cs
  - Purpose: Implement email sending services

- **Cache/**: Contains caching infrastructure.
  - Files: RedisCacheService.cs, MemoryCacheService.cs
  - Purpose: Implement caching services

- **Logging/**: Contains logging infrastructure.
  - Files: SerilogAdapter.cs, ApplicationLogger.cs
  - Purpose: Implement logging services

- **DependencyInjection/**: Contains DI registration modules.
  - Files: InfrastructureModule.cs, PersistenceModule.cs
  - Purpose: Register infrastructure services with the DI container

- **Services/**: Contains infrastructure service implementations.
  - Files: DateTimeService.cs, CurrentUserService.cs
  - Purpose: Implement application service interfaces

### Presentation Layer Project Structure (Web API)

```
WebApi/
├── Controllers/
├── Middleware/
├── Filters/
├── Extensions/
├── Models/
├── Startup.cs
├── Program.cs
└── appsettings.json
```

- **Controllers/**: Contains API controllers.
  - Files: UsersController.cs, ProductsController.cs, OrdersController.cs
  - Purpose: Define API endpoints and handle HTTP requests

- **Middleware/**: Contains custom middleware.
  - Files: ExceptionHandlingMiddleware.cs, RequestLoggingMiddleware.cs
  - Purpose: Implement request processing pipeline components

- **Filters/**: Contains action filters and authorization policies.
  - Files: ApiExceptionFilterAttribute.cs, ValidateModelAttribute.cs
  - Purpose: Implement cross-cutting concerns for API controllers

- **Extensions/**: Contains extension methods.
  - Files: ServiceCollectionExtensions.cs, ApplicationBuilderExtensions.cs
  - Purpose: Extend functionality of framework classes

- **Models/**: Contains request and response models.
  - Files: CreateUserRequest.cs, UserResponse.cs
  - Purpose: Define models specific to the API layer

## Common Packages Used in Clean Architecture

### Domain Layer Packages
- **None or minimal dependencies**: The domain layer should be as pure as possible, with no or minimal external dependencies.
  - Purpose: Keep the domain model clean and free from external concerns

### Application Layer Packages
- **MediatR**: Implements the mediator pattern for decoupling request/response and notification operations.
  - Purpose: Implement CQRS (Command Query Responsibility Segregation) and decouple handlers from controllers

- **FluentValidation**: Provides a fluent interface for building validation rules.
  - Purpose: Validate incoming commands and queries

- **AutoMapper**: Maps objects to other objects based on configuration.
  - Purpose: Transform entities to DTOs and vice versa

- **Newtonsoft.Json/System.Text.Json**: JSON serialization/deserialization.
  - Purpose: Handle data format conversions

### Infrastructure Layer Packages
- **EntityFrameworkCore**: ORM for database access.
  - Purpose: Implement data access using the repository pattern

- **Dapper**: Micro-ORM for performance-critical operations.
  - Purpose: Implement efficient data access for read operations

- **Identity packages**: Authentication and authorization.
  - Purpose: Implement user identity and security

- **Microsoft.Extensions.Configuration**: Configuration management.
  - Purpose: Access application settings

- **Microsoft.Extensions.DependencyInjection**: Dependency injection.
  - Purpose: Register and resolve services

- **Serilog/NLog**: Structured logging.
  - Purpose: Implement logging services

- **StackExchange.Redis**: Redis client for caching.
  - Purpose: Implement distributed caching

- **RabbitMQ.Client/MassTransit**: Message queuing.
  - Purpose: Implement asynchronous messaging

- **Polly**: Resilience and transient-fault-handling.
  - Purpose: Implement retry policies for external services

- **Quartz.NET**: Job scheduling.
  - Purpose: Implement background jobs and scheduled tasks

### Presentation Layer Packages
- **Microsoft.AspNetCore.Mvc**: MVC framework for building API endpoints.
  - Purpose: Implement REST API controllers

- **Swashbuckle.AspNetCore**: Swagger/OpenAPI documentation.
  - Purpose: Generate API documentation

- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication.
  - Purpose: Implement token-based authentication

- **Microsoft.AspNetCore.Cors**: Cross-Origin Resource Sharing.
  - Purpose: Configure CORS policies

## Project References - Purpose and Implementation

### Purpose of Project References

Project references in Clean Architecture establish the dependency direction and ensure that inner layers don't depend on outer layers. Here's why we reference projects to each other:

1. **Enforcing the Dependency Rule**: By explicitly setting up project references, we ensure dependencies flow inward, adhering to the core principle of Clean Architecture.

2. **Separation of Concerns**: Each project has a specific responsibility, and references ensure proper boundaries between these concerns.

3. **Modular Design**: Project references make it easier to understand how components relate to each other and enable better organization of code.

4. **Flexibility**: With proper references, you can swap out implementation details in outer layers without affecting inner layers.

5. **Testability**: Properly referenced projects allow for easier mocking of dependencies in unit tests.

### Implementation of Project References

Here's how project references are typically set up in a Clean Architecture solution:

1. **Domain Project**:
   - References: None
   - Purpose: The domain should not depend on any other project, ensuring it remains pure and focused on business rules.

2. **Application Project**:
   - References: Domain
   - Purpose: Application layer needs access to domain entities and business rules to orchestrate them in use cases.

3. **Infrastructure Project**:
   - References: Domain, Application
   - Purpose: Implements interfaces defined in the Application and Domain layers, providing concrete implementations for repositories, services, etc.

4. **Presentation (WebApi) Project**:
   - References: Domain, Application, Infrastructure
   - Purpose: Acts as the composition root, wiring everything together and providing entry points for users.

### Example Reference Flow

Consider a typical request flow:

1. A user makes an HTTP request to an endpoint in the WebApi project.
2. The controller in WebApi uses MediatR to send a command/query defined in the Application project.
3. The handler in the Application project uses interfaces defined in the Application layer, implemented in the Infrastructure project.
4. The implementation in Infrastructure accesses and manipulates Domain entities as needed.
5. The response flows back up through the layers to the user.

This reference structure ensures that changes in outer layers (like swapping databases or UI frameworks) don't affect inner layers.

## Step-by-Step Guide to Set Up a Web API with Clean Architecture

### Step 1: Create Solution and Project Structure

1. **Create a new blank solution**:
   ```
   dotnet new sln -n YourSolutionName
   ```

2. **Create projects for each layer**:
   ```
   dotnet new classlib -n YourSolutionName.Domain
   dotnet new classlib -n YourSolutionName.Application
   dotnet new classlib -n YourSolutionName.Infrastructure
   dotnet new webapi -n YourSolutionName.WebApi
   ```

3. **Add projects to the solution**:
   ```
   dotnet sln add YourSolutionName.Domain
   dotnet sln add YourSolutionName.Application
   dotnet sln add YourSolutionName.Infrastructure
   dotnet sln add YourSolutionName.WebApi
   ```

### Step 2: Set Up Project References

1. **Set up Application project references**:
   ```
   dotnet add YourSolutionName.Application reference YourSolutionName.Domain
   ```

2. **Set up Infrastructure project references**:
   ```
   dotnet add YourSolutionName.Infrastructure reference YourSolutionName.Domain
   dotnet add YourSolutionName.Infrastructure reference YourSolutionName.Application
   ```

3. **Set up WebApi project references**:
   ```
   dotnet add YourSolutionName.WebApi reference YourSolutionName.Domain
   dotnet add YourSolutionName.WebApi reference YourSolutionName.Application
   dotnet add YourSolutionName.WebApi reference YourSolutionName.Infrastructure
   ```

### Step 3: Install Required Packages

1. **Domain project packages** (minimal or none)

2. **Application project packages**:
   ```
   dotnet add YourSolutionName.Application package MediatR
   dotnet add YourSolutionName.Application package MediatR.Extensions.Microsoft.DependencyInjection
   dotnet add YourSolutionName.Application package FluentValidation
   dotnet add YourSolutionName.Application package FluentValidation.DependencyInjectionExtensions
   dotnet add YourSolutionName.Application package AutoMapper
   dotnet add YourSolutionName.Application package AutoMapper.Extensions.Microsoft.DependencyInjection
   dotnet add YourSolutionName.Application package Microsoft.Extensions.DependencyInjection.Abstractions
   ```

3. **Infrastructure project packages**:
   ```
   dotnet add YourSolutionName.Infrastructure package Microsoft.EntityFrameworkCore
   dotnet add YourSolutionName.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add YourSolutionName.Infrastructure package Microsoft.EntityFrameworkCore.Tools
   dotnet add YourSolutionName.Infrastructure package Microsoft.Extensions.Configuration
   dotnet add YourSolutionName.Infrastructure package Microsoft.Extensions.Configuration.Binder
   dotnet add YourSolutionName.Infrastructure package Microsoft.Extensions.DependencyInjection
   dotnet add YourSolutionName.Infrastructure package Serilog
   ```

4. **WebApi project packages**:
   ```
   dotnet add YourSolutionName.WebApi package Swashbuckle.AspNetCore
   dotnet add YourSolutionName.WebApi package Microsoft.EntityFrameworkCore.Design
   dotnet add YourSolutionName.WebApi package Serilog.AspNetCore
   dotnet add YourSolutionName.WebApi package Serilog.Sinks.Console
   ```

### Step 4: Set Up Domain Layer

1. **Create the basic folder structure**:
   - Entities
   - ValueObjects
   - Enums
   - Exceptions
   - Events
   - Interfaces

2. **Define your first entity**:
   ```csharp
   // Domain/Entities/User.cs
   namespace YourSolutionName.Domain.Entities
   {
       public class User
       {
           public int Id { get; set; }
           public string Name { get; set; }
           public string Email { get; set; }
           // Add other properties and methods
       }
   }
   ```

3. **Define repository interfaces**:
   ```csharp
   // Domain/Interfaces/IUserRepository.cs
   namespace YourSolutionName.Domain.Interfaces
   {
       public interface IUserRepository
       {
           Task<User> GetByIdAsync(int id);
           Task<IEnumerable<User>> GetAllAsync();
           Task<User> AddAsync(User user);
           Task UpdateAsync(User user);
           Task DeleteAsync(User user);
       }
   }
   ```

### Step 5: Set Up Application Layer

1. **Create the basic folder structure**:
   - Common
     - Behaviors
     - Exceptions
     - Interfaces
     - Models
     - Mappings
   - Features
     - Users
       - Commands
       - Queries
   - Services

2. **Set up MediatR and AutoMapper**:
   ```csharp
   // Application/DependencyInjection.cs
   namespace YourSolutionName.Application
   {
       public static class DependencyInjection
       {
           public static IServiceCollection AddApplication(this IServiceCollection services)
           {
               services.AddAutoMapper(Assembly.GetExecutingAssembly());
               services.AddMediatR(Assembly.GetExecutingAssembly());
               services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
               
               services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
               
               return services;
           }
       }
   }
   ```

3. **Create your first query**:
   ```csharp
   // Application/Features/Users/Queries/GetUserQuery.cs
   namespace YourSolutionName.Application.Features.Users.Queries
   {
       public class GetUserQuery : IRequest<UserDto>
       {
           public int Id { get; set; }
       }
       
       public class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
       {
           private readonly IUserRepository _userRepository;
           private readonly IMapper _mapper;
           
           public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
           {
               _userRepository = userRepository;
               _mapper = mapper;
           }
           
           public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
           {
               var user = await _userRepository.GetByIdAsync(request.Id);
               if (user == null)
                   throw new NotFoundException(nameof(User), request.Id);
                   
               return _mapper.Map<UserDto>(user);
           }
       }
   }
   ```

4. **Create your first command**:
   ```csharp
   // Application/Features/Users/Commands/CreateUserCommand.cs
   namespace YourSolutionName.Application.Features.Users.Commands
   {
       public class CreateUserCommand : IRequest<int>
       {
           public string Name { get; set; }
           public string Email { get; set; }
       }
       
       public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
       {
           private readonly IUserRepository _userRepository;
           
           public CreateUserCommandHandler(IUserRepository userRepository)
           {
               _userRepository = userRepository;
           }
           
           public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
           {
               var user = new User
               {
                   Name = request.Name,
                   Email = request.Email
               };
               
               var createdUser = await _userRepository.AddAsync(user);
               return createdUser.Id;
           }
       }
   }
   ```

### Step 6: Set Up Infrastructure Layer

1. **Create the basic folder structure**:
   - Persistence
     - Configurations
     - Repositories
     - Context
   - Identity
   - DependencyInjection
   - Services

2. **Implement DbContext**:
   ```csharp
   // Infrastructure/Persistence/Context/ApplicationDbContext.cs
   namespace YourSolutionName.Infrastructure.Persistence.Context
   {
       public class ApplicationDbContext : DbContext
       {
           public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
               : base(options)
           {
           }
           
           public DbSet<User> Users { get; set; }
           
           protected override void OnModelCreating(ModelBuilder modelBuilder)
           {
               modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
           }
       }
   }
   ```

3. **Implement Repository**:
   ```csharp
   // Infrastructure/Persistence/Repositories/UserRepository.cs
   namespace YourSolutionName.Infrastructure.Persistence.Repositories
   {
       public class UserRepository : IUserRepository
       {
           private readonly ApplicationDbContext _context;
           
           public UserRepository(ApplicationDbContext context)
           {
               _context = context;
           }
           
           public async Task<User> GetByIdAsync(int id)
           {
               return await _context.Users.FindAsync(id);
           }
           
           public async Task<IEnumerable<User>> GetAllAsync()
           {
               return await _context.Users.ToListAsync();
           }
           
           public async Task<User> AddAsync(User user)
           {
               _context.Users.Add(user);
               await _context.SaveChangesAsync();
               return user;
           }
           
           public async Task UpdateAsync(User user)
           {
               _context.Entry(user).State = EntityState.Modified;
               await _context.SaveChangesAsync();
           }
           
           public async Task DeleteAsync(User user)
           {
               _context.Users.Remove(user);
               await _context.SaveChangesAsync();
           }
       }
   }
   ```

4. **Register services for dependency injection**:
   ```csharp
   // Infrastructure/DependencyInjection.cs
   namespace YourSolutionName.Infrastructure
   {
       public static class DependencyInjection
       {
           public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
           {
               services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("DefaultConnection"),
                       b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
               
               services.AddScoped<IUserRepository, UserRepository>();
               
               return services;
           }
       }
   }
   ```

### Step 7: Set Up Presentation Layer (Web API)

1. **Update Program.cs**:
   ```csharp
   // WebApi/Program.cs
   var builder = WebApplication.CreateBuilder(args);

   // Add services to the container.
   builder.Services.AddApplication();
   builder.Services.AddInfrastructure(builder.Configuration);

   builder.Services.AddControllers();
   builder.Services.AddEndpointsApiExplorer();
   builder.Services.AddSwaggerGen();

   var app = builder.Build();

   // Configure the HTTP request pipeline.
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }

   app.UseHttpsRedirection();
   app.UseAuthorization();
   app.MapControllers();

   app.Run();
   ```

2. **Create Controllers**:
   ```csharp
   // WebApi/Controllers/UsersController.cs
   namespace YourSolutionName.WebApi.Controllers
   {
       [ApiController]
       [Route("api/[controller]")]
       public class UsersController : ControllerBase
       {
           private readonly IMediator _mediator;
           
           public UsersController(IMediator mediator)
           {
               _mediator = mediator;
           }
           
           [HttpGet("{id}")]
           public async Task<ActionResult<UserDto>> Get(int id)
           {
               var query = new GetUserQuery { Id = id };
               var result = await _mediator.Send(query);
               return Ok(result);
           }
           
           [HttpPost]
           public async Task<ActionResult<int>> Create(CreateUserCommand command)
           {
               var result = await _mediator.Send(command);
               return Ok(result);
           }
       }
   }
   ```

3. **Configure appsettings.json**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDatabase;Trusted_Connection=True;MultipleActiveResultSets=true"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

### Step 8: Run Database Migrations

1. **Create initial migration**:
   ```
   cd YourSolutionName.WebApi
   dotnet ef migrations add InitialCreate --project ../YourSolutionName.Infrastructure
   ```

2. **Apply migration to database**:
   ```
   dotnet ef database update
   ```

### Step 9: Test Your API

1. **Run the application**:
   ```
   dotnet run --project YourSolutionName.WebApi
   ```

2. **Access Swagger UI** at https://localhost:5001/swagger to test your endpoints.

## Conclusion

Clean Architecture provides a robust, maintainable, and testable structure for your applications. By separating concerns and dependencies, it allows your business logic to evolve independently from external concerns like frameworks, UI, and databases.

When implemented correctly, Clean Architecture results in a system that is:

1. **Independent of frameworks**: Your business rules don't know about the existence of the UI, database, or external services.

2. **Testable**: Your business rules can be tested without the UI, database, web server, or any external element.

3. **Independent of UI**: The UI can change easily without changing the rest of the system.

4. **Independent of database**: Your business rules don't depend on a specific database implementation.

5. **Independent of any external agency**: Business rules don't know anything about interfaces to the outside world.

By following this guide, you'll be able to set up a Clean Architecture solution that adheres to these principles, resulting in a more maintainable and scalable application.
