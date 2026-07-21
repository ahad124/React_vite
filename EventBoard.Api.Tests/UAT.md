# EventBoard Application - User Acceptance Testing (UAT) Plan

## Project
EventBoard API

## Objective
The purpose of this User Acceptance Testing (UAT) plan is to verify that the EventBoard application meets the business requirements and provides the expected functionality for end users before deployment.

---

# Risk Ranking

| Priority | Feature | Risk |
|----------|---------|------|
| 1 | User Registration & Login | High |
| 2 | Create Event | High |
| 3 | Update Event | High |
| 4 | Delete Event | High |
| 5 | View Events | Medium |
| 6 | Event Booking | Medium |
| 7 | Favorites | Medium |
| 8 | Event Image Upload | High |
| 9 | Weather Information | Low |

> **Note on risk:** *Event Image Upload* and *Weather Information* are the two features
> added most recently and are the riskiest to integrate (file handling and an external
> third-party API). They are covered by scripts **UAT-FILE-001** and **UAT-API-001**
> below.

---

# UAT-01: User Registration

**Priority:** High

### Objective
Verify that a new user can successfully register.

### Preconditions
- User is not already registered.

### Test Steps
1. Open the application.
2. Navigate to Register.
3. Enter a username.
4. Enter a unique email.
5. Enter a password.
6. Click **Register**.

### Expected Result
- Registration succeeds.
- User account is created.
- User can log in using the new credentials.

### Status
Pass / Fail

---

# UAT-02: User Login

**Priority:** High

### Objective
Verify that registered users can log in.

### Preconditions
- User account already exists.

### Test Steps
1. Open Login page.
2. Enter email.
3. Enter password.
4. Click **Login**.

### Expected Result
- Login succeeds.
- JWT token is generated.
- User is redirected to the dashboard.

### Status
Pass / Fail

---

# UAT-03: Create Event

**Priority:** High

### Objective
Verify that an authenticated organizer can create an event.

### Preconditions
- User is logged in.

### Test Steps
1. Navigate to Create Event.
2. Enter event title.
3. Select category.
4. Select event date.
5. Enter location.
6. Save the event.

### Expected Result
- Event is successfully created.
- Event appears in the event list.

### Status
Pass / Fail

---

# UAT-04: Update Event

**Priority:** High

### Objective
Verify that an organizer can update an existing event.

### Preconditions
- Event already exists.
- User is the organizer.

### Test Steps
1. Open an existing event.
2. Edit the title.
3. Update the date.
4. Save changes.

### Expected Result
- Event information is updated.
- Updated values are displayed.

### Status
Pass / Fail

---

# UAT-05: Delete Event

**Priority:** High

### Objective
Verify that an organizer can delete an event.

### Preconditions
- Event exists.

### Test Steps
1. Select an event.
2. Click Delete.
3. Confirm deletion.

### Expected Result
- Event is removed.
- Event no longer appears in the event list.

### Status
Pass / Fail

---

# UAT-06: View Events

**Priority:** Medium

### Objective
Verify users can browse available events.

### Preconditions
- Events exist in the system.

### Test Steps
1. Open Events page.
2. Scroll through available events.
3. Open an event.

### Expected Result
- Events load successfully.
- Event details are displayed correctly.

### Status
Pass / Fail

---

# UAT-07: Book an Event

**Priority:** Medium

### Objective
Verify users can book an available event.

### Preconditions
- User is logged in.
- Event has available seats.

### Test Steps
1. Open an event.
2. Click **Book Event**.
3. Confirm booking.

### Expected Result
- Booking is successful.
- Booking appears in the user's bookings.

### Status
Pass / Fail

---

# UAT-08: Add Event to Favorites

**Priority:** Medium

### Objective
Verify users can mark an event as a favorite.

### Preconditions
- User is logged in.

### Test Steps
1. Open an event.
2. Click the Favorite icon.
3. Open Favorites page.

### Expected Result
- Event is added to Favorites.
- Favorite event appears in the user's Favorites list.

### Status
Pass / Fail

---

# UAT-FILE-001: Event Image Upload

**Priority:** High

### Objective
Verify that an admin can upload a valid event image and that files which only *pretend*
to be images (spoofed name / content-type) are rejected.

### Preconditions
- User is logged in as an **Admin**.

### Test Steps
1. Log in as an admin and obtain a token.
2. `POST /api/events/upload-image` with a real image file (`.png`/`.jpg`).
3. Attempt an upload of a non-image file that has been **renamed** to `photo.png`
   and sent with `Content-Type: image/png`.
4. Attempt an upload with a disallowed extension (e.g. `payload.exe`).
5. Attempt an upload **without** an admin token.

### Expected Result
- Step 2: `200 OK`, response contains a server-generated `/uploads/...` URL.
- Step 3: `400 Bad Request` — the magic-byte (file signature) check rejects it even
  though the name and content-type claim it is a PNG.
- Step 4: `400 Bad Request` — extension not in the allow-list.
- Step 5: `401 Unauthorized`.

### Status
Pass — see Evidence table.

---

# UAT-API-001: Weather Information (External API)

**Priority:** Low (feature), High (integration risk)

### Objective
Verify that event weather is displayed when available, and that the app **degrades
gracefully** (never errors) when the external weather provider is unavailable or no API
key is configured.

### Preconditions
- At least one event with a location exists.

### Test Steps
1. `GET /api/weather/event/{id}` for an existing event.
2. `GET /api/weather/event/{id}` for a non-existent event id.
3. Observe behavior when no OpenWeather API key is configured (as in the test/CI env).

### Expected Result
- Step 1: `200 OK` with a `WeatherDto`. When weather cannot be fetched, `Available` is
  `false` and the UI shows *"Weather information is currently unavailable."*
- Step 2: `404 Not Found`.
- Step 3: The endpoint still returns `200` with `Available = false` — no 500, no crash.

### Status
Pass — see Evidence table.

---

# Acceptance Criteria

The application will be accepted if:

- All High-risk UAT scripts pass.
- No Critical defects remain open.
- Authentication works correctly.
- Event CRUD operations function correctly.
- Booking functionality works correctly.
- Favorites functionality works correctly.
- Users can successfully browse events.
- System behaves as expected under normal usage.

---

# Test Execution Summary

Every row below is backed by an automated test that runs in CI. "Pass" means the named
test(s) executed green in the run recorded in
[`TEST-EVIDENCE.md`](TEST-EVIDENCE.md) (64/64 passing). Run them yourself with
`dotnet test` from `EventBoard.Api.Tests/`.

| UAT ID | Feature | Priority | Result | Automated test evidence |
|---------|----------|----------|--------|--------------------------|
| UAT-01 | Registration | High | Pass | `AuthControllerIntegrationTests.Register_NewUser_ReturnsOk`, `Register_DuplicateEmail_ReturnsConflict`, `Register_IgnoresClientSuppliedAdminRole_CreatesPlainUser` |
| UAT-02 | Login | High | Pass | `AuthControllerIntegrationTests.Login_ValidCredentials_ReturnsToken`, `Login_WrongPassword_ReturnsUnauthorized` |
| UAT-03 | Create Event | High | Pass | `EventsUploadAndAdminTests.CreateUpdateDelete_AsAdmin_FullLifecycle`, `EventsControllerIntegrationTests.CreateEvent_WithoutToken_ReturnsUnauthorized` |
| UAT-04 | Update Event | High | Pass | `EventsUploadAndAdminTests.CreateUpdateDelete_AsAdmin_FullLifecycle`, `UpdateEvent_AsAdmin_UnknownId_ReturnsNotFound` |
| UAT-05 | Delete Event | High | Pass | `EventsUploadAndAdminTests.CreateUpdateDelete_AsAdmin_FullLifecycle`, `DeleteEvent_AsAdmin_UnknownId_ReturnsNotFound` |
| UAT-06 | View Events | Medium | Pass | `EventsControllerIntegrationTests.GetAllEvents_ReturnsOk`, `GetEventById_NotFound_ReturnsNotFound` |
| UAT-07 | Book Event | Medium | Pass | `BookingsControllerIntegrationTests.BookEvent_NewEvent_ThenDuplicate_ReturnsCreatedThenBadRequest` |
| UAT-08 | Favorites | Medium | Pass | `FavoritesControllerIntegrationTests.ToggleFavorite_AddThenRemove_TogglesState`, `RemoveFavorite_Existing_ReturnsNoContent` |
| **UAT-FILE-001** | **Event Image Upload** | **High** | **Pass** | `EventsUploadAndAdminTests.UploadImage_ValidPng_ReturnsOkWithUrl`, `UploadImage_FakeImage_RenamedToPng_ReturnsBadRequest`, `UploadImage_DisallowedExtension_ReturnsBadRequest`, `UploadImage_WithoutToken_ReturnsUnauthorized` |
| **UAT-API-001** | **Weather Information** | **Low** | **Pass** | `WeatherControllerIntegrationTests.GetWeatherForEvent_ExistingEvent_ReturnsOkAndDegradesGracefully`, `GetWeatherForEvent_UnknownEvent_ReturnsNotFound` |

**Coverage:** 92.2% line coverage (target > 80%). Committed proof:
screenshot [`coverage-proof/coverage-report.png`](coverage-proof/coverage-report.png)
and [`coverage-proof/coverage-summary.txt`](coverage-proof/coverage-summary.txt).

---

## Prepared By

**Name:** Abdul Ahad

**Project:** EventBoard API

**Testing Type:** User Acceptance Testing (UAT)