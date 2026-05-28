# 🟦 2f-agro-backend

> Backend C# .NET 8 + arquitetura SOA do 2F-AGRO.
> Matérias: **C#** (100 pts) + **SOA** · FIAP 3ES · GS 2026.1

[![Hub](https://img.shields.io/badge/hub-2f--agro-success)](https://github.com/GS-SPACE-CONNECT/2f-agro)

## 🎯 Objetivo
API REST em .NET 8 que serve duas matérias ao mesmo tempo: **C#** (POO completa — abstract, herança, polimorfismo, interfaces, structs, partial, exceções) + **SOA** (microsserviços, REST, MQ, Gateway).

## 👥 Owners
[@brnleao](https://github.com/brnleao), [@DevRuanVieira](https://github.com/DevRuanVieira), [@jota0802](https://github.com/jota0802) · Team [`backend`](https://github.com/orgs/GS-SPACE-CONNECT/teams/backend)

## 🧩 Stack
.NET 8 (Web API + Console) · EF Core 8 · PostgreSQL + PostGIS · RabbitMQ · xUnit · JWT/Identity

## 🏛️ POO (rubrica C# completa)
- ✅ Classes abstratas + herança + polimorfismo → `Alerta` → `AlertaPraga`, `AlertaSeca`, `AlertaGeada`, `AlertaEnchente`, `AlertaErosao`
- ✅ Interfaces + DI → `IDetector<T>`, `INotificador`
- ✅ Structs → `Coordenada` (readonly)
- ✅ Partial classes → `Propriedade` (split em props/métodos)
- ✅ Tratamento de exceções → fallback resiliente
- ✅ DateTime + lógica de fluxo

## 🚀 Setup local
```bash
dotnet restore
dotnet ef database update
dotnet run --project FiapAgro.Api
```

## 🔗 Links
- [Spec § 4.5 C#](https://github.com/GS-SPACE-CONNECT/2f-agro/blob/main/docs/specs/2026-05-27-2f-agro-design.md) · [Spec § 4.7 SOA](https://github.com/GS-SPACE-CONNECT/2f-agro/blob/main/docs/specs/2026-05-27-2f-agro-design.md)
