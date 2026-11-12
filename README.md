https://youtube.com/shorts/_LhVJZO3DgA?si=DVGrEMR7RHNm50cu
# RaceDay – Event Management System

## Programming 2B – Portfolio of Evidence (Part 3)

**Student:** Ndinae Nemushungwa  
**Student Number:** ST10485418  
**Project:** RaceDay  
**Technology:** ASP.NET Core Web API, ASP.NET Core MVC, C#, SQL Server  
**Framework:** .NET 8.0

---

## 1. Project Overview

RaceDay is a web-based event management system developed to help organisers manage sporting events and allow participants to enrol in available event categories.

The application supports running, walking and cycling events. It uses an ASP.NET Core Web API for backend operations, an ASP.NET Core MVC website for the user interface and Microsoft SQL Server for data storage.

The project demonstrates database integration, RESTful API development, authentication, role-based access, CRUD operations and communication between a frontend website and backend API.

## 2. Project Objectives

The main objectives of RaceDay are to:

- Provide a central platform for managing sporting events.
- Allow organisers to create, view, update and delete events.
- Support the management of event categories.
- Allow participants to register, log in and enrol in event categories.
- Allow participants to view and cancel their enrolments.
- Display recorded race results and finishing positions.
- Store application data in a relational SQL Server database.
- Apply role-based access control to protect application functionality.

## 3. Technologies Used

| Technology | Purpose |
|---|---|
| C# | Application programming language |
| ASP.NET Core Web API | Backend REST API |
| ASP.NET Core MVC | Web application frontend |
| .NET 8.0 | Application framework |
| Entity Framework Core | Database access |
| Microsoft SQL Server | Relational database |
| SQL Server Management Studio | Database administration |
| Swagger | API documentation and testing |
| JWT | API authentication |
| HTML, CSS, Bootstrap and Razor | User interface |
| Visual Studio | Development environment |

## 4. Solution Structure

```text
RaceDay Submission/
│
├── RACEDAY.API/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Program.cs
│   └── appsettings.json
│
├── RaceDay.Mvc/
│   ├── Controllers/
│   ├── Views/
│   ├── Filters/
│   ├── wwwroot/
│   ├── Program.cs
│   └── appsettings.json
│
├── README.md
│
└── RaceDay Solution File
```

The API handles business operations and database access, while the MVC project provides the website that users interact with.

## 5. Database Design

The application uses a SQL Server database named **RaceDay**.

The database contains seven main entities:

1. **Users:** Stores organiser and participant account information.
2. **Events:** Stores sporting event details.
3. **Categories:** Stores event category information.
4. **EventCategories:** Connects events to categories and stores entry fees and entry limits.
5. **Enrolments:** Records participant registrations for event categories.
6. **Results:** Stores participant race times and finishing positions.
7. **Routes:** Stores route information associated with events.

Relationships between these tables are implemented using primary and foreign keys to maintain data integrity.

## 6. Authentication and User Roles

RaceDay supports two user roles.

### Organiser

Organisers can access management functionality, including:

- Viewing the Organiser Dashboard.
- Creating sporting events.
- Editing existing events.
- Deleting events.
- Viewing and managing event categories.
- Accessing recorded race results.

### Participant

Participants can access enrolment functionality, including:

- Logging in to the application.
- Viewing available event categories.
- Enrolling in event categories.
- Viewing their enrolment records.
- Checking enrolment status.
- Cancelling an enrolment.

The backend uses JWT authentication, while the MVC application stores login information in the user session to manage access to pages.

## 7. API Functionality

The RaceDay API provides endpoints for accessing and managing application data.

| Endpoint | Method | Purpose |
|---|---|---|
| `/api/auth/register` | POST | Register an account |
| `/api/auth/login` | POST | Authenticate a user |
| `/api/Events` | GET | Retrieve events |
| `/api/Events` | POST | Create an event |
| `/api/Events/{id}` | PUT | Update an event |
| `/api/Events/{id}` | DELETE | Delete an event |
| `/api/Categories` | GET | Retrieve categories |
| `/api/Categories` | POST | Create a category |
| `/api/Categories/{id}` | PUT | Update a category |
| `/api/Categories/{id}` | DELETE | Delete a category |
| `/api/EventCategories` | GET | Retrieve event categories |
| `/api/Enrolments/mine` | GET | Retrieve participant enrolments |
| `/api/Enrolments` | POST | Enrol in an event category |
| `/api/Enrolments/{id}/cancel` | PUT | Cancel an enrolment |
| `/api/Results` | GET | Retrieve recorded results |

Some endpoints require authentication and an appropriate user role.

## 8. How to Run the Project

### Prerequisites

Before running RaceDay, install:

- Visual Studio with ASP.NET and web development support.
- .NET 8 SDK.
- Microsoft SQL Server.
- SQL Server Management Studio.

### Step 1: Open the solution

Extract the submitted ZIP folder and open the RaceDay solution file in Visual Studio.

### Step 2: Configure the database

Ensure SQL Server is running and the **RaceDay** database exists.

The development database server used for this project is:

`localhost\SQLEXPRESS`

Verify the API connection string in `RACEDAY.API/appsettings.json`.

### Step 3: Restore dependencies

In Visual Studio, restore the required NuGet packages.

### Step 4: Build the solution

Select:

**Build → Rebuild Solution**

Confirm that the API and MVC projects compile successfully.

### Step 5: Configure startup projects

Right-click the solution and select **Configure Startup Projects**.

Choose **Multiple startup projects** and set:

- `RACEDAY.API` — Start
- `RaceDay.Mvc` — Start

### Step 6: Run the application

Press **F5** to start both projects.

Open the MVC website to use RaceDay.

The API can be tested through Swagger at the HTTPS address configured in its launch settings, followed by `/swagger`.

## 9. Testing and Verification

The following functionality was tested during development:

| Test | Result |
|---|---|
| API project compilation | Passed |
| MVC project compilation | Passed |
| Organiser authentication | Previously tested successfully |
| Event creation | Passed |
| Event editing | Passed |
| Event deletion | Passed |
| Participant enrolment | Passed |
| View participant enrolment status | Passed |
| Cancel participant enrolment | Passed |

The Visual Studio rebuild confirmed that both projects compiled successfully with zero build errors.

Category management and the Organiser Results page should be verified in the final application run before being recorded as fully tested.

## 10. Security

The application uses JWT authentication to protect API functionality.

Role-based authorisation separates Organiser and Participant operations.

The MVC application also uses session information to control access to protected pages.

Form submissions use ASP.NET Core anti-forgery validation where implemented.

Sensitive credentials and production secrets should not be committed to a public repository.

## 11. Challenges and Solutions

During development, several technical issues were encountered.

**Project compilation errors:** Missing namespace references and duplicate authorization classes initially prevented the solution from compiling. These were resolved by correcting the project structure and removing duplicate definitions.

**API and MVC integration:** The MVC application was configured to communicate with the API through HTTP requests and JWT authentication.

**Database connectivity:** The API was configured to connect to the RaceDay SQL Server database.

**Participant enrolments:** The system was extended to allow participants to enrol in event categories, view their enrolments and cancel registrations.

## 12. Future Improvements

Potential improvements include:

- Email notifications for successful enrolments.
- Online payment integration for event entry fees.
- More detailed participant dashboards.
- Race result filtering and reporting.
- Improved event search functionality.
- Deployment to a cloud hosting platform.

## 13. Demonstration Video

**YouTube Demonstration:** [PASTE YOUR VIDEO LINK HERE]

The demonstration should show the application running, authentication, Organiser functionality, Participant Enrolments, Swagger and the successful solution build.

## 14. Conclusion

RaceDay demonstrates the integration of an ASP.NET Core MVC frontend with a RESTful Web API and SQL Server database.

The application provides a foundation for managing sporting events, participant registrations and race results. It also demonstrates important software development concepts, including relational database design, authentication, authorisation, CRUD operations and API integration.

The project was developed as part of the Programming 2B Portfolio of Evidence.
