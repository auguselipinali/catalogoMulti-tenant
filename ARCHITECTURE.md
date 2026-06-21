# ARCHITECTURE.md — Descripción del sistema

> Este documento describe **cómo está construido** el sistema. Sirve tanto para
> desarrolladores como para asistentes de IA. No es un prompt ni un conjunto de
> reglas de comportamiento (ver `CLAUDE.md`).
>
> Criterio de frontera con `PROJECT_RULES.md`: acá se explica *cómo funciona* un
> mecanismo (con su flujo y su porqué). Las reglas verificables de una línea van
> en `PROJECT_RULES.md`.

---

## 1. Visión general

Plataforma SaaS multi-tenant en **.NET 8**, siguiendo **Clean Architecture**.
El primer tenant es "Lore Perfumería"; el primer módulo de negocio es un catálogo
de productos con panel de administración. La plataforma está diseñada para sumar
tenants y módulos sin reescribir el core.

---

## 2. Clean Architecture — Capas

```
Presentation (WebApi)
        ↓
   Application
        ↓
     Domain
        ↑
 Infrastructure
```

- **Domain**: entidades, value objects, interfaces de dominio, excepciones de
  dominio. No depende de nada externo. No conoce EF Core, HttpContext ni
  frameworks.
- **Application**: casos de uso (CQRS con MediatR), validaciones
  (FluentValidation), interfaces de servicios (puertos). Depende solo de Domain.
- **Infrastructure**: implementaciones concretas (EF Core, persistencia, JWT,
  BCrypt, resolución de tenant). Depende de Application y Domain.
- **Presentation (WebApi)**: controllers, middleware, composición de
  dependencias (DI). Punto de entrada HTTP.

**Dependencias permitidas:** las flechas apuntan hacia adentro. Domain no
referencia a nadie. Application referencia Domain. Infrastructure y WebApi
referencian hacia adentro. Nunca al revés.

---

## 3. Multi-tenancy

**Estrategia actual:** Shared Database + Shared Schema. Una sola base PostgreSQL;
cada entidad de negocio pertenece a un tenant mediante una columna `TenantId`.

**Principio de diseño:** el dominio y los casos de uso **no dependen** de cómo se
resuelve el tenant ni de la estrategia de aislamiento. Solo conocen una
abstracción del tenant actual. Esto deja la puerta abierta a un modelo híbrido
(algún cliente con base propia) sin tocar Domain ni Application. Ver `DECISIONS.md`
(ADR-001).

**Flujo de resolución del tenant:**

```
Request autenticado
      ↓
JWT con claim tenant_id
      ↓
ITenantResolver (Infrastructure) lee el claim
      ↓
ITenantContext (abstracción, Application) expone TenantId al resto de la app
      ↓
Persistencia aplica el TenantId automáticamente
```

- El `TenantId` viaja en el JWT, nunca en el body ni en parámetros enviados por
  el frontend.
**Mecanismo de aislamiento — Global Query Filter:**

`AppDbContext` recibe `ITenantContext` por constructor (ambos Scoped, nueva
instancia por request). En `OnModelCreating`, se iteran todos los tipos del modelo
y, para cada uno que herede de `TenantedEntity`, se aplica via reflexión un
`HasQueryFilter` con la expresión:

```
e => _tenantContext.TenantId.HasValue && e.TenantId == _tenantContext.TenantId!.Value
```

La expresión captura la **referencia** al objeto `_tenantContext`, no su valor:
cada vez que EF Core ejecuta una query, evalúa `_tenantContext.TenantId` en ese
momento y obtiene el TenantId del request en curso. La reflexión ocurre una sola
vez al construir el modelo, no por query (excepción documentada, igual que el
assembly scanning de MediatR/FluentValidation).

**Comportamiento con tenant no resuelto:** si `ITenantContext.TenantId` es `null`
(request sin JWT o con JWT inválido), `HasValue` es `false` → el filtro retorna
`false` para todas las filas → cero resultados. Comportamiento seguro por diseño:
sin contexto de tenant, el developer no ve datos de nadie en lugar de verlo todo.

**`Tenant` y `User` quedan fuera del filtro** porque ninguno hereda de
`TenantedEntity`: `Tenant` es infraestructura de la plataforma; `User` se resuelve
por email antes de conocer el tenant (login).

**Auto-asignación de `TenantId` en `SaveChanges`:** `AppDbContext` sobreescribe
`SaveChangesAsync` para iterar las entidades `TenantedEntity` con estado `Added` y
asignarles el `TenantId` desde `ITenantContext`. Si el contexto no tiene tenant
resuelto al intentar persistir, se lanza `InvalidOperationException` explícita. El
desarrollador no necesita setear `TenantId` al crear una entidad de negocio.

**Identificación del tenant de cara al público:** por path (`/{slug}`) en esta
fase. El slug es un campo del tenant. El mecanismo de resolución está aislado en
un único punto para poder migrar a subdominio sin propagar el cambio.

---

## 4. CQRS

- Commands y Queries vía **MediatR**.
- Cada caso de uso es un handler en su feature folder.
- Validación de entrada con **FluentValidation**, no validación manual dispersa.

---

## 5. Autenticación

- **JWT** para autenticación.
- **BCrypt** para hash de contraseñas (es hash, no cifrado).
- El token incluye, además de los claims de identidad, el claim `tenant_id` que
  asocia al usuario con su tenant.

**Flujo de login (alto nivel):**

```
Credenciales → validación → verificación BCrypt
      ↓
Generación de JWT (incluye tenant_id del usuario)
      ↓
Token devuelto al cliente
```

> El detalle fino del flujo de login y refresh token está **pendiente de
> documentar** (ver sección 8). Documentar contra el código existente, no inventar.

---

## 6. Convenciones

> **Pendiente de documentar** (ver sección 8). Inspeccionar el repo y documentar
> las convenciones de nombres, estructura de feature folders y organización de
> carpetas tal como existen hoy. No inventar.

---

## 7. Autenticación — detalle

Ver sección 5 para el flujo de alto nivel.

---

## 8. Pendientes de documentación

> Esta sección lista lo que **todavía no está documentado**. Su contenido NO debe
> inventarse: se completa inspeccionando el código real o cuando se toma la
> decisión. Mientras esté acá, asumí que el dato no existe aún, no que hay que
> improvisarlo.

- **Flujo de login y refresh token** — detalle fino, contra el código existente.
- **Convenciones del repo** — nombres, feature folders, organización de carpetas.
- **Setup operativo** — cómo levantar el proyecto localmente, variables de
  entorno, cómo correr migraciones EF Core, cómo crear/seedear un tenant nuevo.
  Es el conocimiento que hoy está solo en la cabeza del autor y el primero que se
  pierde al sumar a alguien. Mantenerlo acá o en un futuro `CONTRIBUTING.md`.
