# RiftSystem

## Objetivo
Gerenciar o ciclo de vida das Rifts (fendas dimensionais) ancoradas em superfícies reais: spawn, dano/integridade, spawn periódico de Fragments e fechamento.

## Responsabilidades
- Instanciar e ancorar Rifts em superfícies classificadas (parede/chão/teto/móvel).
- Gerenciar Integridade de cada Rift (dano recebido de Orbes).
- Disparar spawn periódico de Fragments enquanto a Rift estiver aberta.
- Executar sequência de fechamento (implosão) ao atingir Integridade 0.
- **Não** faz: pathfinding de Fragments (isso é `EnemySpawner`/AI), nem física de Orbes (`OrbLaunch`), nem distribuição procedural de posições (isso é `ARSurfaceService` + `AI-006`/`AR-018`).

## Dependências
- `ARSurfaceService` (obtém superfícies válidas e anchors).
- `FragmentDefinitionSO`, `WaveDefinitionSO` (o que spawnar e com que frequência).
- `RiftDefinitionSO` (Integridade base, VFX, cooldown de spawn).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnRiftSpawned` | `RiftData` (id, tipo de superfície, posição) | Rift instanciada e ancorada |
| `OnRiftDamaged` | `RiftData`, `float amount` | Orbe atinge a Rift |
| `OnRiftClosed` | `RiftData` | Integridade chega a 0 |
| `OnFragmentSpawnRequested` | `FragmentDefinitionSO`, posição de spawn | Timer interno de spawn dispara |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnOrbHit` | `OrbLaunch` | Se alvo for uma Rift, aplica dano via `OnRiftDamaged` |
| `OnWaveStarted` | `EnemySpawner`/`SessionStateMachine` | Ativa/reconfigura Rifts da onda atual |
| `OnSessionEnded` | `SessionStateMachine` | Libera todas as Rifts/anchors ativas |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `RiftController` | MonoBehaviour por instância de Rift; estado (aberta/fechando/fechada), timer de spawn |
| `RiftDefinitionSO` | Dados de configuração por tipo de Rift |
| `RiftSpawnDirector` | Decide onde/quando novas Rifts aparecem (usa `ARSurfaceService` + regras de `AI-006`/`AR-018`) |

## Referências
- GDD: `02_gameplay.md §2.1.3`, `03_ar_system.md §3.6, §3.8`
- Backlog: `GP-005`, `AR-007`, `AR-018`, `AR-024`, `PH-008`
