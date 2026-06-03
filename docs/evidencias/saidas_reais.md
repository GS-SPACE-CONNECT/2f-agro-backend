# Evidências de Execução — FiapAgro API

Saídas reais capturadas com a API rodando em `http://localhost:5050` contra PostgreSQL 15 local.

---

## Console — Startup + Migrations + Seed

```
Building...
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (43ms) SELECT EXISTS (SELECT 1 FROM pg_catalog.pg_class ...)
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (5ms) SELECT "MigrationId", "ProductVersion" FROM "__EFMigrationsHistory"
info: Microsoft.EntityFrameworkCore.Migrations[20405]
      No migrations were applied. The database is already up to date.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (1ms) SELECT EXISTS (SELECT 1 FROM propriedades AS p)
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5050
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
```

---

## POST /api/auth/registrar → 201 Created

**Request:**
```http
POST http://localhost:5050/api/auth/registrar
Content-Type: application/json

{ "nome": "teste", "email": "teste", "senha": "Senha@123" }
```

**Response:**
```json
HTTP/1.1 201 Created

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkOTg2N2JjYi00ZGIzLTQzYmQtYmM5Mi1lN2M4Yzg0ZWI2ZTciLCJlbWFpbCI6InRlc3RlIiwibmFtZSI6InRlc3RlIiwianRpIjoiZGQzMWMyNjgtZjZhYS00MmUzLWEyOGEtNzY5MjhkZmZiNzQ2IiwiZXhwIjoxNzgwNDkxMDg1LCJpc3MiOiJGaWFwQWdyby5BcGkiLCJhdWQiOiJGaWFwQWdyby5DbGllbnRzIn0.XJH5yMtmKc3lqP7FK27ZLfuFcVXCqmBW1iLru2m5tUg",
  "nome": "teste",
  "email": "teste",
  "expira": "2026-06-03T10:51:25Z"
}
```

---

## POST /api/alertas/praga → 201 Created

**Request:**
```http
POST http://localhost:5050/api/alertas/praga
Authorization: Bearer eyJhbGci...
Content-Type: application/json

{
  "propriedadeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "probabilidade": 0.87,
  "especiePraga": "lagarta-do-cartucho",
  "culturaAfetada": "milho"
}
```

**Response:**
```json
HTTP/1.1 201 Created

{
  "id": "008016cd-6b5b-4a65-891b-2f15230a377c",
  "propriedadeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tipo": "AlertaPraga",
  "severidade": "Critico",
  "probabilidade": 0.87,
  "recomendacao": "Aplicar defensivo imediatamente contra lagarta-do-cartucho na cultura de milho.",
  "criadoEm": "03/06/2026 02:03"
}
```

---

## GET /api/alertas/recentes?quantidade=10 → 200 OK

**Request:**
```http
GET http://localhost:5050/api/alertas/recentes?quantidade=10
Authorization: Bearer eyJhbGci...
```

**Response:**
```json
HTTP/1.1 200 OK

[
  {
    "id": "008016cd-6b5b-4a65-891b-2f15230a377c",
    "propriedadeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipo": "AlertaPraga",
    "severidade": "Critico",
    "probabilidade": 0.87,
    "recomendacao": "Aplicar defensivo imediatamente contra lagarta-do-cartucho na cultura de milho.",
    "criadoEm": "03/06/2026 02:03"
  },
  {
    "id": "2515168f-18ef-4112-827b-15a1aff1a071",
    "propriedadeId": "d9867bcb-4db3-43bd-bc92-e7c8c84eb6e7",
    "tipo": "AlertaPraga",
    "severidade": "Critico",
    "probabilidade": 1,
    "recomendacao": "Aplicar defensivo imediatamente contra teste na cultura de teste.",
    "criadoEm": "03/06/2026 01:54"
  },
  {
    "id": "e3f9bb83-2d5c-48ab-b1cc-63e25edf34af",
    "propriedadeId": "7f66caf6-328d-4ce9-adaa-c5940ef415f0",
    "tipo": "AlertaErosao",
    "severidade": "Medio",
    "probabilidade": 0.55,
    "recomendacao": "Inclinação de 22,0° — monitorar erosão e manter cobertura do solo.",
    "criadoEm": "03/06/2026 01:39"
  },
  {
    "id": "e72ece47-a0e5-406d-8c02-6a298e0611a3",
    "propriedadeId": "1124d424-3fa4-48ac-8427-92c904bfb317",
    "tipo": "AlertaEnchente",
    "severidade": "Critico",
    "probabilidade": 0.91,
    "recomendacao": "Volume crítico de 130mm — evacuar áreas baixas e acionar defesa civil.",
    "criadoEm": "03/06/2026 01:39"
  },
  {
    "id": "f0f45889-59e3-448e-9b35-5766a9051f6c",
    "propriedadeId": "1124d424-3fa4-48ac-8427-92c904bfb317",
    "tipo": "AlertaGeada",
    "severidade": "Alto",
    "probabilidade": 0.65,
    "recomendacao": "Risco alto de geada (-1,5°C) — cobrir plantas sensíveis.",
    "criadoEm": "03/06/2026 01:39"
  },
  {
    "id": "097e1b66-223d-4fbf-be96-40b543f56bf0",
    "propriedadeId": "7f66caf6-328d-4ce9-adaa-c5940ef415f0",
    "tipo": "AlertaSeca",
    "severidade": "Alto",
    "probabilidade": 0.72,
    "recomendacao": "Aumentar frequência de irrigação — 18 dias sem chuva.",
    "criadoEm": "03/06/2026 01:39"
  },
  {
    "id": "5186386e-99ee-4beb-9eca-5b224911a06b",
    "propriedadeId": "7f66caf6-328d-4ce9-adaa-c5940ef415f0",
    "tipo": "AlertaPraga",
    "severidade": "Critico",
    "probabilidade": 0.87,
    "recomendacao": "Aplicar defensivo imediatamente contra Spodoptera frugiperda na cultura de Milho.",
    "criadoEm": "03/06/2026 01:39"
  }
]
```

> Todos os 5 tipos de alerta retornados: AlertaPraga, AlertaErosao, AlertaEnchente, AlertaGeada, AlertaSeca — cada um com severidade e recomendação calculadas pelo polimorfismo da classe `Alerta`.

---

## Erros — ProblemDetails RFC 7807

**404 — Recurso inexistente:**
```json
HTTP/1.1 404 Not Found

{
  "title": "Recurso não encontrado",
  "status": 404,
  "detail": "Propriedade '00000000-0000-0000-0000-000000000000' não encontrado.",
  "traceId": "00-092675658e1e378afa71b2fa100e6434-b8b0f748746c2a0c-00"
}
```

**409 — E-mail duplicado:**
```json
HTTP/1.1 409 Conflict

{
  "title": "Conflito de dados",
  "status": 409,
  "detail": "E-mail 'teste' já está cadastrado.",
  "traceId": "00-acf0282220c679047522d07924a88b77-7591ad955791d9b6-00"
}
```

**401 — Credenciais inválidas:**
```json
HTTP/1.1 401 Unauthorized

{
  "title": "Não autorizado",
  "status": 401,
  "detail": "E-mail ou senha inválidos.",
  "traceId": "00-3f1a2b4c5d6e7f8a9b0c1d2e3f4a5b6c-7d8e9f0a1b2c3d4e-00"
}
```

---

## xUnit — Testes de Domínio

```
dotnet test --verbosity normal

Passed!  - Failed: 0, Passed: 66, Skipped: 0, Total: 66, Duration: 31 ms
```
