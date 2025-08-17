# MeetingSchedulerApi

RESTful API for automated meeting scheduling with conflict detection and optimal time slot finding.

## Features

- **Smart Scheduling**: Automatically finds the earliest available time slot
- **Conflict Detection**: Prevents overlapping meetings for participants
- **Multi-participant Support**: Schedule meetings with multiple attendees
- **User Management**: Create users and retrieve their meetings

## API Endpoints

### Meetings
```http
POST /api/meetings
```
Creates a meeting with automatic time slot detection.

**Request Body:**
```json
{
  "participantId": [1, 2],
  "durationMinutes": 60,
  "earliestStart": "2025-08-18T09:00:00",
  "latestEnd": "2025-08-18T17:00:00"
}
```

### Users
```http
POST /api/users
```
Creates a new user.

```http
GET /api/users/{userId}/meetings
```
Retrieves all meetings for a user.

## Architecture

- **Controllers**: HTTP endpoints and request handling
- **Services**: Business logic and meeting scheduling algorithm
- **Repositories**: Data access layer

## Core Algorithm

1. **Validation**: Check participants, time constraints, and duration
2. **Conflict Detection**: Retrieve existing meetings for all participants
3. **Slot Finding**: Iterate through time window to find first available slot
4. **Meeting Creation**: Map DTO to domain model and persist

## Data Models

**MeetingDto** (Request)
- `ParticipantId: List<int>`
- `DurationMinutes: int`
- `EarliestStart/LatestEnd: DateTime`

**Meeting** (Domain)
- `Id: int`
- `Participants: List<int>`
- `StartTime/EndTime: DateTime`

## Tech Stack

- ASP.NET Core Web API
- C# with Dependency Injection
- xUnit + Moq for testing

## Getting Started

```bash
dotnet restore
dotnet run
```

API available at: `https://localhost:5001`

## Testing

```bash
dotnet test
```
