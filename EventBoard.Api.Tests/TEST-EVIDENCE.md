# Test Execution Evidence

**Generated:** 2026-07-21 (run on `dotnet test`, .NET 8).

This file is the proof behind the "Pass" results in [UAT.md](UAT.md). It is the
raw output of the automated suite. Reproduce with:

```bash
cd EventBoard.Api.Tests
dotnet test --collect:"XPlat Code Coverage"

# Regenerate the HTML drill-down report (optional):
reportgenerator \
  -reports:TestResults/*/coverage.cobertura.xml \
  -targetdir:../CoverageReport \
  -reporttypes:"Html;TextSummary"
```

## Result: 64 / 64 passed, 0 failed

- **Line coverage: 92.2%** (target > 80%)
- Coverage screenshot (committed): [coverage-proof/coverage-report.png](coverage-proof/coverage-report.png)
- Coverage summary (committed): [coverage-proof/coverage-summary.txt](coverage-proof/coverage-summary.txt)
- Full drill-down HTML report is regenerated on demand into `CoverageReport/` (gitignored)
  via `reportgenerator` — see command below.

## Passing tests

- ✅ AuthControllerIntegrationTests.Login_UnknownEmail_ReturnsUnauthorized
- ✅ AuthControllerIntegrationTests.Login_ValidCredentials_ReturnsToken
- ✅ AuthControllerIntegrationTests.Login_WrongPassword_ReturnsUnauthorized
- ✅ AuthControllerIntegrationTests.Register_DuplicateEmail_ReturnsConflict
- ✅ AuthControllerIntegrationTests.Register_IgnoresClientSuppliedAdminRole_CreatesPlainUser
- ✅ AuthControllerIntegrationTests.Register_InvalidModel_ReturnsBadRequest
- ✅ AuthControllerIntegrationTests.Register_NewUser_ReturnsOk
- ✅ AuthServiceTests.LoginAsync_InvalidPassword_ReturnsNull
- ✅ AuthServiceTests.LoginAsync_UserNotFound_ReturnsNull
- ✅ AuthServiceTests.LoginAsync_ValidCredentials_ReturnsToken
- ✅ AuthServiceTests.RegisterAsync_EmailAlreadyExists_ThrowsException
- ✅ AuthServiceTests.RegisterAsync_ValidUser_ReturnsUserId
- ✅ BookingsControllerIntegrationTests.BookEvent_NewEvent_ThenDuplicate_ReturnsCreatedThenBadRequest
- ✅ BookingsControllerIntegrationTests.BookEvent_UnknownEvent_ReturnsNotFound
- ✅ BookingsControllerIntegrationTests.GetAllBookings_AsAdmin_ReturnsOk
- ✅ BookingsControllerIntegrationTests.GetAllBookings_AsUser_ReturnsForbidden
- ✅ BookingsControllerIntegrationTests.GetEventBookings_AsAdmin_ReturnsOk
- ✅ BookingsControllerIntegrationTests.GetMyBookings_AsUser_ReturnsOk
- ✅ BookingsControllerIntegrationTests.GetMyBookings_WithoutToken_ReturnsUnauthorized
- ✅ BookingsControllerIntegrationTests.UpdateBookingStatus_UnknownBooking_ReturnsNotFound
- ✅ CategoriesControllerIntegrationTests.CreateCategory_AsAdmin_DuplicateName_ReturnsConflict
- ✅ CategoriesControllerIntegrationTests.CreateCategory_AsAdmin_UniqueName_ReturnsCreated
- ✅ CategoriesControllerIntegrationTests.CreateCategory_AsUser_ReturnsForbidden
- ✅ CategoriesControllerIntegrationTests.CreateCategory_WithoutToken_ReturnsUnauthorized
- ✅ CategoriesControllerIntegrationTests.DeleteCategory_AsAdmin_CreatedThenDeleted_ReturnsNoContent
- ✅ CategoriesControllerIntegrationTests.DeleteCategory_AsAdmin_NonExisting_ReturnsNotFound
- ✅ CategoriesControllerIntegrationTests.GetCategories_Anonymous_ReturnsOk
- ✅ EventServiceTests.CreateEvent_NullEvent_ThrowsArgumentNullException
- ✅ EventServiceTests.CreateEvent_ValidEvent_ReturnsCreatedEvent
- ✅ EventServiceTests.DeleteEvent_ExistingId_ReturnsTrue
- ✅ EventServiceTests.DeleteEvent_InvalidId_ReturnsFalse
- ✅ EventServiceTests.DeleteEvent_NonExistingId_ReturnsFalse
- ✅ EventServiceTests.GetAllEvents_ReturnsAllEvents
- ✅ EventServiceTests.GetEventById_ExistingId_ReturnsEvent
- ✅ EventServiceTests.GetEventById_InvalidId_ReturnsNull
- ✅ EventServiceTests.GetEventById_NonExistingId_ReturnsNull
- ✅ EventsControllerIntegrationTests.CreateEvent_WithoutToken_ReturnsUnauthorized
- ✅ EventsControllerIntegrationTests.DeleteEvent_WithoutAuthentication_ReturnsUnauthorized
- ✅ EventsControllerIntegrationTests.DeleteEvent_WithoutToken_ReturnsUnauthorized
- ✅ EventsControllerIntegrationTests.GetAllEvents_ReturnsOk
- ✅ EventsControllerIntegrationTests.GetEventById_InvalidId_ReturnsBadRequest
- ✅ EventsControllerIntegrationTests.GetEventById_NonExistingId_ReturnsNotFound
- ✅ EventsControllerIntegrationTests.GetEventById_NotFound_ReturnsNotFound
- ✅ EventsControllerIntegrationTests.UpdateEvent_WithoutToken_ReturnsUnauthorized
- ✅ EventsUploadAndAdminTests.CreateUpdateDelete_AsAdmin_FullLifecycle
- ✅ EventsUploadAndAdminTests.DeleteEvent_AsAdmin_UnknownId_ReturnsNotFound
- ✅ EventsUploadAndAdminTests.GetEventsByCategory_ReturnsOk
- ✅ EventsUploadAndAdminTests.UpdateEvent_AsAdmin_UnknownId_ReturnsNotFound
- ✅ EventsUploadAndAdminTests.UploadImage_DisallowedExtension_ReturnsBadRequest
- ✅ EventsUploadAndAdminTests.UploadImage_FakeImage_RenamedToPng_ReturnsBadRequest
- ✅ EventsUploadAndAdminTests.UploadImage_ValidPng_ReturnsOkWithUrl
- ✅ EventsUploadAndAdminTests.UploadImage_WithoutToken_ReturnsUnauthorized
- ✅ FavoritesControllerIntegrationTests.GetMyFavorites_AsUser_ReturnsOk
- ✅ FavoritesControllerIntegrationTests.GetMyFavorites_WithoutToken_ReturnsUnauthorized
- ✅ FavoritesControllerIntegrationTests.RemoveFavorite_Existing_ReturnsNoContent
- ✅ FavoritesControllerIntegrationTests.RemoveFavorite_NotFavorited_ReturnsNotFound
- ✅ FavoritesControllerIntegrationTests.ToggleFavorite_AddThenRemove_TogglesState
- ✅ FavoritesControllerIntegrationTests.ToggleFavorite_UnknownEvent_ReturnsNotFound
- ✅ ReportsControllerIntegrationTests.GetEventsReport_AsAdmin_ReturnsAggregatedRows
- ✅ ReportsControllerIntegrationTests.GetEventsReport_AsAdmin_WithDateFilter_ReturnsOk
- ✅ ReportsControllerIntegrationTests.GetEventsReport_AsUser_ReturnsForbidden
- ✅ ReportsControllerIntegrationTests.GetEventsReport_WithoutToken_ReturnsUnauthorized
- ✅ WeatherControllerIntegrationTests.GetWeatherForEvent_ExistingEvent_ReturnsOkAndDegradesGracefully
- ✅ WeatherControllerIntegrationTests.GetWeatherForEvent_UnknownEvent_ReturnsNotFound
