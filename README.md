# HexagonalExemplo - Arquitetura Hexagonal em .NET

Projeto de exemplo demonstrando **Arquitetura Hexagonal (Ports and Adapters)** com .NET 9, utilizando **Dapper** para persistência e **Scalar** para documentação da API.

## 📐 Arquitetura Hexagonal

A Arquitetura Hexagonal (também conhecida como Ports and Adapters) separa a aplicação em camadas com dependências direcionadas para o centro:

```
         ┌─────────────────────────────────────┐
         │           Camada Externa            │
         │  (Drivers: API, CLI, Testes...)     │
         │            ↑                        │
         │            │                        │
         │  ┌─────────┴──────────┐            │
         │  │    Application     │            │
         │  │   (Casos de Uso)   │            │
         │  └─────────┬──────────┘            │
         │            │                        │
         │  ┌─────────┴──────────┐            │
         │  │      Domain        │            │
         │  │ (Entidades, Ports) │            │
         │  └─────────┬──────────┘            │
         │            │                        │
         │  ┌─────────┴──────────┐            │
         │  │  Infrastructure      │            │
         │  │(Adapters: DB, Ext)   │            │
         └─────────────────────────────────────┘
```

## 🏗️ Estrutura de Projetos

```
HexagonalExemplo/
├── src/
│   ├── HexagonalExemplo.Dominio/        # Core - Regras de negócio
│   │   ├── Entidades/                   # Entidades do domínio
│   │   ├── ObjetosDeValor/              # Value Objects
│   │   ├── Repositorios/                # Interfaces (Ports)
│   │   └── Excecoes/                    # Exceções de domínio
│   │
│   ├── HexagonalExemplo.Aplicacao/      # Casos de uso
│   │   ├── CasosDeUso/                  # Implementações dos casos de uso
│   │   ├── DTOs/                        # Data Transfer Objects
│   │   ├── Interfaces/                  # Contratos dos casos de uso
│   │   └── Mapeamentos/                 # Mapeamentos entre entidades e DTOs
│   │
│   ├── HexagonalExemplo.Infraestrutura/ # Adapters externos
│   │   ├── Repositorios/                # Implementações dos repositórios
│   │   └── Persistencia/                # DbContext, configurações EF
│   │
│   └── HexagonalExemplo.API/            # Driver Adapter (Web API)
│       ├── Controladores/               # Controllers
│       └── Configuracoes/               # Configurações da API
│
└── tests/
    └── HexagonalExemplo.Testes/         # Testes unitários e integração
```

## 🔄 Fluxo de Dependências

```
Domain ← Infrastructure
  ↑
Application
  ↑
API / Testes
```

**Regras:**
- `Domain` não depende de ninguém
- `Application` depende apenas de `Domain`
- `Infrastructure` depende de `Domain` e `Application`
- `API` depende de `Application` e `Infrastructure`

## � Mensageria na Arquitetura Hexagonal

Exemplo: **Notificação de Tarefa Concluída** (padrão Saga/Outbox)

### Diagrama de Mensageria

```
┌─────────────────────────────────────────────────────────────────────┐
│                           FLUXO DO EVENTO                           │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────────────┐  │
│  │   API        │    │ Aplicação    │    │   Infraestrutura     │  │
│  │  (Driver)    │───→│  CasoDeUso   │───→│  Adapter (MassTransit)│  │
│  │              │    │  Concluir()  │    │   PublicarAsync()    │  │
│  └──────────────┘    └──────────────┘    └──────────┬───────────┘  │
│                                                     │               │
│                           ┌─────────────────────────┘               │
│                           ↓                                         │
│                    ┌──────────────┐                                 │
│                    │  RabbitMQ    │                                 │
│                    │  /In-Memory  │                                 │
│                    └──────┬───────┘                                 │
│                           │                                         │
│                           ↓                                         │
│                    ┌──────────────┐                                 │
│                    │  Consumer    │  ← Outro serviço/adapter        │
│                    │ (Notificação)│    (envia email, relatórios) │
│                    └──────────────┘                                 │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

### Onde cada peça mora:

| Conceito | Tipo | Localização | Implementação |
|----------|------|-------------|---------------|
| `TarefaConcluidaEvento` | Domain Event | `Dominio/Eventos/` | Record puro |
| `IBarramentoDeMensagens` | Port (Secondary) | `Dominio/BarramentoDeMensagens/` | Interface |
| `BarramentoMassTransit` | Adapter | `Infraestrutura/Mensageria/` | MassTransit + RabbitMQ |
| `TarefaConcluidaConsumidor` | Adapter (Inbound) | `Infraestrutura/Mensageria/Consumidores/` | IConsumer<T> |
| Publicação do evento | Use Case | `Aplicacao/CasosDeUso/` | `_barramento.PublicarAsync()` |

### Vantagens dessa abordagem:

1. **Domain Events**: Evento nasce no domínio (regra de negócio: "quando concluir, notificar")
2. **Port abstrato**: Application não sabe se é RabbitMQ, Kafka ou In-Memory
3. **Testabilidade**: Pode mockar `IBarramentoDeMensagens` nos testes
4. **Resiliência**: Consumer roda em processo separado (pode estar em outro serviço)

## �🚀 Como Executar

```bash
# Compilar a solução
dotnet build

# Executar a API
cd src/HexagonalExemplo.API
dotnet run

# Acessar documentação Scalar
# http://localhost:5000/scalar
```

## 🔧 Migrations (FluentMigrator)

As migrations são aplicadas automaticamente na inicialização da API. Para aplicar migrations manualmente sem executar o servidor, use a opção `--migrate-only` ao executar o projeto API.

PowerShell:

```powershell
dotnet run --project src/HexagonalExemplo.API -- --migrate-only
```

Bash / macOS / Linux:

```bash
dotnet run --project src/HexagonalExemplo.API -- --migrate-only
```

Também há scripts de conveniência em `scripts/`:

- `scripts/apply-migrations.ps1` — PowerShell
- `scripts/apply-migrations.sh` — Bash
 - `tools/MigrateRunner` — small .NET tool to apply migrations (`dotnet run --project tools/MigrateRunner`)
 - `scripts/run-tool-migrations.ps1` / `scripts/run-tool-migrations.sh` — convenience scripts to run the tool

Optional: there's a local tool manifest for the FluentMigrator CLI at `.config/dotnet-tools.json`. You can install the local tools with:

```powershell
dotnet tool restore
```

After `dotnet tool restore` you can run the FluentMigrator CLI (if installed by the manifest) or use the provided `MigrateRunner` project.

Ambas executam `dotnet run --project src/HexagonalExemplo.API -- --migrate-only`.

Observação: o projeto usa SQLite em memória com `Cache=Shared`. Ao aplicar migrations manualmente, certifique-se de que o processo que executa as migrations mantenha a conexão aberta (o script usa `dotnet run` que cria e encerra o processo — para ambientes persistentes, execute a API normalmente).

## 📚 Endpoints da API

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/tarefas` | Lista todas as tarefas |
| GET | `/api/tarefas/{id}` | Obtém uma tarefa por ID |
| POST | `/api/tarefas` | Cria uma nova tarefa |
| PUT | `/api/tarefas/{id}` | Atualiza uma tarefa |
| PATCH | `/api/tarefas/{id}/concluir` | Marca como concluída |
| PATCH | `/api/tarefas/{id}/cancelar` | Cancela a tarefa |
| DELETE | `/api/tarefas/{id}` | Remove a tarefa |

## 🎯 Conceitos Demonstrados

- **Ports**: Interfaces definidas no Domain (`ITarefaRepositorio`)
- **Adapters**: Implementações na Infrastructure (`TarefaRepositorio`)
- **Inversão de Dependência**: Application conhece apenas as interfaces (Ports)
- **Injeção de Dependência**: Configurada em `Program.cs`
- **Domain-Driven**: Entidades ricas com comportamentos (métodos `Concluir()`, `Cancelar()`)

## 📝 Sobre a Nomenclatura

Este projeto segue **Arquitetura Hexagonal (Ports and Adapters)**, mas usa **nomenclatura pragmática** ao invés de nomes puristas de patterns:

### Por que não usamos "Adapters" nos nomes?

| Aspecto | Justificativa |
|---------|---------------|
| **Ubiquitous Language** | Nosso time fala em "Repositórios" e "Controllers", não em "Secondary Driven Adapters" |
| **Clareza** | `TarefaRepositorio` diz **o que faz**. `TarefaAdapter` é vago — adapter de quê? |
| **Onboarding** | Evitamos que devs precisem decorar glossário de patterns |
| **Explicitação** | Ports já estão implícitos: `ITarefaRepositorio` no **Domínio** é a Port, independente do nome |

### Onde estão os Ports e Adapters?

```
┌─────────────────────────────────────────────────────────────┐
│  Ports (Interfaces)      │  ITarefaRepositorio (Domain)     │
│  ← Saída do Domínio      │  ← Contrato que o domínio expõe  │
├─────────────────────────────────────────────────────────────┤
│  Adapters (Implementação)│  TarefaRepositorio (Infra)       │
│  → Adaptam tecnologia    │  → Adapta SQL/banco para o Port  │
├─────────────────────────────────────────────────────────────┤
│  Adapters (Entrada)      │  TarefasController (API)         │
│  → Adaptam protocolo   │  → Adapta HTTP/JSON para o UseCase│
└─────────────────────────────────────────────────────────────┘
```

> **Arquitetura Hexagonal ≠ Nomes de Pastas Hexagonais**. Chamamos pelo que **FAZEM**, não pelo que **SÃO** na arquitetura.

## 📝 Licença

MIT
