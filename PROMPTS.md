# AI Prompts

This file records the AI prompts used while working on the EventBoard project.
(Prompts specific to writing the test suite live in
[`EventBoard.Api.Tests/AI-Prompts.md`](EventBoard.Api.Tests/AI-Prompts.md).)

## Day — Security hardening of registration & image upload

### Prompt 1 — Fix privileged self-registration

> The registration flow lets anyone register as Admin. The API's `RegisterAsync`
> honors a client-supplied `Role`, and the frontend has a "Register As" dropdown with
> an Admin option. Fix this so public self-registration can only ever create a plain
> "User". Keep the ability to provision Admins server-side (seeding / an admin-only
> path). Remove the misleading role selector from the frontend register form.

### Prompt 2 — Deepen file-type validation on image upload

> The image upload endpoint only checks the file extension and the Content-Type, both
> of which come from the client and can be faked (a bad file renamed to `photo.jpg`
> passes). Add a deeper check: read the first bytes of the uploaded file and verify the
> real file signature (magic bytes) matches the declared image type. Add a comment
> marking where a real virus/malware scan should be integrated.

### Prompt 3 — Write the upload security review

> Add a short `SECURITY-REVIEW.md` at the repo root for the file-upload feature. State
> clearly what we check, what we do not check, and the residual risks (with likelihood
> and impact) plus recommended next steps.

### Prompt 4 — Diagnose weather not working

> Weather doesn't work when someone runs the app. Find out why and surface the fix.
> (Root cause: `OpenWeather:ApiKey` is empty in `appsettings.json`; the service is
> written to degrade gracefully to "unavailable" when no key is present.)

### Prompt 5 — Record the prompts

> The AI prompts used for this day's work were not saved anywhere except the test
> folder. Add a `PROMPTS.md` in the repo root capturing the prompts used for this day.
