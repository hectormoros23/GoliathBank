# GoliathBank.TransactionsApi

API en .NET 8 para consultar transacciones y calcular el total acumulado en EUR por SKU a partir de tipos de cambio proporcionados.

---

## Ejecución del proyecto

1. Descargar o clonar el repositorio  

2. Abrir en el navegador:

Al ejecutar el proyecto, Swagger se abrirá automáticamente.

Si necesitas abrirlo manualmente:
- http://localhost:5244/swagger

Desde Swagger se pueden probar todos los endpoints.

Opcionalmente se incluye un archivo `.http` con ejemplos de llamadas.

## Decisiones de diseño y consideraciones

### Uso de .NET y tipos numéricos

- Implementado en .NET 8  

---

### Conversión de divisas

- Cuando no hay conversión directa a EUR, se buscan conversiones intermedias entre monedas hasta llegar a EUR  
- El cambio final se calcula multiplicando los valores de cada paso  
- Las conversiones ya calculadas se guardan en memoria para evitar recalcularlas

---

### Redondeo

Se utiliza **Banker’s Rounding**:

```csharp
Math.Round(valor, 2, MidpointRounding.ToEven)
```

Estrategia aplicada:

- La conversión se calcula con precisión completa (sin redondeo intermedio)  
- `eurAmount` se redondea a 2 decimales al mostrarse en la respuesta  
- `totalEur` se calcula usando los valores sin redondear y se redondea al final a 2 decimales  

Motivo:

- Evitar pequeños errores acumulados por redondeos intermedios  
- Mantener coherencia en importes monetarios

---

### Manejo de casos límite

- SKU sin transacciones  
  → 404 Not Found  

- Moneda sin ruta de conversión a EUR  
  → 422 Unprocessable Entity (errorCode = NO_CONVERSION_PATH)  

- Datos inconsistentes o inválidos en JSON  
  → Error controlado

---

### Manejo de errores y logging

- Manejo centralizado mediante middleware  
- No se devuelven stacktraces al cliente  
- Logging suficiente para diagnóstico en servidor

---

### Inyección de dependencias y estructura

Se aplica inyección de dependencias para:

- Repositories (acceso a datos JSON)  
- Services (lógica de negocio y conversión)

## Tests

Se incluyen tests unitarios para validar Banker’s Rounding en casos como:

- 1.005 → 1.00  
- 1.015 → 1.02
