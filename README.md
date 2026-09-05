# RaceDay Event Management System

## 1. Project Overview

RaceDay is an Event Management System designed to manage running, walking and cycling events.

The system provides a central platform where event organisers can create and manage events, define event categories, manage participant enrolments and record race results. Participants can register for an account, maintain their profile, view available events, enrol in events and view their results.

The project is being developed using a structured approach that includes database design, API planning, version control and continuous integration.

---

## 2. Project Objectives

The main objectives of the RaceDay Event Management System are to:

- Allow users to register and authenticate securely.
- Support different user roles.
- Allow Organisers to create and manage events.
- Allow Organisers to create event categories.
- Allow Participants to enrol in events.
- Allow Organisers to manage participant enrolments.
- Record participant race results.
- Allow Participants to view their results.
- Maintain data integrity through relational database constraints.
- Use GitHub for version control.
- Use GitHub Actions for continuous integration and validation.

---

## 3. User Roles

### Organiser

An Organiser is responsible for managing events.

Organisers can:

- Create events.
- Update events.
- Delete events.
- Create event categories.
- Update categories.
- Delete categories.
- View participant enrolments.
- Manage enrolment status.
- Record participant results.
- Update results.

### Participant

A Participant can interact with available events and manage their participation.

Participants can:

- Register for an account.
- Log into the system.
- Manage their profile.
- View available events.
- View event details.
- Enrol in an event.
- Select an event category.
- View their enrolments.
- View their race results.
- Cancel an enrolment where applicable.

---

## 4. Main System Components

The RaceDay system is divided into several functional areas:

### Authentication

Handles user registration and login.

### User Profiles

Stores additional information associated with a registered user.

### Events

Stores information about running, walking and cycling events.

### Categories

Allows events to have specific participation categories.

### Enrolments

Records which Participants have entered which events and categories.

### Results

Stores participant finishing times and finishing positions.

---

## 5. Database Design

The database consists of six main entities:

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
- UNIQUE constraints where duplicate values are not allowed.
- CHECK constraints to restrict invalid values.
- Default values where appropriate.
- Relational integrity between related entities.

The database script is available in:

`docs/RaceDayDB.sql`

---

## 6. API Endpoint Planning

The planned API is divided into six functional areas:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

The API uses standard HTTP methods including:

- GET
- POST
- PUT
- DELETE

The complete API Endpoint Plan is available in:

`docs/API.pdf`

---

## 7. Part 1 Documentation

The `docs` directory contains the planning and database documentation required for Part 1.

### Entity Relationship Diagram

The ERD represents the database entities, attributes, relationships and multiplicities.

File:

`docs/ERD.pdf`

### API Endpoint Plan

The API Endpoint Plan defines the planned endpoints, HTTP methods, routes, descriptions, required roles, request bodies and expected responses.

File:

`docs/API.pdf`

### SQL Database Script

The SQL script creates the RaceDay database, tables, relationships, constraints and sample data.

File:

`docs/RaceDayDB.sql`

---

## 8. Sample Database Data

The database script includes sample data for testing and demonstration purposes.

The sample data includes:

- 2 Organisers
- 2 Participants
- 3 Events
- Categories for each event
- Sample Participant enrolments
- Sample race results

The SQL script was tested successfully in SQL Server Management Studio.

---

## 9. Version Control

Git and GitHub are used to manage the development of the RaceDay project.

The repository contains the project documentation and supporting configuration files.

Meaningful commits are used to record major development milestones and changes.

Repository:

`RaceDay`

---

## 10. Continuous Integration

GitHub Actions is used to perform continuous integration checks.

The CI workflow automatically runs when changes are pushed to the `main` branch or when a pull request targets the `main` branch.

The workflow:

1. Checks out the repository.
2. Checks that the required Part 1 documentation exists.
3. Validates the presence of:
   - `docs/ERD.pdf`
   - `docs/API.pdf`
   - `docs/RaceDayDB.sql`

A successful workflow is indicated by a green check mark in GitHub Actions.

Workflow file:

`.github/workflows/main.yml`

---

## 11. YouTube Link
[https://youtu.be/fRL6_2z8uDo
](url)

## 12. CI
<img width="1407" height="806" alt="image" src="https://github.com/user-attachments/assets/f676697a-50d3-4618-a44f-f11ad8601ac8" />


## 13. Repository Structure

```text
RaceDay/
│
├── .github/
│   └── workflows/
│       └── main.yml
│
├── docs/
│   ├── ERD.pdf
│   ├── API.pdf
│   └── RaceDayDB.sql
│
└── README.md



