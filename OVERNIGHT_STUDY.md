# OVERNIGHT_STUDY — feat/lavoura-diagnostico-mobile

> **Branch:** `feat/lavoura-diagnostico-mobile`
> **Repo:** `2f-agro-backend` (worktree isolado)
> **Data:** 02/06/2026
> **Autor do estudo:** Claude (fase de análise)

---

## 1. Contexto e objetivo

O app mobile (`2f-agro-mobile`) já possui telas funcionais com **mock data** para:

- **Lavouras** (talhões/culturas dentro de uma propriedade) — listagem na Home, tab dedicada, tela de detalhe placeholder
- **Diagnóstico de Praga** ("Olho na Folha") — câmera captura foto → resultado com praga, confiança, recomendação

O backend C# (`2f-agro-backend`) **não possui** nenhuma dessas entidades. Hoje só existem:
- `Propriedade` (CRUD completo)
- `Usuario` (auth JWT)
- `Alerta` (5 subtipos: Praga, Seca, Geada, Enchente, Erosão)

**Esta feature preenche essa lacuna**: criar as entidades `Lavoura` e `DiagnosticoPraga` no backend com endpoints REST que espelham exatamente os tipos que o mobile espera, permitindo que o Sprint 2 do mobile substitua os mocks por chamadas HTTP reais.

---

## 2. Análise do contrato mobile (fonte da verdade)

### 2.1 Tipo `Lavoura` (mobile `lib/types.ts:46-60`)

```typescript
interface Lavoura {
  id: string;
  propriedadeId: string;
  cultura: CulturaTipo;          // "milho" | "tomate" | "alface" | "feijao" | "mandioca" | "soja" | "cana"
  culturaLabel: string;          // "Milho", "Tomate", etc.
  identificador: string;         // "L1", "L2" (label local do agricultor)
  areaHectares: number;
  saude: LavouraSaudeKey;        // "saudavel" | "atencao" | "risco" | "perdida"
  ndviAtual?: number;            // -1..1 (Normalized Difference Vegetation Index)
  ultimaLeitura?: string;        // ISO timestamp
  coordenadas?: { lat: number; lng: number };
}
```

### 2.2 Tipo `DiagnosticoPraga` (mobile `lib/types.ts:81-99`)

```typescript
interface DiagnosticoPraga {
  id: string;
  lavouraId?: string;            // opcional — foto pode ser "solta"
  fotoUri: string;               // URI da imagem
  praga: PragaTipo;              // slug: "sadia" | "ferrugem_asiatica" | "lagarta_do_cartucho" | ...
  pragaLabel: string;            // "Ferrugem Asiática"
  confianca: number;             // 0..1
  severidade: AlertaSeveridadeKey; // "baixo" | "medio" | "alto" | "critico"
  recomendacao: string;          // max ~120 chars
  agronomoTelefone: string;      // E.164
  criadoEm: string;             // ISO timestamp
}
```

### 2.3 Endpoints que o mobile espera (`lib/api.ts`)

| Método mobile            | Verbo + Rota esperada                     | Corpo                    |
|--------------------------|-------------------------------------------|--------------------------|
| `api.listLavouras()`     | `GET /api/lavouras?propriedadeId={guid}`  | —                        |
| `api.getLavoura(id)`      | `GET /api/lavouras/{id}`                  | —                        |
| `api.diagnosticarFolha()` | `POST /api/diagnosticos`                  | multipart (foto + lavouraId?) |
| *(CRUD implícito)*       | `POST /api/lavouras`                      | JSON body                |
| *(CRUD implícito)*       | `PUT /api/lavouras/{id}`                  | JSON body                |
| *(CRUD implícito)*       | `DELETE /api/lavouras/{id}`               | —                        |

Nota: o spec (§ 4.5) lista `DiagnosticoController.cs` na estrutura de pastas planejada.

---

## 3. Estado atual do backend (gap analysis)

### 3.1 O que JÁ existe e pode ser reutilizado

| Artefato | Localização | Relevância |
|----------|-------------|------------|
| `Propriedade` entity | `Domain/Entities/Propriedade.cs` | FK pai de Lavoura |
| `Coordenada` value object | `Domain/ValueObjects/Coordenada.cs` | Reutilizar para `Lavoura.Coordenadas` |
| `NivelSeveridade` enum | `Domain/Enums/NivelSeveridade.cs` | Mapear para severidade do diagnóstico |
| Padrão Repository (interface + EF) | `Domain/Interfaces/` + `Infrastructure/Repositories/` | Seguir para ILavouraRepository e IDiagnosticoRepository |
| Padrão DTO (record request/response + `FromEntity`) | `Api/Dtos/` | Seguir para LavouraDto e DiagnosticoDto |
| Padrão Controller ([Authorize], ILogger, throw NaoEncontradoException) | `Api/Controllers/` | Seguir para LavourasController e DiagnosticosController |
| DI registration | `Infrastructure/Extensions/ServiceCollectionExtensions.cs` | Adicionar novos repos/services |
| AppDbContext + migrations | `Infrastructure/Data/` | Adicionar DbSets + configuração + migration |
| Seed data | `Infrastructure/Data/AppDbContextSeed.cs` | Adicionar lavouras e diagnósticos de exemplo |
| Exception hierarchy | `Domain/Exceptions/` | Reutilizar NaoEncontradoException, RegraDeNegocioException |

### 3.2 O que NÃO existe (a ser criado)

| Artefato | Tipo | Camada |
|----------|------|--------|
| `Lavoura` | Entity | Domain |
| `DiagnosticoPraga` | Entity | Domain |
| `CulturaTipo` | Enum | Domain |
| `SaudeLavoura` | Enum | Domain |
| `PragaTipo` | Enum | Domain |
| `ILavouraRepository` | Interface | Domain |
| `IDiagnosticoRepository` | Interface | Domain |
| `LavouraRepositoryEF` | Repository | Infrastructure |
| `DiagnosticoRepositoryEF` | Repository | Infrastructure |
| `LavourasController` | Controller | Api |
| `DiagnosticosController` | Controller | Api |
| `LavouraDtos` | DTOs (Request + Response) | Api |
| `DiagnosticoDtos` | DTOs (Request + Response) | Api |
| Migration `AddLavouraDiagnostico` | Migration | Infrastructure |
| `LavouraTests` | Testes | Tests |
| `DiagnosticoTests` | Testes | Tests |

---

## 4. Decisões de design

### 4.1 Lavoura — modelagem

```csharp
public class Lavoura
{
    public Guid Id { get; private set; }
    public Guid PropriedadeId { get; set; }       // FK → Propriedade
    public CulturaTipo Cultura { get; set; }       // enum
    public string Identificador { get; set; }      // "L1", "L2" — label local
    public double AreaHectares { get; set; }
    public SaudeLavoura Saude { get; set; }        // enum: Saudavel, Atencao, Risco, Perdida
    public double? NdviAtual { get; set; }         // -1..1, nullable
    public DateTime? UltimaLeitura { get; set; }   // nullable
    public Coordenada Coordenadas { get; set; }    // value object reaproveitado
    public DateTime CriadoEm { get; private set; }
}
```

**Justificativas:**
- `CulturaTipo` como enum C# (não string livre) — valida no domínio, impede lixo no banco. Os 7 valores batem com o mobile: `Milho, Tomate, Alface, Feijao, Mandioca, Soja, Cana`.
- `SaudeLavoura` como enum: `Saudavel, Atencao, Risco, Perdida` — 4 estados, mapeiam 1:1 com `LavouraSaudeKey` do mobile.
- `Coordenada` (value object existente) reutilizado para localização do talhão.
- `Identificador` como string livre porque o agricultor define ("L1", "Milharal da beira do rio", etc.).
- `NdviAtual` e `UltimaLeitura` são nullable — Sprint 2 feature, dados vêm de satélite.

**Label `culturaLabel`:** Não armazenar no banco. O DTO response resolve via `Cultura.ToString()` ou um helper `CulturaLabel()`. O mobile recebe o label já formatado no JSON, sem precisar mapear localmente.

### 4.2 DiagnosticoPraga — modelagem

```csharp
public class DiagnosticoPraga
{
    public Guid Id { get; private set; }
    public Guid? LavouraId { get; set; }           // FK nullable → Lavoura
    public string FotoUrl { get; set; }            // URL/path no servidor (não file://)
    public PragaTipo Praga { get; set; }           // enum
    public double Confianca { get; set; }          // 0..1
    public NivelSeveridade Severidade { get; set; } // reusa enum existente
    public string Recomendacao { get; set; }       // texto curto
    public string AgronomoTelefone { get; set; }   // E.164
    public DateTime CriadoEm { get; private set; }
}
```

**Justificativas:**
- `PragaTipo` como enum com os 8 valores do mobile: `Sadia, FerrugemAsiatica, LagartaDoCartucho, ManchaFoliar, Oidio, MoscaBranca, BrocaDoCafe, Antracnose`.
- `Severidade` reutiliza `NivelSeveridade` existente (Baixo/Medio/Alto/Critico).
- `LavouraId` nullable — o agricultor pode tirar foto "solta" sem associar a uma lavoura.
- `FotoUrl` armazena o path/URL do servidor (o upload salva a imagem em disco ou blob; o banco guarda o caminho).
- `AgronomoTelefone` — Sprint 1 pode ser um valor fixo/configurável. Sprint 2 pode vir do perfil da cooperativa.

### 4.3 Endpoint POST /api/diagnosticos — Sprint 1 (mock inference)

Para Sprint 1, o backend **não terá** o modelo YOLO/ONNX integrado. O endpoint deve:

1. Receber upload da foto (multipart/form-data) + `lavouraId` opcional
2. Salvar a foto em disco (pasta `wwwroot/uploads/diagnosticos/`)
3. **Simular inferência** — retornar um diagnóstico mock (similar ao que o mobile já faz), ou aceitar os campos `praga`, `confianca`, `severidade` como parâmetros opcionais no body (permitindo que o script Python do IoT envie resultados reais via API)
4. Persistir o `DiagnosticoPraga` no banco
5. Retornar o DTO response

**Decisão arquitetural:** Aceitar TANTO upload puro (backend simula) QUANTO resultado pré-computado (IoT/Python envia). Isso permite:
- Mobile Sprint 1: envia foto → backend simula → retorna mock
- IoT/CV Sprint 2: Python processa YOLO → envia resultado pré-computado via API → backend persiste

### 4.4 Enums — serialização JSON

Os enums no DTO response precisam sair como **string lowercase** para bater com o mobile:

| C# Enum | JSON | Mobile key |
|---------|------|------------|
| `CulturaTipo.Milho` | `"milho"` | `CulturaTipo = "milho"` |
| `SaudeLavoura.Saudavel` | `"saudavel"` | `LavouraSaudeKey = "saudavel"` |
| `PragaTipo.FerrugemAsiatica` | `"ferrugem_asiatica"` | `PragaTipo = "ferrugem_asiatica"` |
| `NivelSeveridade.Critico` | `"critico"` | `AlertaSeveridadeKey = "critico"` |

**Implementação:** Usar `JsonStringEnumConverter` com `JsonNamingPolicy.SnakeCaseLower` nos DTOs, ou mapear manualmente no `FromEntity()` com `.ToString().ToLowerInvariant()` / helper. A abordagem mais simples e consistente é mapear no DTO response (mesmo padrão que `AlertaResponse.FromEntity` já faz com `a.CalcularSeveridade().ToString()`).

### 4.5 Relação Alerta ↔ Lavoura

O tipo `Alerta` do mobile tem `lavouraId?: string` (opcional). O backend `Alerta` atual só tem `PropriedadeId`. Duas opções:

- **Opção A (mínima):** Não alterar Alerta agora. O mobile já trata `lavouraId` como opcional.
- **Opção B (completa):** Adicionar `Guid? LavouraId` à entidade Alerta base + migration.

**Recomendação:** Opção A para esta feature. Manter escopo focado em Lavoura + Diagnóstico. Vincular alertas a lavouras pode ser uma issue separada.

---

## 5. Plano de implementação (ordem de commits)

### Commit 1: `feat(domain): entidades Lavoura e DiagnosticoPraga + enums`

**Arquivos novos — FiapAgro.Domain:**

| Arquivo | Conteúdo |
|---------|----------|
| `Enums/CulturaTipo.cs` | Enum com 7 valores: `Milho, Tomate, Alface, Feijao, Mandioca, Soja, Cana` |
| `Enums/SaudeLavoura.cs` | Enum com 4 valores: `Saudavel, Atencao, Risco, Perdida` |
| `Enums/PragaTipo.cs` | Enum com 8 valores: `Sadia, FerrugemAsiatica, LagartaDoCartucho, ManchaFoliar, Oidio, MoscaBranca, BrocaDoCafe, Antracnose` |
| `Entities/Lavoura.cs` | Entidade conforme § 4.1. Construtor privado (EF) + construtor público. `private set` em Id e CriadoEm. |
| `Entities/DiagnosticoPraga.cs` | Entidade conforme § 4.2. Mesmo padrão de construtores. `Confianca` com `Math.Clamp(0,1)`. |
| `Interfaces/ILavouraRepository.cs` | `BuscarPorIdAsync`, `ListarPorPropriedadeAsync`, `AdicionarAsync`, `AtualizarAsync`, `RemoverAsync` |
| `Interfaces/IDiagnosticoRepository.cs` | `BuscarPorIdAsync`, `ListarPorLavouraAsync`, `ListarRecentesAsync`, `AdicionarAsync` |

**Padrões a seguir (existentes):**
- Construtor privado sem parâmetros para EF Core (mesmo que `Propriedade`, `AlertaPraga`)
- `Id = Guid.NewGuid()` e `CriadoEm = DateTime.UtcNow` no construtor público
- Interface repository no Domain, implementação no Infrastructure

### Commit 2: `feat(infra): repositórios EF + migration + seed de Lavoura e Diagnóstico`

**Arquivos novos — FiapAgro.Infrastructure:**

| Arquivo | Conteúdo |
|---------|----------|
| `Repositories/LavouraRepositoryEF.cs` | Implementa `ILavouraRepository`. Segue padrão de `PropriedadeRepositoryEF`. `ListarPorPropriedadeAsync` ordena por `Identificador`. |
| `Repositories/DiagnosticoRepositoryEF.cs` | Implementa `IDiagnosticoRepository`. `ListarRecentesAsync` ordena por `CriadoEm DESC`. |

**Arquivos modificados — FiapAgro.Infrastructure:**

| Arquivo | Alteração |
|---------|-----------|
| `Data/AppDbContext.cs` | Adicionar `DbSet<Lavoura> Lavouras`, `DbSet<DiagnosticoPraga> Diagnosticos`. Adicionar `ConfigurarLavoura()` e `ConfigurarDiagnostico()` no `OnModelCreating`. |
| `Data/AppDbContextSeed.cs` | Adicionar 4-6 lavouras para as propriedades existentes (Fazenda ABC e Sítio Verde) + 1-2 diagnósticos de exemplo. |
| `Extensions/ServiceCollectionExtensions.cs` | Registrar `ILavouraRepository → LavouraRepositoryEF` e `IDiagnosticoRepository → DiagnosticoRepositoryEF` como Scoped. |

**Migration:** gerar com `dotnet ef migrations add AddLavouraDiagnostico`

**Configuração EF Core para `Lavoura`:**
```
Tabela: "lavouras"
PK: Id
FK: PropriedadeId → propriedades(Id)
Cultura: string (stored as name via HasConversion<string>)
Identificador: varchar(50)
AreaHectares: double, required
Saude: string (stored as name via HasConversion<string>)
NdviAtual: double, nullable
UltimaLeitura: timestamp with tz, nullable
Coordenadas: ComplexProperty (localizacao_lat, localizacao_lng)
CriadoEm: timestamp with tz, required
```

**Configuração EF Core para `DiagnosticoPraga`:**
```
Tabela: "diagnosticos"
PK: Id
FK: LavouraId → lavouras(Id), nullable
FotoUrl: text, required
Praga: string (stored as name via HasConversion<string>)
Confianca: double, required
Severidade: string (stored as name via HasConversion<string>)
Recomendacao: varchar(500), required
AgronomoTelefone: varchar(20)
CriadoEm: timestamp with tz, required
```

### Commit 3: `feat(api): LavourasController + DiagnosticosController + DTOs`

**Arquivos novos — FiapAgro.Api:**

| Arquivo | Conteúdo |
|---------|----------|
| `Dtos/LavourasDtos.cs` | `LavouraRequest` record + `LavouraResponse` record com `FromEntity()` |
| `Dtos/DiagnosticoDtos.cs` | `DiagnosticoRequest` record + `DiagnosticoResponse` record com `FromEntity()` |
| `Controllers/LavourasController.cs` | CRUD completo: GET (por propriedade), GET {id}, POST, PUT {id}, DELETE {id} |
| `Controllers/DiagnosticosController.cs` | GET {id}, GET (recentes/por lavoura), POST (upload foto ou resultado pré-computado) |

**DTOs detalhados:**

```csharp
// --- Lavoura ---
public record LavouraRequest(
    Guid PropriedadeId,
    string Cultura,           // "milho", "tomate", etc. → parse para CulturaTipo
    string Identificador,     // "L1"
    double AreaHectares,
    string? Saude = null,     // "saudavel" → parse para SaudeLavoura (default Saudavel)
    double? NdviAtual = null,
    double? Latitude = null,
    double? Longitude = null);

public record LavouraResponse(
    Guid Id,
    Guid PropriedadeId,
    string Cultura,           // "milho" (lowercase)
    string CulturaLabel,      // "Milho"
    string Identificador,
    double AreaHectares,
    string Saude,             // "saudavel" (lowercase)
    double? NdviAtual,
    string? UltimaLeitura,    // ISO 8601
    CoordenadaDto? Coordenadas,
    string CriadoEm)          // ISO 8601
{
    public static LavouraResponse FromEntity(Lavoura l) => ...;
}

public record CoordenadaDto(double Lat, double Lng);

// --- Diagnóstico ---
public record DiagnosticoRequest(
    Guid? LavouraId = null,
    string? Praga = null,      // se pré-computado (IoT/CV): "ferrugem_asiatica"
    double? Confianca = null,
    string? Severidade = null,
    string? Recomendacao = null,
    string? AgronomoTelefone = null);

public record DiagnosticoResponse(
    Guid Id,
    Guid? LavouraId,
    string FotoUri,            // URL pública da foto
    string Praga,              // "ferrugem_asiatica" (snake_case)
    string PragaLabel,         // "Ferrugem Asiática"
    double Confianca,
    string Severidade,         // "alto" (lowercase)
    string Recomendacao,
    string AgronomoTelefone,
    string CriadoEm)
{
    public static DiagnosticoResponse FromEntity(DiagnosticoPraga d) => ...;
}
```

**LavourasController endpoints:**

| Verbo + Rota | Ação | Response |
|--------------|------|----------|
| `GET /api/lavouras?propriedadeId={guid}` | Lista lavouras da propriedade | `LavouraResponse[]` |
| `GET /api/lavouras/{id}` | Busca por Id | `LavouraResponse` |
| `POST /api/lavouras` | Cria lavoura | 201 + `LavouraResponse` |
| `PUT /api/lavouras/{id}` | Atualiza lavoura | `LavouraResponse` |
| `DELETE /api/lavouras/{id}` | Remove lavoura | 204 No Content |

**DiagnosticosController endpoints:**

| Verbo + Rota | Ação | Response |
|--------------|------|----------|
| `POST /api/diagnosticos` | Upload foto + diagnóstico | 201 + `DiagnosticoResponse` |
| `GET /api/diagnosticos/{id}` | Busca por Id | `DiagnosticoResponse` |
| `GET /api/diagnosticos?lavouraId={guid}` | Lista diagnósticos de uma lavoura | `DiagnosticoResponse[]` |
| `GET /api/diagnosticos/recentes?quantidade=10` | Últimos diagnósticos | `DiagnosticoResponse[]` |

**POST /api/diagnosticos — lógica detalhada:**

1. Receber `IFormFile foto` + `DiagnosticoRequest` (via `[FromForm]`)
2. Validar: foto obrigatória, tamanho < 5MB, extensão `.jpg/.jpeg/.png`
3. Salvar foto em `wwwroot/uploads/diagnosticos/{guid}.jpg`
4. Se `request.Praga` preenchido → usar valores pré-computados (cenário IoT)
5. Se `request.Praga` null → simular inferência mock (Sprint 1): sortear praga, confiança, severidade
6. Construir entidade `DiagnosticoPraga`, salvar no banco
7. Retornar 201 com `DiagnosticoResponse`

### Commit 4: `test: testes unitários para Lavoura e DiagnosticoPraga`

**Arquivos novos — FiapAgro.Tests:**

| Arquivo | Testes |
|---------|--------|
| `LavouraTests.cs` | Construtor popula Id/CriadoEm; CulturaTipo parse; SaudeLavoura default; AreaHectares positivo; Coordenada nullable |
| `DiagnosticoTests.cs` | Construtor popula Id/CriadoEm; Confianca clamped 0..1; LavouraId nullable; PragaTipo parse; Severidade correta |

---

## 6. Mapeamento enum ↔ string (helpers)

Para converter enums C# (PascalCase) para strings snake_case que o mobile espera, criar um helper estático:

```csharp
// Domain/Helpers/EnumHelper.cs
public static class EnumHelper
{
    // "FerrugemAsiatica" → "ferrugem_asiatica"
    public static string ToSnakeCase(string pascalCase) { ... }

    // "ferrugem_asiatica" → PragaTipo.FerrugemAsiatica
    public static T FromSnakeCase<T>(string snakeCase) where T : struct, Enum { ... }

    // Labels PT-BR
    public static string CulturaLabel(CulturaTipo cultura) => cultura switch
    {
        CulturaTipo.Milho => "Milho",
        CulturaTipo.Tomate => "Tomate",
        CulturaTipo.Alface => "Alface",
        CulturaTipo.Feijao => "Feijão",
        CulturaTipo.Mandioca => "Mandioca",
        CulturaTipo.Soja => "Soja",
        CulturaTipo.Cana => "Cana-de-açúcar",
        _ => cultura.ToString()
    };

    public static string PragaLabel(PragaTipo praga) => praga switch
    {
        PragaTipo.Sadia => "Sadia",
        PragaTipo.FerrugemAsiatica => "Ferrugem Asiática",
        PragaTipo.LagartaDoCartucho => "Lagarta-do-cartucho",
        PragaTipo.ManchaFoliar => "Mancha Foliar",
        PragaTipo.Oidio => "Oídio",
        PragaTipo.MoscaBranca => "Mosca-branca",
        PragaTipo.BrocaDoCafe => "Broca-do-café",
        PragaTipo.Antracnose => "Antracnose",
        _ => praga.ToString()
    };
}
```

---

## 7. Dados de seed (AppDbContextSeed)

Adicionar ao seed existente (após as propriedades Fazenda ABC e Sítio Verde):

```
Lavouras:
  - Fazenda ABC: Milho L1 (80ha, saudavel, ndvi 0.72), Soja L2 (120ha, atencao, ndvi 0.58), Cana L3 (200ha, saudavel, ndvi 0.75)
  - Sítio Verde: Feijão L1 (40ha, saudavel, ndvi 0.68), Tomate L2 (30ha, risco, ndvi 0.35), Alface L3 (20ha, saudavel, ndvi 0.71)

Diagnósticos:
  - Sítio Verde / Tomate L2: ferrugem_asiatica, confiança 0.87, severidade Alto
  - Fazenda ABC / Milho L1: sadia, confiança 0.91, severidade Baixo
```

---

## 8. Impacto em arquivos existentes (alterações em código preexistente)

| Arquivo | Tipo de alteração | Detalhe |
|---------|-------------------|---------|
| `AppDbContext.cs` | MODIFICAR | +2 DbSets, +2 métodos de configuração no `OnModelCreating` |
| `AppDbContextSeed.cs` | MODIFICAR | +6 lavouras, +2 diagnósticos no método `SeedAsync` |
| `ServiceCollectionExtensions.cs` | MODIFICAR | +2 registros Scoped (ILavouraRepository, IDiagnosticoRepository) |
| `Program.cs` | MODIFICAR (possivelmente) | Adicionar `app.UseStaticFiles()` se salvar fotos em `wwwroot/` |

**Nenhum arquivo existente será removido ou renomeado.**

---

## 9. Riscos e mitigações

| Risco | Impacto | Mitigação |
|-------|---------|-----------|
| Enum `CulturaTipo` diverge do mobile se alguém adicionar cultura | Médio | Documentar que ambos os lados devem ser atualizados simultaneamente |
| Upload de foto em disco sem cleanup | Baixo | Sprint 1 aceitável; Sprint 2 pode migrar para blob storage |
| Migration pesada (2 tabelas novas + FKs) | Baixo | Banco de dev; seed idempotente |
| Serialização snake_case inconsistente | Alto | Testar manualmente que o JSON de saída bate exatamente com os tipos do mobile |
| Acoplamento entre DiagnosticoPraga e modelo YOLO ausente | Nenhum | Sprint 1 mock; endpoint aceita resultado pré-computado |

---

## 10. Checklist de aceite

- [ ] `GET /api/lavouras?propriedadeId=X` retorna array de lavouras com todos os campos que o mobile espera
- [ ] `GET /api/lavouras/{id}` retorna lavoura com `cultura` em snake_case e `culturaLabel` em PT-BR
- [ ] `POST /api/lavouras` cria lavoura e retorna 201
- [ ] `PUT /api/lavouras/{id}` atualiza lavoura
- [ ] `DELETE /api/lavouras/{id}` remove lavoura e retorna 204
- [ ] `POST /api/diagnosticos` aceita upload de foto + cria diagnóstico
- [ ] `GET /api/diagnosticos/{id}` retorna diagnóstico com `praga` em snake_case e `pragaLabel` em PT-BR
- [ ] `GET /api/diagnosticos?lavouraId=X` lista diagnósticos de uma lavoura
- [ ] `GET /api/diagnosticos/recentes` lista diagnósticos recentes
- [ ] Seed popula lavouras e diagnósticos de exemplo
- [ ] Migration roda sem erros sobre o schema atual
- [ ] Testes unitários passam
- [ ] Build limpo sem warnings
- [ ] JSON response fields batem 1:1 com os tipos do `lib/types.ts` do mobile

---

## 11. Árvore de arquivos criados/modificados (visão resumida)

```
FiapAgro.Domain/
├── Entities/
│   ├── Lavoura.cs                    ← NOVO
│   └── DiagnosticoPraga.cs           ← NOVO
├── Enums/
│   ├── CulturaTipo.cs                ← NOVO
│   ├── SaudeLavoura.cs               ← NOVO
│   └── PragaTipo.cs                  ← NOVO
├── Helpers/
│   └── EnumHelper.cs                 ← NOVO
└── Interfaces/
    ├── ILavouraRepository.cs         ← NOVO
    └── IDiagnosticoRepository.cs     ← NOVO

FiapAgro.Infrastructure/
├── Data/
│   ├── AppDbContext.cs               ← MODIFICAR (+2 DbSets, +2 configs)
│   ├── AppDbContextSeed.cs           ← MODIFICAR (+lavouras, +diagnósticos)
│   └── Migrations/
│       └── XXXXXXXX_AddLavouraDiagnostico.cs  ← NOVO (gerado)
├── Extensions/
│   └── ServiceCollectionExtensions.cs ← MODIFICAR (+2 registros DI)
└── Repositories/
    ├── LavouraRepositoryEF.cs        ← NOVO
    └── DiagnosticoRepositoryEF.cs    ← NOVO

FiapAgro.Api/
├── Controllers/
│   ├── LavourasController.cs         ← NOVO
│   └── DiagnosticosController.cs     ← NOVO
├── Dtos/
│   ├── LavourasDtos.cs               ← NOVO
│   └── DiagnosticoDtos.cs            ← NOVO
└── Program.cs                        ← MODIFICAR (UseStaticFiles, se necessário)

FiapAgro.Tests/
├── LavouraTests.cs                   ← NOVO
└── DiagnosticoTests.cs               ← NOVO
```

**Total: ~14 arquivos novos, ~4 arquivos modificados.**
