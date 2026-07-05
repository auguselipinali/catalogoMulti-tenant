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

## 2026-06-23 — Incremento 6: panel de administración (escritura protegida)

Endpoints admin de escritura de productos bajo /admin (POST, PUT, DELETE),
protegidos con [Authorize]. El TenantId sale exclusivamente del JWT, sin slug en
la ruta — esto elimina por diseño el edge case de tenant cruzado declarado en el
Incremento 5. Commands MediatR Create/Update/Delete con validators. Método Update
en el dominio (mutación controlada). KeyNotFoundException → 404, sin jerarquía de
excepciones custom (anti-sobreingeniería).

AISLAMIENTO DE ESCRITURA VERIFICADO a mano de punta a punta: POST con JWT crea en
el tenant correcto (201), producto visible solo en su tenant, PUT/DELETE sobre id
de otro tenant da 404 (el filtro lo aísla), POST sin JWT da 401.

---

## 2026-06-23 — CORS para el frontend

Política CORS nombrada en la WebApi que permite el origin del frontend Vite,
leído de appsettings.json (Cors:AllowedOrigins), no hardcodeado. UseCors ubicado
antes de UseAuthentication. Habilita que el catálogo React consuma la API. Build 0/0.

---

## 2026-06-27 — Renombre de slug: lore → caricias-al-alma

El tenant pasa a slug `caricias-al-alma` y Name "Caricias al Alma". DataSeeder
actualizado (guard por el slug nuevo para no duplicar; email admin@lore.com y
productos sin cambios). Data migration RenameTenantLoreToCariciasAlAlma: UPDATE
in-place de la fila existente (slug + name) por el slug viejo, mismo Id, productos
preservados; reversible vía Down. La migración se aplica antes del arranque, así
el guard del seeder encuentra la fila renombrada y no crea un duplicado. Build 0/0.

---

## 2026-07-02 — Categorías multi-tenant (modelo + CRUD)

Bloque de categorías en dos incrementos.

Incremento 1 (modelo de datos): entidad `Category` (hereda de TenantedEntity,
bajo el global query filter automáticamente) y `Product.CategoryId` nullable
(los productos existentes quedan sin categoría, no se rompen). FK opcional con
ON DELETE SET NULL. Migración AddCategories (tabla Categories + columna/índice/FK
en Products).

Incremento 2 (CRUD + asignación): CRUD de categorías bajo /admin/categories
(POST/GET/PUT/DELETE, protegido con JWT, mismo patrón que products).
CreateProduct/UpdateProduct aceptan CategoryId opcional con validación de
aislamiento: AnyAsync sobre el DbSet ya filtrado por tenant — categoría de otro
tenant es invisible → 404, nunca se guarda un producto con categoría ajena.
GET público /{slug}/products expone CategoryId + CategoryName vía LEFT JOIN
(DefaultIfEmpty, conserva productos sin categoría). Sin migración (modelo sin
cambios). Build 0/0.

---

## 2026-07-02 — Dockerfile para Render + seeding de producción

Dockerfile multi-stage (sdk build → aspnet runtime) con restore cacheado por
capa. Kestrel escucha en 0.0.0.0:${PORT:-8080} vía entrypoint shell-form, así
respeta el PORT dinámico de Render; exec entrega el proceso a dotnet para el
SIGTERM. .dockerignore excluye bin/obj/.git del build context.

Seeding separado en esencial vs demo: Caricias al Alma + admin se siembra en
TODOS los entornos (idempotente por slug), Nova Modas solo con includeDemoData
(Development, gate por app.Environment.IsDevelopment()). La password del admin
pasa a leerse de la env var SEED_ADMIN_PASSWORD (fallback al valor de dev): ya no
hay credencial real hardcodeada en el repo. Rename SeedDevelopmentDataAsync →
SeedDataAsync (corre también en producción). Build 0/0.

Deploy en Render: cargar ConnectionStrings__DefaultConnection, SEED_ADMIN_PASSWORD,
Jwt__SecretKey y Cors__AllowedOrigins como env vars del servicio.

---
