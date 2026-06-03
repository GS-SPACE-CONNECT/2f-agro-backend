# FiapAgro — Backend

> API REST .NET 8 para monitoramento agroclimático — FIAP Global Solution 2026.1

[![build](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/GS-SPACE-CONNECT/2f-agro-backend/actions)
[![Hub](https://img.shields.io/badge/hub-2f--agro-success)](https://github.com/GS-SPACE-CONNECT/2f-agro)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com)

---

## Sobre o Projeto

O agronegócio brasileiro responde por mais de 25% do PIB nacional, mas ainda é altamente vulnerável a eventos climáticos extremos — pragas, secas, geadas, enchentes e erosão de solo causam bilhões em perdas a cada safra.

O **FiapAgro Backend** é uma Web API que detecta e registra alertas agroclimáticos em tempo real para propriedades rurais cadastradas. O produtor recebe recomendações precisas por tipo de risco (praga, seca, geada, enchente ou erosão), com nível de severidade calculado automaticamente, permitindo ação imediata antes que o dano se torne irreversível.

O projeto integra o tema da **Global Solution 2026.1 — Space Connect**: dados agroclimáticos provenientes de sensores e satélites alimentam os detectores da API, que classificam automaticamente o risco e geram alertas com recomendações. Desenvolvido em C# .NET 8, atende às rubricas de **C# (100 pts)** e **SOA** da FIAP 3ES — GS 2026.1.

## Equipe

| GitHub | Nome |
|--------|------|
| [@brnleao](https://github.com/brnleao) | Bruno Leão |
| [@DevRuanVieira](https://github.com/DevRuanVieira) | Ruan Vieira |
| [@jota0802](https://github.com/jota0802) | José Otávio |

---

## Stack Técnica

| Componente | Tecnologia |
|---|---|
| Runtime | .NET 8 (C# 12) |
| Web Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 |
| Banco de Dados | PostgreSQL 15 + Npgsql 8 |
| Autenticação | JWT Bearer — HMAC-SHA256 |
| Hash de senha | PBKDF2/SHA-256 (`Rfc2898DeriveBytes`, 10 000 iterações) |
| Documentação | Swagger / OpenAPI (Swashbuckle 6) |
| Testes | xUnit 2.5 |
| Exceções | `IExceptionHandler` .NET 8 + RFC 7807 ProblemDetails |

---

## Arquitetura

O projeto segue a **Arquitetura em Camadas** com dependências unidirecionais:

```
FiapAgro.Api  →  FiapAgro.Domain  ←  FiapAgro.Infrastructure
```

### Diagrama de Camadas

```mermaid
graph TB
    subgraph CLI["Cliente"]
        SW["Swagger UI / HTTP Client"]
    end

    subgraph API["FiapAgro.Api"]
        CTRL["Controllers\nAuth · Propriedades · Alertas"]
        MW["GlobalExceptionHandler\nIExceptionHandler — RFC 7807"]
        JWTMW["JWT Bearer Middleware"]
    end

    subgraph DOM["FiapAgro.Domain"]
        ENT["Entities\nAlerta abstract · Propriedade · Usuario"]
        IFACE["Interfaces\nIAlertaRepo · IPropriedadeRepo · IUsuarioRepo · IDetector‹T›"]
        EXC["Exceptions\nDomainException → NaoEncontrado · Conflito · RegraDeNegocio"]
        VO["ValueObjects\nCoordenada readonly struct"]
    end

    subgraph INF["FiapAgro.Infrastructure"]
        REPO["Repositories EF Core\nAlerta · Propriedade · Usuario"]
        JWTSS["JwtService\nPBKDF2 + HMAC-SHA256"]
        DET["Detectors\nPraga · Seca · Geada · Enchente · Erosao"]
        DB[("PostgreSQL\nfiapagro")]
    end

    CLI --> API
    API --> DOM
    API --> INF
    INF --> DOM
    REPO --> DB
```

---

### Diagrama de Classes — Hierarquia de Alertas

```mermaid
classDiagram
    direction TB

    class Alerta {
        <<abstract>>
        +Guid Id
        +DateTime CriadoEm
        +Guid PropriedadeId
        +double Probabilidade
        +int TotalCriados$
        +CalcularSeveridade()* NivelSeveridade
        +Recomendacao()* string
        +FormatarData(DateTime)$ string
    }
    class AlertaPraga {
        +string EspeciePraga
        +string CulturaAfetada
        +CalcularSeveridade() NivelSeveridade
        +Recomendacao() string
    }
    class AlertaSeca {
        +int DiasSemChuva
        +CalcularSeveridade() NivelSeveridade
        +Recomendacao() string
    }
    class AlertaGeada {
        +double TemperaturaMinima
        +CalcularSeveridade() NivelSeveridade
        +Recomendacao() string
    }
    class AlertaEnchente {
        +double VolumeMM
        +CalcularSeveridade() NivelSeveridade
        +Recomendacao() string
    }
    class AlertaErosao {
        +double InclinacaoSolo
        +CalcularSeveridade() NivelSeveridade
        +Recomendacao() string
    }
    class NivelSeveridade {
        <<enumeration>>
        Baixo
        Medio
        Alto
        Critico
    }

    Alerta <|-- AlertaPraga   : herda
    Alerta <|-- AlertaSeca    : herda
    Alerta <|-- AlertaGeada   : herda
    Alerta <|-- AlertaEnchente : herda
    Alerta <|-- AlertaErosao   : herda
    Alerta ..> NivelSeveridade  : usa
```

---

### Diagrama de Fluxo — Ciclo de Autenticação

```mermaid
sequenceDiagram
    actor C as Cliente
    participant AC as AuthController
    participant UR as IUsuarioRepository
    participant DB as PostgreSQL
    participant JS as JwtService

    C->>AC: POST /api/auth/registrar
    AC->>UR: ExisteEmailAsync(email)
    UR->>DB: SELECT FROM usuarios WHERE email = ?
    DB-->>UR: 0 linhas
    UR-->>AC: false
    AC->>JS: HashSenha(senha)
    JS-->>AC: salt:hash  (PBKDF2/SHA-256)
    AC->>UR: AdicionarAsync(usuario)
    UR->>DB: INSERT INTO usuarios
    DB-->>UR: OK
    AC->>JS: GerarToken(usuario)
    JS-->>AC: JWT assinado (HMAC-SHA256, 8 h)
    AC-->>C: 201 Created — TokenResponse

    Note over C,AC: Login segue o mesmo fluxo com VerificarSenha()
```

---

### Diagrama de Fluxo — Tratamento de Exceções

```mermaid
flowchart TD
    REQ([HTTP Request]) --> CTRL[Controller Action]
    CTRL -->|sucesso| RES([2xx Response])
    CTRL -->|NaoEncontradoException| GEH[GlobalExceptionHandler]
    CTRL -->|ConflitoException| GEH
    CTRL -->|RegraDeNegocioException| GEH
    CTRL -->|Exception não tratada| GEH

    GEH --> LOG{Tipo?}
    LOG -->|DomainException| WARN[log.Warning]
    LOG -->|Exception genérica| ERR["log.Error + stack trace"]

    WARN --> PD["ProblemDetails RFC 7807\nstatus · title · detail · traceId"]
    ERR  --> PD

    PD -->|404| R404([404 Not Found])
    PD -->|409| R409([409 Conflict])
    PD -->|400| R400([400 Bad Request])
    PD -->|500| R500([500 Internal Server Error])
```

---

### Diagrama ER — Banco de Dados

```mermaid
erDiagram
    usuarios {
        uuid Id PK
        varchar Nome
        varchar Email UK
        text SenhaHash
        timestamptz CriadoEm
    }
    propriedades {
        uuid Id PK
        varchar Nome
        varchar Municipio
        char Estado
        float8 AreaHectares
        uuid UsuarioId
        float8 localizacao_lat
        float8 localizacao_lng
        timestamptz CriadoEm
    }
    alertas {
        uuid Id PK
        varchar tipo_alerta
        uuid PropriedadeId FK
        float8 Probabilidade
        timestamptz CriadoEm
        varchar EspeciePraga
        varchar CulturaAfetada
        int DiasSemChuva
        float8 TemperaturaMinima
        float8 VolumeMM
        float8 InclinacaoSolo
    }
    propriedades ||--o{ alertas : "tem alertas"
```

> `alertas` usa **Table-Per-Hierarchy (TPH)**: uma única tabela com coluna discriminadora `tipo_alerta`.

---

## Rubrica C# — Checklist

| Item | Pts | Implementação |
|------|:---:|---|
| Modelagem de Domínio & POO — herança, polimorfismo, classes públicas/privadas/estáticas | 20 | `Alerta` (abstract) → 5 herdeiros; `CalcularSeveridade()` e `Recomendacao()` sobrescritos; `_totalCriados` (private static) · `TotalCriados` (public static) |
| Abstração e Interfaces + injeção de dependência | 20 | `IAlertaRepository`, `IPropriedadeRepository`, `IUsuarioRepository`, `IDetector<T>`, `INotificador` — todos Scoped via `AddFiapAgroServices()` |
| Lógica de Fluxo, Métodos e Datas | 15 | Métodos modulares por detector (`DetectorPraga`, `DetectorSeca` etc.); `DateTime.UtcNow` em `Alerta`; `FormatarData(DateTime)` estático; `Math.Clamp` para probabilidade; `ToString()` sobrescrito |
| Tratamento de Exceções | 10 | Hierarquia `DomainException` → `GlobalExceptionHandler` (`IExceptionHandler` .NET 8) → RFC 7807 ProblemDetails com `traceId` |
| Structs + Partial Classes | 5 | `readonly struct Coordenada` (GPS, value-semantics, IEquatable); `partial class Propriedade` (estado / comportamento em arquivos separados) |
| Organização — estrutura de pastas, nomenclatura, README, diagrama, evidências | 30 | Este arquivo + Mermaid (camadas · classes · fluxo · ER) + [`docs/evidencias/`](docs/evidencias/) |
| | **100** | |

---

## Endpoints REST

### Auth (público)

| Método | Rota | Status | Descrição |
|--------|------|:------:|-----------|
| `POST` | `/api/auth/registrar` | 201 | Cadastra usuário e retorna JWT |
| `POST` | `/api/auth/login` | 200 | Autentica e retorna JWT |

### Propriedades (🔒 Bearer JWT)

| Método | Rota | Status | Descrição |
|--------|------|:------:|-----------|
| `GET` | `/api/propriedades?usuarioId={guid}` | 200 | Lista propriedades do usuário |
| `GET` | `/api/propriedades/{id}` | 200 / 404 | Busca por Id |
| `POST` | `/api/propriedades` | 201 | Cadastra nova propriedade |
| `PUT` | `/api/propriedades/{id}` | 200 / 404 | Atualiza dados |
| `DELETE` | `/api/propriedades/{id}` | 204 / 404 | Remove propriedade |

### Alertas (🔒 Bearer JWT)

| Método | Rota | Status | Descrição |
|--------|------|:------:|-----------|
| `GET` | `/api/alertas/recentes?quantidade=20` | 200 | Últimos N alertas |
| `GET` | `/api/alertas/propriedade/{id}` | 200 | Alertas de uma propriedade |
| `GET` | `/api/alertas/{id}` | 200 / 404 | Alerta por Id |
| `POST` | `/api/alertas/praga` | 201 | Registra alerta de praga |
| `POST` | `/api/alertas/seca` | 201 | Registra alerta de seca |
| `POST` | `/api/alertas/geada` | 201 | Registra alerta de geada |
| `POST` | `/api/alertas/enchente` | 201 | Registra alerta de enchente |
| `POST` | `/api/alertas/erosao` | 201 | Registra alerta de erosão |

Erros retornam **ProblemDetails (RFC 7807)**:

```json
{
  "status": 404,
  "title": "Recurso não encontrado",
  "detail": "Propriedade 'abc...' não encontrado.",
  "traceId": "00-abc123..."
}
```

---

## Configuração e Execução

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- PostgreSQL 15+ (veja opções abaixo)

### 1. Banco de dados — escolha uma opção

**Docker (recomendado):**
```bash
docker run --name fiapagro-pg \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=fiapagro \
  -p 5432:5432 -d postgres:15
```

**PostgreSQL local já instalado** — crie o banco:
```bash
createdb -U postgres fiapagro
```

A connection string padrão em `appsettings.json` já está configurada para `localhost:5432` com usuário/senha `postgres`:
```
Host=localhost;Port=5432;Database=fiapagro;Username=postgres;Password=postgres
```
Ajuste o arquivo se seu PostgreSQL usar credenciais diferentes.

### 2. Restaurar pacotes

```bash
dotnet restore
```

### 3. Subir a API

```bash
dotnet run --project FiapAgro.Api
```

> As migrations e o seed de dados rodam **automaticamente** na startup — não é necessário rodar `dotnet ef database update` manualmente.

A API inicia em `http://localhost:5050` · Swagger abre automaticamente em `http://localhost:5050/swagger`.

### 4. Testar via Swagger

1. Acesse `http://localhost:5050/swagger`
2. Chame `POST /api/auth/registrar` para criar um usuário — copie o `token` e o `sub` (id do usuário) da resposta
3. Clique em **Authorize 🔒** no topo e cole o token
4. Use `POST /api/propriedades` com o `sub` como `usuarioId` para cadastrar uma propriedade
5. Use `POST /api/alertas/praga` (ou outro tipo) com o `id` da propriedade
6. Verifique com `GET /api/alertas/recentes?quantidade=10`

### 5. Testes unitários

```bash
dotnet test --verbosity normal
```

### (Opcional) Gerenciar migrations manualmente

```bash
# Instalar ferramenta global (uma vez)
dotnet tool install --global dotnet-ef --version 8.0.11

# Aplicar migrations sem subir a API
dotnet ef database update --project FiapAgro.Infrastructure --startup-project FiapAgro.Api

# Criar nova migration após alterar o modelo
dotnet ef migrations add NomeDaMigration --project FiapAgro.Infrastructure --startup-project FiapAgro.Api
```

---

## Evidências de Execução

Saídas reais capturadas com a API rodando em `http://localhost:5050` contra PostgreSQL 15 local.

- Arquivo de requisições completo: [`docs/evidencias/requests.http`](docs/evidencias/requests.http)
- Saídas reais de execução: [`docs/evidencias/saidas_reais.md`](docs/evidencias/saidas_reais.md)

---

### Console — Startup + Migrations + Seed

```
info: Microsoft.EntityFrameworkCore.Migrations[20405]
      No migrations were applied. The database is already up to date.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [...] SELECT EXISTS (SELECT 1 FROM propriedades AS p)
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (10ms) [...] INSERT INTO propriedades (...) VALUES (...);  -- seed: 2 propriedades
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [...] INSERT INTO alertas (...) VALUES (...);        -- seed: 5 alertas
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5050
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\...\FiapAgro.Api
```

---

### Migrations do zero (`dotnet ef database update`)

```
Build started...
Build succeeded.
Applying migration '20260601230202_Initial'.
Applying migration '20260602002805_AddUsuario'.
Done.
```

---

### POST /api/auth/registrar → 201 Created

```http
POST http://localhost:5050/api/auth/registrar
Content-Type: application/json

{ "nome": "Bruno Leao", "email": "bruno@fiapagro.com", "senha": "Senha@123" }
```

```json
HTTP/1.1 201 Created

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1ZWRmYmE1...",
  "nome": "Bruno Leao",
  "email": "bruno@fiapagro.com",
  "expira": "2026-06-03T10:52:14Z"
}
```

---

### POST /api/auth/login → 200 OK

```json
HTTP/1.1 200 OK

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI1ZWRmYmE1...",
  "nome": "Bruno Leao",
  "email": "bruno@fiapagro.com",
  "expira": "2026-06-03T10:52:28Z"
}
```

---

### POST /api/propriedades → 201 Created

```json
HTTP/1.1 201 Created

{
  "id": "68a9c102-0951-41c0-93b3-f0c8e94621ad",
  "nome": "Fazenda Boa Vista",
  "municipio": "Ribeirao Preto",
  "estado": "SP",
  "areaHectares": 250.5,
  "usuarioId": "00000000-0000-0000-0000-000000000001",
  "descricao": "Fazenda Boa Vista — Ribeirao Preto/SP (250,5 ha) @ (-21,170400, -47,810300)",
  "criadoEm": "2026-06-03T02:52:33Z"
}
```

---

### POST /api/alertas/praga → 201 Created (severidade Critico)

```json
HTTP/1.1 201 Created

{
  "id": "008016cd-6b5b-4a65-891b-2f15230a377c",
  "propriedadeId": "68a9c102-0951-41c0-93b3-f0c8e94621ad",
  "tipo": "AlertaPraga",
  "severidade": "Critico",
  "probabilidade": 0.87,
  "recomendacao": "Aplicar defensivo imediatamente contra lagarta-do-cartucho na cultura de milho.",
  "criadoEm": "03/06/2026 02:03"
}
```

---

### GET /api/alertas/recentes → 200 OK (todos os 5 tipos)

```json
HTTP/1.1 200 OK

[
  { "tipo": "AlertaPraga",    "severidade": "Critico", "probabilidade": 0.87, "recomendacao": "Aplicar defensivo imediatamente contra lagarta-do-cartucho na cultura de milho." },
  { "tipo": "AlertaErosao",   "severidade": "Medio",   "probabilidade": 0.55, "recomendacao": "Inclinação de 22,0° — monitorar erosão e manter cobertura do solo." },
  { "tipo": "AlertaEnchente", "severidade": "Critico", "probabilidade": 0.91, "recomendacao": "Volume crítico de 130mm — evacuar áreas baixas e acionar defesa civil." },
  { "tipo": "AlertaGeada",    "severidade": "Alto",    "probabilidade": 0.65, "recomendacao": "Risco alto de geada (-1,5°C) — cobrir plantas sensíveis." },
  { "tipo": "AlertaSeca",     "severidade": "Alto",    "probabilidade": 0.72, "recomendacao": "Aumentar frequência de irrigação — 18 dias sem chuva." }
]
```

> Resposta completa em [`docs/evidencias/saidas_reais.md`](docs/evidencias/saidas_reais.md).

---

### Erros — ProblemDetails RFC 7807

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
  "detail": "E-mail 'bruno@fiapagro.com' já está cadastrado.",
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

### Swagger UI

Disponível em `http://localhost:5050/swagger` — use o botão **Authorize 🔒** para inserir o JWT e testar os endpoints protegidos.

---

### xUnit — Testes de domínio

```
dotnet test --verbosity normal

Passed!  - Failed: 0, Passed: 66, Skipped: 0, Total: 66, Duration: 31 ms
```

---

## Screenshots — Swagger UI

> Prints capturados com a API rodando em `http://localhost:5050/swagger`.  
> Arquivos salvos em `docs/evidencias/screenshots/`.

---

### 1. Swagger UI — Visão Geral

![Swagger UI - visão geral](docs/evidencias/screenshots/01-swagger-geral.png)

---

### 2. POST /api/auth/registrar → 201 Created

![Registrar usuário - 201 Created](docs/evidencias/screenshots/02-auth-registrar.png)

---

### 3. Authorize — Bearer Token

![Authorize com JWT](docs/evidencias/screenshots/03-authorize-token.png)

---

### 4. POST /api/propriedades → 201 Created

![Criar propriedade - 201 Created](docs/evidencias/screenshots/04-propriedades-criar.png)

---

### 5. POST /api/alertas/praga → 201 Created (Critico)

![Alerta praga - 201 Created](docs/evidencias/screenshots/05-alertas-praga.png)

---

### 6. GET /api/alertas/recentes → 200 OK

![Alertas recentes - 200 OK](docs/evidencias/screenshots/06-alertas-recentes.png)

---

### 7. GET /api/propriedades/{id} inexistente → 404 ProblemDetails

![404 ProblemDetails](docs/evidencias/screenshots/07-erro-404.png)

---

### 8. POST /api/auth/registrar e-mail duplicado → 409 ProblemDetails

![409 Conflict ProblemDetails](docs/evidencias/screenshots/08-erro-409.png)

---

### 9. POST /api/auth/login credenciais inválidas → 401 ProblemDetails

![401 Unauthorized ProblemDetails](docs/evidencias/screenshots/09-erro-401.png)

---

### 10. xUnit — Testes Passando

![xUnit 66 testes passando](docs/evidencias/screenshots/10-testes-xunit.png)

---

## Estrutura de Pastas

```
2f-agro-backend/
├── FiapAgro.Api/
│   ├── Controllers/        # AuthController · PropriedadesController · AlertasController
│   ├── Dtos/               # Records tipados para request/response
│   ├── Middleware/         # GlobalExceptionHandler (IExceptionHandler)
│   └── Program.cs          # Pipeline, DI, JWT, Swagger, migrations
├── FiapAgro.Domain/
│   ├── Entities/           # Alerta (abstract) + 5 herdeiros · Propriedade (partial) · Usuario
│   ├── Exceptions/         # DomainException · NaoEncontrado · Conflito · RegraDeNegocio
│   ├── Interfaces/         # IAlertaRepository · IPropriedadeRepository · IUsuarioRepository · IDetector‹T›
│   └── ValueObjects/       # Coordenada (readonly struct)
├── FiapAgro.Infrastructure/
│   ├── Auth/               # JwtService (PBKDF2 + JWT)
│   ├── Data/               # AppDbContext · Migrations · Seed
│   ├── Detectors/          # DetectorPraga · Seca · Geada · Enchente · Erosao
│   ├── Repositories/       # Implementações EF Core dos repositórios
│   └── Extensions/         # ServiceCollectionExtensions (AddFiapAgroServices)
├── FiapAgro.Tests/         # xUnit — testes de domínio
└── docs/evidencias/        # requests.http · saidas_reais.md
```
