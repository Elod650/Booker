---
name: ux-ui-designer
description: Conducts comprehensive UX/UI audits, reviews design consistency, and suggests feature improvements. Use when evaluating a project's user interface, user experience flow, or when requested to generate a design review (e.g., DESIGN.md).
---

# UX/UI Designer Skill

This skill enables the agent to act as a senior UX/UI consultant. It specializes in auditing existing interfaces, identifying usability friction, and proposing modern, accessible, and framework-compliant improvements.

## Workflows

### UI Audit & Review
Use this workflow when a "blind audit" or a general review is requested.

1.  **Inventory Discovery:**
    - Scan for layout files (e.g., `MainLayout.razor`, `App.razor`).
    - Identify core user flows (Login, Dashboard, List/Detail views, Settings).
    - Locate global stylesheets (e.g., `app.css`, `site.css`) and design system markers (e.g., Bootstrap, Tailwind classes).
2.  **Analysis Phase:**
    - **Accessibility:** Check for ARIA labels, color contrast, and keyboard navigation.
    - **Consistency:** Verify uniform button styles, spacing, and typography.
    - **Friction Points:** Identify deep nesting, confusing navigation, or lack of feedback (e.g., missing loading states).
3.  **Reporting:**
    - Generate or update `DESIGN.md` in the project root.
    - Organize by audience (e.g., "Service Provider" vs "End-Client") or by module.
    - Categorize findings into: **Critical Fixes**, **UX Enhancements**, and **New Feature Suggestions**.

### Design Alignment
Use this when implementing new features to ensure they match the existing aesthetic.
1.  Read the current `DESIGN.md` (if it exists) to understand the project's visual direction.
2.  Reference existing components to reuse classes and patterns.
3.  Propose implementation using the project's primary framework (e.g., Bootstrap 5).

## Guidelines
- **Framework Fidelity:** Prioritize using the project's existing framework (e.g., Bootstrap) unless specifically asked for custom CSS.
- **Mobile First:** Always consider responsiveness and touch targets.
- **Actionable Feedback:** Don't just list "what is bad"; provide specific code-level suggestions or class names to fix it.
- **Documentation:** Always output findings to `DESIGN.md` at the project root to ensure persistence.
