# ZyphraTrades

ZyphraTrades is a professional desktop trading journal and analytics application built with .NET 8 and WPF, designed using Clean Architecture and MVVM principles.

The project is conceived as a real, scalable product rather than a simple demo, with a reusable business core prepared for future Web API and Mobile implementations.

---

## Vision

Trading performance is not only defined by entries and exits, but also by discipline, consistency, and emotional control.

ZyphraTrades aims to become a complete platform for:
- Trade journaling
- Performance analytics
- Emotional tracking
- Long-term trader development

---

## Architecture Overview

The solution follows Clean Architecture principles to ensure separation of concerns, maintainability, and scalability.

ZyphraTrades
│
├── ZyphraTrades.Domain // Business entities and core rules
├── ZyphraTrades.Application // Use cases and application contracts
├── ZyphraTrades.Infrastructure // Entity Framework Core, SQLite, repositories
└── ZyphraTrades.Presentation // WPF UI (MVVM)

### Dependency Rule

- Domain is independent and does not depend on any other layer.
- Application depends only on Domain.
- Infrastructure depends on Application and Domain.
- Presentation depends on Application and Infrastructure.

This structure allows replacing the UI or persistence layer without affecting business logic.

---

## Technology Stack

- .NET 8 (LTS)
- C#
- WPF (Windows Presentation Foundation)
- MVVM Pattern
- SQLite
- Entity Framework Core
- Dependency Injection
- Async / Await
- Custom dark user interface

---

## Current Features

- Clean Architecture foundation
- Modular project structure
- Local SQLite database with automatic creation
- Entity Framework Core integration
- Dependency Injection via Microsoft.Extensions.Hosting
- Asynchronous commands to keep the UI responsive
- Styled dark UI with cards and DataGrid
- Trade entity persistence
- Architecture ready for future growth

---

## User Interface

The user interface is designed with a modern dark theme focused on clarity, usability, and performance, following Windows desktop application standards.

---

## Roadmap

### Short Term
- Trade creation and editing
- Trade deletion
- Sessions and market context tracking
- Emotional journaling
- Core metrics (win rate, expectancy, drawdown)

### Mid Term
- Advanced analytics dashboards
- Equity curves and performance charts
- Trade import and export functionality

### Long Term
- ASP.NET Core Web API
- Web-based frontend
- Mobile application (MAUI or Blazor Hybrid)
- Cloud synchronization
- Multi-device support

---

## Project Goals

- Maintain clean and testable code
- Follow professional software architecture practices
- Enable long-term scalability without major refactoring
- Support disciplined and consistent trading workflows

---

## Status

This project is under active development and evolving incrementally following clean code and architecture best practices.
