# CLAUDE.md — Reglas de comportamiento para asistentes de IA

> Este archivo define **cómo debe pensar y trabajar** cualquier asistente de IA
> (Claude Code, ChatGPT, Gemini, etc.) en este repositorio.
> No describe el sistema (ver `ARCHITECTURE.md`) ni las decisiones técnicas
> concretas del proyecto (ver `PROJECT_RULES.md`).
>
> Mantener este documento por debajo de ~30 reglas. Si crece, fusionar y podar.

---

## 1. Principio N.º 1 — No sobreingeniería

**Nunca agregues abstracciones, patrones o complejidad que no resuelvan un
problema real y presente.** Preferí siempre la solución más simple que mantenga
la arquitectura limpia.

No crear interfaces, factories, decorators, servicios genéricos ni capas
adicionales "por si acaso". Si una funcionalidad necesita dos clases, son dos
clases, no quince. La complejidad se agrega cuando un problema concreto la
exige, nunca antes.

Ante la duda entre dos soluciones, gana la más simple de leer.

---

## 2. Filosofía del proyecto

Este proyecto **no es un sistema para un cliente específico**. Es el core de una
plataforma SaaS multi-tenant, comercial, pensada para mantenerse durante años y
servir a múltiples clientes (tenants).

- Cada decisión debe favorecer la **mantenibilidad a largo plazo**.
- Optimizá para que un desarrollador pueda entender y modificar el código
  **dentro de dos años** (probablemente seas vos mismo).
- Nunca sacrifiques legibilidad para ahorrar unas pocas líneas.
- Código simple por encima de código "inteligente".

---

## 3. Cómo trabajar

- **Nunca asumas cómo funciona el código existente.** Antes de modificar una
  funcionalidad: inspeccioná la implementación actual, entendé el flujo,
  reutilizá lo que ya existe, y recién después proponé cambios.
  **No dupliques lógica ya implementada.** (Ej: si no encontrás dónde se hace
  el login, buscalo mejor; no crees un segundo servicio de login.)
- Seguí las convenciones del repo. Mirá cómo están hechas las entidades,
  handlers y configuraciones actuales antes de escribir, y seguí ese estilo.
- Escribí código listo para producción, no código de ejemplo.
- No dupliques lógica. Si algo ya existe, reutilizalo.
- Si algo del repo actual no encaja con lo que asumís, **avisá antes de
  improvisar** una solución que rompa la arquitectura.

---

## 4. Qué nunca hacer

- No romper el aislamiento entre capas de Clean Architecture.
- El dominio nunca referencia Infrastructure, HttpContext ni detalles de
  framework.
- No introducir deuda técnica silenciosa. Si la introducís a conciencia,
  declarala.
- No tomar decisiones de arquitectura importantes sin justificarlas.
- El `TenantId` nunca se obtiene de datos enviados por el frontend.

---

## 5. Cuándo frenar y consultar

Implementá **directo** las tareas de rutina. No actúes como consultor para cada
entidad o cambio menor.

**Frená y consultá solo si** una decisión compromete:
- la escalabilidad futura,
- la seguridad,
- el aislamiento entre tenants,
- o el aislamiento entre capas.

En esos casos: detené la implementación, explicá el problema y proponé
alternativas antes de avanzar.

---

## 6. Flujo de implementación

Cuando implementes una funcionalidad, seguí este orden:

1. Analizar el código existente relevante.
2. Explicar el enfoque en menos de 10 líneas.
3. Implementar de forma completa.
4. Verificar que compile **y que no introduzca warnings nuevos**.
5. Revisar que respete Clean Architecture y las convenciones del repo.
6. Informar cualquier deuda técnica detectada.

**Proporcionalidad:** para cambios triviales (renombrar, ajustes menores) este
flujo puede comprimirse. Para cambios que tocan arquitectura, multi-tenancy o
seguridad, seguilo completo.

---

## 7. Sobre estos documentos

- `CLAUDE.md` (este) → cómo pensar y trabajar.
- `ARCHITECTURE.md` → cómo está construido el sistema.
- `PROJECT_RULES.md` → qué decisiones técnicas concretas tomamos.
- `DECISIONS.md` → por qué tomamos las decisiones importantes (ADRs).

Si detectás un error recurrente que valga una regla nueva, proponé agregarla al
documento que corresponda. Este sistema mejora de forma acumulativa, pero solo
si se mantiene podado: agregar reglas tiene como contrapartida fusionar y borrar
las que sobran.
