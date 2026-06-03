# Contrato de Endpoints — Lavoura + Diagnóstico de Praga

> **Feature:** `feat/lavoura-diagnostico-mobile`
> **Base URL:** `/api`
> **Autenticação:** Bearer JWT em todas as rotas

---

## 1. Lavouras

### 1.1 `GET /api/lavouras?propriedadeId={guid}`

Lista lavouras de uma propriedade.

| Item | Detalhe |
|------|---------|
| **Método** | `GET` |
| **Query param** | `propriedadeId` (Guid, obrigatório) |
| **Autenticação** | Bearer JWT |
| **Sucesso** | `200 OK` — `LavouraResponse[]` |

**Response body (exemplo):**

```json
[
  {
    "id": "a1b2c3d4-0000-0000-0000-000000000001",
    "propriedadeId": "f1e2d3c4-0000-0000-0000-000000000001",
    "cultura": "milho",
    "culturaLabel": "Milho",
    "identificador": "L1",
    "areaHectares": 80.0,
    "saude": "saudavel",
    "ndviAtual": 0.72,
    "ultimaLeitura": "2026-06-01T18:00:00.0000000Z",
    "coordenadas": { "lat": -21.179, "lng": -47.8115 },
    "criadoEm": "2026-06-01T12:00:00.0000000Z"
  }
]
```

---

### 1.2 `GET /api/lavouras/{id}`

Retorna uma lavoura pelo Id.

| Item | Detalhe |
|------|---------|
| **Método** | `GET` |
| **Path param** | `id` (Guid) |
| **Sucesso** | `200 OK` — `LavouraResponse` |
| **Erro** | `404 Not Found` — Lavoura não encontrada |

**Response body (exemplo):**

```json
{
  "id": "a1b2c3d4-0000-0000-0000-000000000001",
  "propriedadeId": "f1e2d3c4-0000-0000-0000-000000000001",
  "cultura": "soja",
  "culturaLabel": "Soja",
  "identificador": "L2",
  "areaHectares": 120.0,
  "saude": "atencao",
  "ndviAtual": 0.58,
  "ultimaLeitura": "2026-06-01T12:00:00.0000000Z",
  "coordenadas": { "lat": -21.18, "lng": -47.8095 },
  "criadoEm": "2026-06-01T12:00:00.0000000Z"
}
```

---

### 1.3 `POST /api/lavouras`

Cadastra uma nova lavoura.

| Item | Detalhe |
|------|---------|
| **Método** | `POST` |
| **Content-Type** | `application/json` |
| **Sucesso** | `201 Created` — `LavouraResponse` + header `Location` |
| **Erro** | `400 Bad Request` — valor de enum inválido |

**Request body:**

```json
{
  "propriedadeId": "f1e2d3c4-0000-0000-0000-000000000001",
  "cultura": "feijao",
  "identificador": "L4",
  "areaHectares": 25.5,
  "saude": "saudavel",
  "ndviAtual": 0.65,
  "latitude": -21.18,
  "longitude": -47.81
}
```

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `propriedadeId` | Guid | Sim | FK da propriedade |
| `cultura` | string | Sim | snake_case: `milho`, `tomate`, `alface`, `feijao`, `mandioca`, `soja`, `cana` |
| `identificador` | string | Sim | Label local do agricultor (ex.: "L1") |
| `areaHectares` | double | Sim | Área em hectares |
| `saude` | string | Não | `saudavel` (default), `atencao`, `risco`, `perdida` |
| `ndviAtual` | double? | Não | NDVI entre -1 e 1 |
| `latitude` | double? | Não | Latitude (-90 a 90) |
| `longitude` | double? | Não | Longitude (-180 a 180) |

**Response body (exemplo):**

```json
{
  "id": "b2c3d4e5-0000-0000-0000-000000000002",
  "propriedadeId": "f1e2d3c4-0000-0000-0000-000000000001",
  "cultura": "feijao",
  "culturaLabel": "Feijão",
  "identificador": "L4",
  "areaHectares": 25.5,
  "saude": "saudavel",
  "ndviAtual": 0.65,
  "ultimaLeitura": null,
  "coordenadas": { "lat": -21.18, "lng": -47.81 },
  "criadoEm": "2026-06-02T14:30:00.0000000Z"
}
```

---

### 1.4 `PUT /api/lavouras/{id}`

Atualiza dados de uma lavoura existente.

| Item | Detalhe |
|------|---------|
| **Método** | `PUT` |
| **Path param** | `id` (Guid) |
| **Content-Type** | `application/json` |
| **Sucesso** | `200 OK` — `LavouraResponse` |
| **Erro** | `404 Not Found` / `400 Bad Request` |

**Request body:** mesmo formato de `POST /api/lavouras`.

---

### 1.5 `DELETE /api/lavouras/{id}`

Remove uma lavoura.

| Item | Detalhe |
|------|---------|
| **Método** | `DELETE` |
| **Path param** | `id` (Guid) |
| **Sucesso** | `204 No Content` |
| **Erro** | `404 Not Found` |

---

## 2. Diagnósticos de Praga

### 2.1 `POST /api/diagnosticos`

Upload de foto + criação de diagnóstico. Aceita resultado pré-computado (IoT/CV) ou simula inferência mock (Sprint 1).

| Item | Detalhe |
|------|---------|
| **Método** | `POST` |
| **Content-Type** | `multipart/form-data` |
| **Sucesso** | `201 Created` — `DiagnosticoResponse` + header `Location` |
| **Erro** | `400 Bad Request` — foto ausente, tamanho > 5 MB, formato inválido, enum inválido |

**Form fields:**

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| `foto` | File | Sim | Imagem `.jpg`, `.jpeg` ou `.png` (máx. 5 MB) |
| `lavouraId` | Guid? | Não | FK da lavoura (foto pode ser "solta") |
| `praga` | string? | Não | Se informado, usa resultado pré-computado: `sadia`, `ferrugem_asiatica`, `lagarta_do_cartucho`, `mancha_foliar`, `oidio`, `mosca_branca`, `broca_do_cafe`, `antracnose` |
| `confianca` | double? | Não | 0.0 a 1.0 (default 0.5 se pré-computado) |
| `severidade` | string? | Não | `baixo`, `medio`, `alto`, `critico` (default `medio` se pré-computado) |
| `recomendacao` | string? | Não | Texto curto (máx. 500 chars) |
| `agronomoTelefone` | string? | Não | Formato E.164 (default `+5511999990000`) |

**Response body (exemplo):**

```json
{
  "id": "c3d4e5f6-0000-0000-0000-000000000003",
  "lavouraId": "a1b2c3d4-0000-0000-0000-000000000001",
  "fotoUri": "/uploads/diagnosticos/c3d4e5f6-abcd-1234-efgh-567890abcdef.jpg",
  "praga": "ferrugem_asiatica",
  "pragaLabel": "Ferrugem Asiática",
  "confianca": 0.87,
  "severidade": "alto",
  "recomendacao": "Aplicar fungicida sistêmico e monitorar folhas adjacentes.",
  "agronomoTelefone": "+5541999990001",
  "criadoEm": "2026-06-02T14:45:00.0000000Z"
}
```

---

### 2.2 `GET /api/diagnosticos/{id}`

Retorna um diagnóstico pelo Id.

| Item | Detalhe |
|------|---------|
| **Método** | `GET` |
| **Path param** | `id` (Guid) |
| **Sucesso** | `200 OK` — `DiagnosticoResponse` |
| **Erro** | `404 Not Found` |

**Response body:** mesmo formato do response de `POST`.

---

### 2.3 `GET /api/diagnosticos?lavouraId={guid}`

Lista diagnósticos de uma lavoura. Se `lavouraId` não for informado, retorna os 10 mais recentes.

| Item | Detalhe |
|------|---------|
| **Método** | `GET` |
| **Query param** | `lavouraId` (Guid, opcional) |
| **Sucesso** | `200 OK` — `DiagnosticoResponse[]` |

---

### 2.4 `GET /api/diagnosticos/recentes?quantidade={int}`

Lista diagnósticos mais recentes.

| Item | Detalhe |
|------|---------|
| **Método** | `GET` |
| **Query param** | `quantidade` (int, default 10) |
| **Sucesso** | `200 OK` — `DiagnosticoResponse[]` |

---

## 3. Status Codes Comuns

| Código | Significado |
|--------|-------------|
| `200 OK` | Requisição bem-sucedida |
| `201 Created` | Recurso criado com sucesso (inclui header `Location`) |
| `204 No Content` | Recurso removido com sucesso |
| `400 Bad Request` | Validação falhou (enum inválido, regra de negócio violada) |
| `401 Unauthorized` | Token JWT ausente ou inválido |
| `404 Not Found` | Recurso não encontrado |
| `422 Unprocessable Entity` | Erro de domínio genérico |
| `500 Internal Server Error` | Erro inesperado do servidor |

Todos os erros seguem o formato **RFC 7807 Problem Details**:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Lavoura 'a1b2c3d4-...' não encontrado.",
  "traceId": "00-abc123..."
}
```

---

## 4. Tipos Enum (valores aceitos)

### `CulturaTipo`
`milho` | `tomate` | `alface` | `feijao` | `mandioca` | `soja` | `cana`

### `SaudeLavoura`
`saudavel` | `atencao` | `risco` | `perdida`

### `PragaTipo`
`sadia` | `ferrugem_asiatica` | `lagarta_do_cartucho` | `mancha_foliar` | `oidio` | `mosca_branca` | `broca_do_cafe` | `antracnose`

### `NivelSeveridade`
`baixo` | `medio` | `alto` | `critico`
