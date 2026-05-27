---
name: project-manager
description: Analyze the project's current state, review workflows, and suggest functional/technical improvements and next features. Use when asked to plan the next steps, review the project roadmap, or identify missing business logic and technical debt in a booking application.
---

# Project Manager Skill

## Quick start

To use this skill, invoke the `generalist` agent or perform the following steps directly:
1. Scan the root directory for roadmap files (e.g., `plan.md`, `ROADMAP.md`).
2. Analyze the `src/` directory to understand existing domains and features.
3. Compare the current implementation against industry standards for booking systems.
4. Generate or update `ROADMAP.md` in the root folder.

## Workflows

### Project Review & Roadmap Generation

1.  **Baseline Research**: 
    - Read `GEMINI.md` for project standards.
    - Read `plan.md` to see what was recently planned/implemented.
    - List all controllers in `Booker.Backend` and pages in `Booker.Clients.Blazor.Server`.

2.  **Gap Analysis**:
    - **Functional**: Are there missing features like "Cancel Appointment", "Service Search", "Provider Schedules", or "Email Notifications"?
    - **Technical**: Is there hardcoded configuration? Is the test coverage low for critical services? Is the "InMemory" database still used when it shouldn't be for production?
    - **Security**: Are roles being checked in the UI and API?

3.  **Industry Standards Alignment**:
    - Ensure the system supports:
        - Timezone management.
        - Appointment reminders.
        - Buffer times between appointments.
        - Resource management (e.g., specific rooms for services).

4.  **Reporting**:
    - Create/Update `ROADMAP.md` in the root.
    - Organize by priority: `Short-term (Next 2 weeks)`, `Medium-term (1-2 months)`, `Long-term (Backlog)`.
    - Include a "Technical Debt" section.

## Advanced features

- **Task Breakdown**: When suggested a new feature, provide a high-level todo list similar to the `plan.md` format.
- **Dependency Mapping**: Highlight if a new feature requires changes across multiple projects (ApiCaller, Models, Services, etc.).
