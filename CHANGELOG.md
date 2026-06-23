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

## 2026-06-19 — Incremento 2: abstracción del tenant del request

`ITenantContext` en Application (TenantId/TenantSlug del request actual),
clase base `TenantedEntity` en Domain (TenantId para entidades de negocio;
Tenant no la hereda), y `TenantContextService` en Infrastructure que lee los
claims vía IHttpContextAccessor. `dotnet build` en verde, 0 warnings. Deuda
declarada: sin JWT, TenantId/TenantSlug son null y no hay aislamiento real
todavía; se resuelve en Incremento 3.

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

## 2026-06-19 — Incremento 4: aislamiento automático entre tenants

Global query filter en EF Core aplicado automáticamente a toda subclase de
TenantedEntity (reflexión una vez al construir el modelo, declarada como excepción
intencional). El filtro lee ITenantContext.TenantId dinámicamente por request;
con tenant null devuelve cero filas (fail-closed). SaveChangesAsync auto-asigna
TenantId a entidades nuevas y lanza InvalidOperationException si no hay tenant
(no guarda con Guid.Empty). NullTenantContext en DesignTimeDbContextFactory para
las migraciones. Mecanismo documentado en ARCHITECTURE.md sección 3. `dotnet build`
0/0. Salda la deuda de aislamiento declarada en Incrementos 2 y 3. Pendiente:
verificación real del aislamiento, posible recién en Incremento 5 con Product.

---

## 2026-06-23 — Incremento 5: módulo Catálogo (lectura pública) + aislamiento verificado

Entidad `Product` (hereda de TenantedEntity, primera entidad de negocio bajo el
global query filter). Resolución de tenant por slug en rutas públicas vía
TenantSlugMiddleware + fallback en TenantContextService (prioridad: JWT > slug).
GET público /{slug}/products. SaveChangesAsync refinado para permitir el seeder
(TenantId explícito con contexto null). Segundo tenant "nova" en seed para probar
aislamiento. Migración AddProducts.

AISLAMIENTO VERIFICADO de punta a punta con dos tenants reales: /lore/products
devuelve solo perfumería, /nova/products solo ropa, sin cruce de ids; slug
inexistente da 404. Valida los Incrementos 2-3-4.

Deuda declarada: edge case de JWT de tenant A en URL de tenant B (a validar en
Incremento 6). Setup operativo requiere paquete EFCore.Design 8.0.x en WebApi.

---
