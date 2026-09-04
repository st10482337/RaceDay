# RaceDay Event Management System

## Project Overview

RaceDay is an event management system designed to support the management of running, walking and cycling events.

The system supports two main user roles:

- Organiser
- Participant

Organisers can create and manage events, define event categories, manage participant enrolments and capture results.

Participants can register for events, select an event category and view their enrolments and results.

## Part 1 Documentation

The following Part 1 planning documents are available in the `docs` folder:

- [ERD](docs/ERD.pdf)
- [API Endpoint Plan](docs/API.pdf)
- [SQL Database Script](docs/RaceDayDB.sql)

## Database

The database contains the following entities:

1. USER
2. USER_PROFILE
3. EVENT
4. CATEGORY
5. ENROLMENT
6. RESULT

The database uses primary keys, foreign keys, constraints and relationships to maintain data integrity.

## API Planning

The API endpoint plan covers:

- Authentication
- User Profiles
- Events
- Categories
- Event Enrolments
- Results

## CI/CD

GitHub Actions is used to validate the required Part 1 documentation files.

The CI workflow checks that the following files exist:

- `docs/ERD.pdf`
- `docs/API.pdf`
- `docs/RaceDayDB.sql`

The workflow runs automatically when changes are pushed to the `main` branch.

## Repository Structure

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
