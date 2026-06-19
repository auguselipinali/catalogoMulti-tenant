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

> Próxima entrada esperada: implementación de la entidad `Tenant` y el esqueleto
> multi-tenant. **Escribir esa entrada cuando el código exista y compile, no
> antes.**
