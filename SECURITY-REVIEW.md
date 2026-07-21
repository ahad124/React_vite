# Security Review — Event Image Upload

Scope: `POST /api/events/upload-image` in
[`EventBoard.Api/Controllers/EventsController.cs`](EventBoard.Api/Controllers/EventsController.cs).
This document records what the upload path checks, what it deliberately does **not**
check, and the residual risks.

## What we check

1. **Authentication + authorization.** The endpoint is `[Authorize(Roles = "Admin")]`,
   so only authenticated admins can upload. Combined with the fix that stops users
   self-registering as Admin, the attack surface is limited to trusted admin accounts.
2. **Request size limit.** `[RequestSizeLimit(5 MB)]` plus an explicit
   `file.Length > MaxImageBytes` check reject oversized files before they are written.
3. **Empty file rejected.** A null or zero-length upload is refused.
4. **Extension allow-list.** Only `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp` are accepted.
5. **Content-Type allow-list.** The declared MIME type must be one of the allowed
   image types.
6. **Magic-byte (file signature) check.** We read the first 12 bytes of the actual file
   content and confirm they match a real image signature consistent with the extension
   (JPEG `FF D8 FF`, PNG `89 50 4E 47 …`, GIF `GIF87a/GIF89a`, WEBP `RIFF … WEBP`).
   This is the check that the extension and Content-Type alone cannot provide, because
   both of those are client-supplied and trivially spoofable.
7. **Server-generated file name.** The file is written as a random `Guid` + safe
   extension into a fixed `wwwroot/uploads` directory. The client's file name is never
   used for the path, which prevents path traversal and overwriting existing files.

## What we do NOT check (yet)

- **Malware / virus scanning.** We confirm the file *starts* like an image, but we do
  not scan the full contents for malware. A placeholder comment marks where a real AV
  scan (e.g. ClamAV/clamd, Windows Defender AMSI, or a cloud scanning API) belongs.
- **Full image decode / re-encode.** We do not decode the image to prove the whole
  file is well-formed. A file can have valid magic bytes and still be a malformed or
  polyglot file (valid image header + trailing payload interpreted as another format).
- **Image dimension / decompression limits.** No guard against "decompression bomb"
  images (small file, huge pixel dimensions) that could exhaust memory on any later
  processing/thumbnailing step.
- **Rate limiting.** No per-user throttling on uploads, so a compromised admin account
  could fill disk within the size limits.
- **Metadata stripping.** EXIF/embedded metadata (which can contain location data or
  scripts) is not stripped.
- **Content Security / serving isolation.** Files are served from the API's own
  `wwwroot`; there is no separate, cookie-less, non-executable static host.

## Residual risks

| Risk | Likelihood | Impact | Mitigation status |
|------|-----------|--------|-------------------|
| Spoofed non-image (renamed executable) | Low | Medium | **Mitigated** by magic-byte check |
| Malware embedded in a valid image | Medium | High | **Open** — needs AV scan (placeholder added) |
| Polyglot / trailing-payload file | Low | Medium | **Open** — needs decode/re-encode |
| Decompression bomb | Low | Medium | **Open** — needs dimension limits |
| Path traversal / overwrite | Low | High | **Mitigated** by random server-side name |
| Disk exhaustion via many uploads | Low | Medium | **Partially** — size limit only, no rate limit |
| Stored XSS via SVG | N/A | — | SVG not in allow-list |

## Recommended next steps

1. Integrate a real virus scanner at the marked location before persisting the file.
2. Decode + re-encode uploads through an imaging library to normalize content and drop
   trailing payloads and metadata.
3. Enforce max pixel dimensions to prevent decompression bombs.
4. Serve uploads from an isolated static host with a restrictive `Content-Type` and
   `Content-Disposition`.
5. Add per-user upload rate limiting.
