# 2f-agro-backend

Backend C# .NET 8 + arquitetura SOA do 2F-AGRO.
Materias: **C#** (100pts) + **SOA** | FIAP 3ES | GS 2026.1

[![Hub](https://img.shields.io/badge/hub-2f--agro-success)](https://github.com/GS-SPACE-CONNECT/2f-agro)

## Objetivo
API REST em .NET 8 que serve duas materias: **C#** (POO completa: abstract, heranca, polimorfismo, interfaces, structs, partial, excecoes) + **SOA** (microsservicos, REST, MQ, Gateway).

## Owners
[@brnleao](https://github.com/brnleao), [@DevRuanVieira](https://github.com/DevRuanVieira), [@jota0802](https://github.com/jota0802) | Team [`backend`](https://github.com/orgs/GS-SPACE-CONNECT/teams/backend)

## Stack
.NET 8 (Web API + Console) | EF Core 8 | PostgreSQL + PostGIS | RabbitMQ | xUnit | JWT/Identity

## POO (rubrica C# completa)
- Classes Abstratas + Heranca + Polimorfismo: Alerta -> AlertaPraga, AlertaSeca, AlertaGeada, AlertaEnchente, AlertaErosao
- Interfaces + DI: IDetector<T>, INotificador
- Structs: Coordenada readonly
- Partial Classes: Propriedade (split props/metodos)
- Tratamento de Excecoes: fallback resiliente
- DateTime + logica de fluxo

## Setup
dotnet restore
dotnet ef database update
dotnet run --project FiapAgro.Api

## Links
- [Spec C#](https://github.com/GS-SPACE-CONNECT/2f-agro/blob/main/docs/specs/2026-05-27-2f-agro-design.md) | [Spec SOA](https://github.com/GS-SPACE-CONNECT/2f-agro/blob/main/docs/specs/2026-05-27-2f-agro-design.md)
