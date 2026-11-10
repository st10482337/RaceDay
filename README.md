# RaceDay Event Management System

## 1. Project Overview

RaceDay is a web-based Event Management System developed to manage running, walking and cycling events. The system provides a central platform where participants can discover events, enrol in available activities, manage their profiles and view their race results.

The system also provides organisers with tools to create and manage events, maintain event categories, oversee participant enrolments and record race results.

RaceDay is developed using ASP.NET Core Web API and ASP.NET Core MVC, with SQL Server used for relational data storage. The project also incorporates Azure Blob Storage for image storage, GitHub for version control and GitHub Actions for continuous integration.

## 2. Project Objectives

The main objectives of RaceDay are to:

- Provide secure user registration and authentication.
- Support separate Participant and Organiser roles.
- Allow organisers to create, update and delete events.
- Allow organisers to manage event categories.
- Allow participants to browse and view event details.
- Allow participants to enrol in events and select categories.
- Allow organisers to view and manage participant enrolments.
- Record and manage participant race results.
- Allow participants to view their finishing times and positions.
- Allow participants to manage their profiles and profile pictures.
- Support event banner and profile picture storage.
- Maintain data integrity through relational database constraints.
- Use GitHub for source control and project collaboration.
- Use automated builds and tests through GitHub Actions.

## 3. System Users and Roles

### 3.1 Organiser

Organisers are responsible for managing sporting events and overseeing participant activities.

Organisers can:

- Access the organiser dashboard.
- View event management information.
- Create new events.
- Edit existing event details.
- Delete events.
- Manage event categories.
- View participant enrolments.
- Monitor enrolment statuses.
- Record participant race results.
- Update race results.

### 3.2 Participant

Participants use the system to discover events and manage their participation.

Participants can:

- Register for an account.
- Log in to the system.
- View available events.
- View individual event details.
- Enrol in events.
- Select an event category.
- View their enrolled events.
- Manage their profile information.
- Upload a profile picture.
- View their race results, including finishing times and positions.

## 4. Main System Features

### 4.1 Authentication and User Management

The authentication functionality allows users to register and log in. Access to system functionality is controlled according to the user's role.

### 4.2 Event Management

Organisers can create, update and delete sporting events. Event information includes details such as the event name, date, location, distance and event type.

Participants can browse available events and view additional event information before enrolling.

### 4.3 Category Management

Event categories allow organisers to define different participation options for events. Categories can include distance-based or age-based classifications.

### 4.4 Participant Enrolments

Participants can enrol in available events and select the relevant category. Organisers can view enrolment information and manage participant enrolment statuses.

### 4.5 Results Management

Organisers can record participant race results, including finishing times and finishing positions.

Participants can access their personal results through the participant interface.

### 4.6 Participant Profiles

Participants can view and update their profile information. The profile functionality also supports uploading a profile picture.

### 4.7 Image Storage

Azure Blob Storage is used to store uploaded images, including event banners and participant profile pictures.

The application uses the API to handle image storage and retrieval, allowing uploaded images to be displayed through the MVC interface.

## 5. System Architecture and Technologies

RaceDay uses a layered application structure to separate the user interface, business functionality and data storage.

The main technologies used include:

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API | Backend services and API endpoints |
| ASP.NET Core MVC | Web application and user interface |
| Entity Framework Core | Database access and management |
| Microsoft SQL Server | Relational database |
| Azure Blob Storage | Image storage |
| HTML and CSS | Web page structure and styling |
| JavaScript | Client-side functionality |
| Git and GitHub | Version control |
| GitHub Actions | Continuous integration |
| Docker | Containerisation and deployment support |

## 6. Database Design

The RaceDay database contains six main entities:

1. USER
2. USER_PROFILE
3. EVENT
4. CATEGORY
5. ENROLMENT
6. RESULT

The database uses:

- Primary keys to uniquely identify records.
- Foreign keys to establish relationships between tables.
- NOT NULL constraints for required information.
- UNIQUE constraints to prevent duplicate values where applicable.
- CHECK constraints to restrict invalid data.
- Default values where appropriate.
- Relational integrity between related entities.

The SQL database script is located in:

`docs/RaceDayDB.sql`

## 7. API Functionality

The Web API provides endpoints for the main system operations, including:

- Authentication
- User profiles
- Events
- Categories
- Enrolments
- Results
- Image uploads and retrieval

The API uses standard HTTP methods, including GET, POST, PUT and DELETE, to retrieve, create, update and remove information.

The API documentation is available in:

`docs/API.pdf`

## 8. Azure Blob Storage Integration

Azure Blob Storage is used to store images separately from the main relational database.

The application supports image uploads for event banners and participant profile pictures.

The image storage process involves:

1. A user selects an image through the MVC interface.
2. The MVC application sends the image to the Web API.
3. The API processes the upload through the blob storage service.
4. The image is stored in an Azure Blob Storage container.
5. The application retrieves and displays the stored image when required.

This approach allows image files to be stored and retrieved without storing the actual image data directly in the SQL Server database.

## 9. Version Control

Git and GitHub are used to manage the development of the RaceDay project.

The repository contains the source code, project documentation and supporting configuration files.

Git commits are used to track development progress, record changes and maintain a history of the project.

**GitHub Repository:**

https://github.com/st10482337/RaceDay

## 10. Continuous Integration

GitHub Actions is used to automatically validate and test the RaceDay project whenever changes are pushed to the main branch or a pull request is created.

The workflow is stored in `.github/workflows/ci.yml`.

The CI pipeline performs the following tasks:

- Checks out the repository.
- Validates the required Part 1 documentation files.
- Restores NuGet dependencies.
- Builds the RaceDay API.
- Builds the RaceDay MVC application.
- Builds the API test project.
- Runs automated API tests.
- <img width="930" height="1016" alt="image" src="https://github.com/user-attachments/assets/366c1291-6c72-449e-87c4-132b02c381d7" />


The workflow completed successfully, as indicated by the green success status on GitHub Actions.
## 11. Running RaceDay with Docker

RaceDay can be run locally using Docker Compose. The setup includes the SQL Server database, ASP.NET Core Web API and MVC web application.

### Requirements
- Docker Desktop
- WSL 2 enabled on Windows

### Starting the Application

1. Open Docker Desktop and make sure the Docker Engine is running.
2. Open PowerShell in the root RaceDay project folder.
3. Create a `.env` file by copying `.env.example`.
4. Run the following command to build and start the application:

```powershell
docker compose up --build -d
```

Docker Compose automatically starts SQL Server, creates and populates the database, and starts the API and MVC application.

### Application URLs

- MVC Website: http://localhost:8081
- Web API: http://localhost:8080
- Swagger API Documentation: http://localhost:8080/swagger

### Stopping the Application

Run:

```powershell
docker compose down
```

The database uses a persistent Docker volume. Avoid using `docker compose down -v` because this removes the database volume and its stored data.

### Azure Blob Storage

Azure Blob Storage configuration is optional for running the application locally. Image upload functionality requires valid Azure Blob Storage settings.
## 12. Project Demonstration

The project demonstration video is available below:

https://youtu.be/Qvy8HtJOBFs

The final Part 3 demonstration should cover the application functionality, participant and organiser interfaces, image storage integration and Docker configuration.

## 13. AI Usage Disclosure

AI tools were used as supplementary development and learning resources during the project. They assisted with understanding technical concepts, troubleshooting errors and reviewing aspects of the implementation.

The development, testing, integration and verification of the project remain part of the student's work and responsibility.

## 14.YouTube link


## 15. Part 1 Documentation

The `docs` directory contains the original planning and database documentation.

| Document | Description |
|---|---|
| ERD.pdf | Entity Relationship Diagram |
| API.pdf | API Endpoint Plan |
| RaceDayDB.sql | SQL database creation script |

## 16. Repository Structure

The repository is organised into the following main areas:

- `.github/workflows/` – GitHub Actions workflow configuration.
- `docs/` – Part 1 planning and database documentation.
- `part 2/` – RaceDay application solution and related project files.
- `README.md` – Project overview and documentation.

---

**Project:** RaceDay Event Management System  
**Repository:** st10482337/RaceDay  
**Framework:** .NET 10  
**Database:** Microsoft SQL Server
