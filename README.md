# FiapAgro — Backend

> API REST .NET 8 para monitoramento agroclimático — FIAP Global Solution 2026.1

[![build](https://img.shields.io/badge/build-passing-brightgreen)](https://github.com/GS-SPACE-CONNECT/2f-agro-backend/actions)
[![Hub](https://img.shields.io/badge/hub-2f--agro-success)](https://github.com/GS-SPACE-CONNECT/2f-agro)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple)](https://dotnet.microsoft.com)

---

## Sobre o Projeto

O **FiapAgro Backend** é uma Web API que detecta e registra alertas agroclimáticos (pragas, secas, geadas, enchentes e erosão) para propriedades rurais cadastradas. Desenvolvido em C# .NET 8, atende às rubricas de **C# (100 pts)** e **SOA** da FIAP 3ES — GS 2026.1.

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

| # | Item | Pts | Implementação |
|---|------|:---:|---|
| 4 | Classes abstratas + herança + polimorfismo | 20 | `Alerta` (abstract) → 5 herdeiros; `CalcularSeveridade()` e `Recomendacao()` sobrescritos em cada subclasse |
| 5 | Interfaces + injeção de dependência | 20 | `IAlertaRepository`, `IPropriedadeRepository`, `IUsuarioRepository`, `IDetector<T>`, `INotificador` — todos Scoped via `AddFiapAgroServices()` |
| 6 | Structs + Partial Classes | 5 | `readonly struct Coordenada` (GPS, value-semantics, IEquatable); `partial class Propriedade` (estado / comportamento em arquivos separados) |
| 7 | EF Core + migrations + PostgreSQL | 20 | TPH para `Alerta`, `ComplexProperty` para `Coordenada`, 2 migrations, seed automático na startup |
| 8 | Controllers + endpoints REST | 10 | 3 controllers · 15 endpoints · DTOs tipados (records) · `CreatedAtAction` |
| 9 | Tratamento de exceções | 10 | Hierarquia `DomainException` → `GlobalExceptionHandler` (`IExceptionHandler` .NET 8) → RFC 7807 ProblemDetails com `traceId` |
| 10 | Auth + JWT | 15 | `JwtService` (PBKDF2 + HMAC-SHA256) · `AuthController` · `[Authorize]` · Swagger Bearer |
| 11 | README + diagrama + evidências | 30 | Este arquivo + Mermaid (camadas · classes · fluxo · ER) + `docs/evidencias/` |
| | **Total** | **130** | |

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
- [PostgreSQL 15+](https://www.postgresql.org/) **ou** Docker

### PostgreSQL com Docker

```bash
docker run --name fiapagro-pg \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=fiapagro \
  -p 5432:5432 -d postgres:15
```

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=fiapagro;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "chave_secreta_com_minimo_32_caracteres_aqui!",
    "Issuer": "FiapAgro.Api",
    "Audience": "FiapAgro.Clients",
    "ExpiresHours": "8"
  }
}
```

### Executar

```bash
# 1. Restaurar pacotes
dotnet restore

# 2. Aplicar migrations (cria tabelas + seed automático na startup)
dotnet ef database update --project FiapAgro.Infrastructure --startup-project FiapAgro.Api

# 3. Subir a API
dotnet run --project FiapAgro.Api
```

A API sobe em `http://localhost:5000` · Swagger em `http://localhost:5000/swagger`.

### Testes unitários

```bash
dotnet test --verbosity normal
```

---

## Evidências de Execução

Exemplos completos de requisições HTTP em [`docs/evidencias/requests.http`](docs/evidencias/requests.http) (VS Code REST Client).

**Como usar:**
1. Instale a extensão [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) no VS Code
2. Suba a API: `dotnet run --project FiapAgro.Api`
3. Abra `docs/evidencias/requests.http` → clique em **Send Request** em cada bloco

**Fluxo de teste recomendado:**

```
1. POST /api/auth/registrar  → copiar o token
2. POST /api/auth/login      → confirmar autenticação
3. POST /api/propriedades    → criar propriedade (usar o token)
4. POST /api/alertas/praga   → criar alerta
5. GET  /api/alertas/recentes → listar alertas
```

O Swagger (`/swagger`) também serve como evidência interativa — autentique com o token JWT no botão **Authorize 🔒** e execute os endpoints diretamente no browser.

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
└── docs/evidencias/        # requests.http + exemplos de resposta
```
