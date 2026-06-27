# PROJECT_RULES.md — Reglas técnicas del proyecto

> Decisiones técnicas concretas y **verificables** de este proyecto. Cada regla
> debería poder chequearse con un sí/no en una revisión de código.
>
> Criterio de frontera con `ARCHITECTURE.md`: si una regla necesita un párrafo
> para entenderse, su explicación va en `ARCHITECTURE.md` y acá queda solo la
> línea verificable. El *porqué* de una decisión con alternativa descartada va en
> `DECISIONS.md`.

---

## Persistencia y datos

- Todas las primary keys son **`Guid`**. Nunca `int` autoincremental.
- Todas las fechas se almacenan en **UTC**.
- **Soft delete**: las entidades de negocio no se borran físicamente; se marcan
  como eliminadas. *(Pendiente de confirmar el mecanismo concreto contra el repo
  antes de tratarlo como ley.)*
- **EF Core** es el ORM oficial. PostgreSQL como motor.

## Multi-tenancy

- Toda entidad de negocio lleva **`TenantId`** y es obligatorio.
- El `TenantId` se asigna desde el contexto autenticado, **nunca** desde el
  frontend.
- Ninguna query de negocio debe poder devolver datos de otro tenant.

## Aplicación

- **MediatR** para Commands y Queries (CQRS). No lógica de negocio en los
  controllers.
- **FluentValidation** para validación de entrada. No validación manual dispersa.
- **No AutoMapper.** El mapeo se hace explícito. [Decisión a registrar como ADR
  si se quiere dejar el porqué.]
- **No repositorios genéricos.** [Decisión a registrar como ADR si se quiere
  dejar el porqué.]
- **No usar reflection** salvo que exista una justificación documentada. Evitar
  el auto-registro "mágico" (escanear assemblies para registrar cosas): es ilegible
  y difícil de mantener. Registro explícito por defecto.

## Seguridad

- **JWT** para autenticación; el token incluye el claim `tenant_id`.
- **BCrypt** para hash de contraseñas.

---

> Nota: varias de estas reglas (No AutoMapper, No repositorios genéricos, Soft
> delete) son decisiones con alternativas reales descartadas. Si se quiere
> preservar el razonamiento, conviene escribir su ADR en `DECISIONS.md`. Esta
> lista dice *qué* hacemos; el ADR dice *por qué*.
