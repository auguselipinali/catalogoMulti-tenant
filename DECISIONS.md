# DECISIONS.md — Architecture Decision Records (ADRs)

> Registro de decisiones arquitectónicas importantes. Cada ADR documenta una
> decisión **con una alternativa real que fue descartada**.
>
> **Reglas de este documento:**
> - Un ADR es **inmutable**. No se edita una decisión vieja. Si cambia, se
>   escribe un ADR nuevo que diga "supersedes ADR-XXX" y el viejo se deja como
>   estaba. El valor está en ver cómo evolucionó el pensamiento.
> - Solo se registran decisiones con **alternativa considerada y descartada**.
>   Una regla sin alternativa (ej: "usamos Guid") es una regla de
>   `PROJECT_RULES.md`, no un ADR.
> - Cada ADR incluye **consecuencias**: las ataduras que la decisión deja. Es la
>   parte más útil para el lector futuro.

---

## ADR-001 — Aislamiento multi-tenant: Shared Database + TenantId

**Estado:** Aceptada
**Fecha:** 2026-06-19

### Contexto
La plataforma debe servir a múltiples clientes (tenants) con aislamiento de sus
datos. Se evaluaron dos estrategias principales de aislamiento en la base de
datos.

### Decisión
Usar **Shared Database + Shared Schema**: una sola base PostgreSQL con una columna
`TenantId` discriminadora en cada entidad de negocio.

### Alternativa descartada
**Database-per-tenant** (una base por cliente).

### Por qué
- Menor costo de infraestructura (una sola base que sirve a muchos tenants).
- Una sola migración, un solo backup, un solo punto de monitoreo.
- Escala de sobra para la cantidad de clientes proyectada en las fases
  iniciales (decenas a cientos).
- Database-per-tenant agrega complejidad operativa (N migraciones, N backups,
  N conexiones) que no se justifica en esta etapa.

### Consecuencias
- **El aislamiento pasa a depender del código de aplicación**, no de una barrera
  física. Esto obliga a:
  - aplicar el filtro por tenant de forma automática a nivel de persistencia
    (no confiar en que cada query recuerde el `Where`),
  - prohibir queries crudas sin filtro de tenant,
  - asignar el `TenantId` automáticamente al crear entidades.
- El diseño debe mantener la **resolución del tenant abstraída** (el dominio solo
  conoce `ITenantContext`), de modo que un futuro modelo híbrido —algún cliente
  premium con base propia— se implemente cambiando infraestructura, sin tocar
  Domain ni Application.

### Camino de evolución previsto
1. Fase 1 (actual): una base + `TenantId`.
2. Fase 2: base compartida + algún cliente grande con base propia (híbrido).
3. Fase 3 (si hiciera falta): motor de resolución de tenants configurable.

---

> Próximos ADRs candidatos (decisiones ya tomadas con alternativa real, conviene
> registrarlas cuando se confirmen contra el código):
> - Stack .NET 8 sobre Node.js para el core.
> - No AutoMapper (mapeo explícito) vs. AutoMapper.
> - No repositorios genéricos vs. repositorio genérico.
> - Identificación de tenant por path vs. subdominio.
