# EnemySpawner

## Objetivo
Gerenciar a composição e spawn de ondas de Fragments, controlando o fluxo de inimigos durante o gameplay.

## Responsabilidades
- Interpretar `WaveDefinitionSO` para compor ondas de Fragments.
- Spawnar Fragments de acordo com a configuração da onda atual.
- Controlar o ritmo de spawn (intervalos, lotes, condições de clear).
- Rastrear Fragments ativos e determinar quando uma onda está completa.
- **Não** faz: pathfinding de Fragments (é responsabilidade de `FragmentController`), nem dano ao Core (é `CoreController`), nem spawn de Rifts (é `RiftSpawnDirector`).

## Dependências
- `WaveDefinitionSO` (definição da composição da onda).
- `FragmentDefinitionSO` (dados de cada tipo de Fragment).
- `ObjectPool` (para instanciar Fragments).
- `DifficultyDirector` (para ajustar parâmetros de spawn).
- `CoreController` (referência para target dos Fragments).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnWaveStarted` | `WaveStartedData` (waveIndex, totalWaves, isBossWave) | Nova onda inicia |
| `OnWaveCompleted` | `WaveCompletedData` (waveIndex, timeTaken, coreHpRemaining, perfectWave) | Todos os Fragments da onda derrotados |
| `OnFragmentSpawned` | `FragmentSpawnData` (fragmentId, position, waveIndex) | Fragment instanciado |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnFragmentKilled` | `FragmentController` | Decrementa contador de Fragments ativos; verifica clear da onda |
| `OnRiftClosed` | `RiftController` | Pode afetar spawn se Rift estava gerando Fragments |
| `OnSessionEnded` | `SessionStateMachine` | Para spawn e limpa Fragments restantes |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `WaveGenerator` | Gera definições de onda baseado em dificuldade |
| `WaveDefinitionSO` | Dados da composição da onda (lista de Fragments, timing) |
| `FragmentDefinitionSO` | Configuração de cada tipo de Fragment |
| `FragmentController` | Controla comportamento individual do Fragment |

## Referências
- GDD: `02_gameplay.md §2.1.2`, `05_ai.md §5.3`
- Backlog: `AI-006`, `GP-009`, `GP-011`
