# VISION.md — Por qué existe este producto

> Documento no técnico. Responde *para qué* existe el software, no *cómo* está
> hecho. Sirve para que cualquier persona o IA que trabaje en el proyecto
> entienda el propósito antes que la implementación.

---

## Qué es

Una plataforma SaaS multi-tenant para que comercios chicos y medianos tengan su
catálogo online con panel de administración propio, sin depender de un
desarrollador para cada cambio.

El primer cliente (tenant) es **Lore Perfumería**. El primer módulo es un
**catálogo con carrito que deriva el pedido a WhatsApp**.

---

## Qué problema resuelve

Los comercios chicos hoy venden por WhatsApp e Instagram respondiendo producto
por producto, a mano. No tienen forma simple de mostrar su catálogo y recibir
pedidos armados. Las soluciones existentes son o muy caras, o genéricas
(WordPress + plugins frágiles), o exigen conocimiento técnico que no tienen.

Esta plataforma les da: catálogo propio, autonomía para cargar sus productos, y
pedidos que les llegan listos por el canal que ya usan (WhatsApp).

---

## Quién es el cliente

Comercios chicos y medianos (perfumerías, distribuidoras, locales de barrio) que:
- ya venden de forma informal (WhatsApp/Instagram),
- quieren presencia online sin complejidad,
- prefieren coordinar pago y envío ellos mismos,
- no tienen ni quieren tener equipo técnico.

---

## Visión a 5 años

Pasar de "le hago un catálogo a un cliente" a **una plataforma donde sumar un
cliente nuevo es configurar un tenant**, no empezar de cero. El core hecho una
vez, vendido muchas veces. La base técnica de una software factory propia.

---

## Qué NO queremos que sea el producto

Esta sección es deliberada y es un **freno al scope creep**. Si una idea entra en
conflicto con esto, la idea espera o se descarta.

- **No** es un e-commerce con checkout y pasarela de pago como núcleo. El pago lo
  coordina el comerciante (al menos en la fase actual). Pago online es un módulo
  futuro y opcional, no el corazón.
- **No** es una herramienta que requiera conocimiento técnico del comerciante.
- **No** es un producto a medida que se reescribe por cliente. Lo que un cliente
  necesita distinto se resuelve por configuración o módulo reutilizable, no con
  un fork.
- **No** es una plataforma que intente hacer todo (CRM + turnos + facturación +
  contabilidad) desde el día uno. Los módulos se agregan cuando un cliente real
  los paga, no por completitud.
