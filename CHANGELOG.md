# CHANGELOG.md — Hitos del proyecto

> Registro de hechos grandes y ejecutados, no de releases formales. Sirve para
> reconstruir contexto al volver después de meses.
>
> Regla: acá se anota lo que **ya pasó** (código escrito, decisión ejecutada,
> módulo terminado), no lo que se planea. Si todavía no se hizo, no va.

---

## 2026-06-19 — Base documental de ingeniería

Se crean los documentos fundacionales del proyecto:
`VISION.md`, `CLAUDE.md`, `ARCHITECTURE.md`, `PROJECT_RULES.md`, `DECISIONS.md`,
`MODULES.md`, `CHANGELOG.md`. Se decide stack .NET 8 + Clean Architecture y
estrategia multi-tenant Shared Database + TenantId (ver ADR-001).

---

## 2026-06-19 — Incremento 1: esqueleto multi-tenant

Solución .NET 8 `CatalogoMultiTenant` con cuatro proyectos (Domain, Application,
Infrastructure, WebApi). Entidad `Tenant` (Id, Name, Slug único, CreatedAtUtc),
`AppDbContext` mínimo y migración inicial `InitialCreate`. `dotnet build` en verde,
0 warnings. Deuda declarada: connection string de desarrollo hardcodeada en
DesignTimeDbContextFactory, a resolver en Incremento 2 con variables de entorno.

---

## 2026-06-19 — Incremento 3: autenticación JWT con tenant_id

Entidad `User` (Email único global, PasswordHash, TenantId; no hereda de
TenantedEntity y queda fuera del filtro multi-tenant por diseño del login).
BCrypt para hash, `IJwtTokenService` que emite claims tenant_id/tenant_slug,
LoginCommand con MediatR + FluentValidation, `AuthController` con POST /auth/login,
`IAppDbContext` en Application para no violar capas, y seed de Lore + admin.
Migración AddUsers. `dotnet build` 0/0. Excepción de reflection documentada
(assembly scanning de MediatR/FluentValidation). Deuda: credencial de dev en seed,
a mover a variable de entorno en Incremento 4.

---
