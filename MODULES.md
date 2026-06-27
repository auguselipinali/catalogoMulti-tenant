# MODULES.md — Estado de los módulos

> Qué módulos existen y en qué estado. Evita que una IA o un colaborador asuma
> que algo está construido cuando todavía no lo está.
>
> Regla de este documento: un módulo entra acá cuando es **real** (en construcción
> o terminado) o cuando hay un cliente concreto que lo va a pagar. No se listan
> módulos hipotéticos para que el roadmap no se vuelva una fantasía. La sección
> "Futuro posible" queda a propósito corta y sin compromiso.

---

## Estados posibles

- **Done** — implementado y en uso.
- **In Progress** — en construcción activa.
- **Next** — es lo próximo a construir, con necesidad concreta.
- **Future** — idea válida, sin trabajo ni compromiso todavía.

---

## Core

| Componente            | Estado       | Notas                                         |
|-----------------------|--------------|-----------------------------------------------|
| Multi-tenant (Tenant, User→Tenant, resolución, aislamiento) | **Next** | Primera tarea de código. Esqueleto mínimo. |

## Módulos de negocio

| Módulo    | Estado       | Notas                                              |
|-----------|--------------|----------------------------------------------------|
| Catálogo  | **Next**     | Productos, categorías, marcas, imágenes. Panel admin + carrito a WhatsApp. Primer módulo de la plataforma. Depende del core multi-tenant. |

---

## Futuro posible

Ideas válidas, **sin compromiso ni fecha**. Solo se mueven a "Next" cuando hay un
cliente real que las necesita y paga:

- Pago online (Mercado Pago) — opcional, nunca el núcleo (ver `VISION.md`).
- Gestión de stock / inventario.
- Estadísticas / analytics de catálogo.

> Mantener esta lista corta a propósito. Si crece sin que nada se construya, es
> señal de planificación en exceso, no de progreso.
