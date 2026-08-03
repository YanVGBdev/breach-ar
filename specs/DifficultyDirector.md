# DifficultyDirector

## Objetivo
Ajustar dinamicamente a dificuldade do jogo baseado no desempenho do jogador, mantendo o desafio equilibrado e envolvente.

## Responsabilidades
- Monitorar métricas de desempenho (HP do Core, combo, tempo de onda, mortes).
- Ajustar parâmetros de dificuldade (HP de Fragments, velocidade, spawn rate).
- Implementar DDA (Dynamic Difficulty Adjustment) para prevenir frustração ou tédio.
- Fornecer nível de dificuldade atual para outros sistemas.
- **Não** faz: spawn de Fragments (é `WaveGenerator`/`EnemySpawner`), nem balanceamento de Orbs (é `LaunchSystem`).

## Dependências
- `ScoreSystem` (dados de performance: score, combo).
- `CoreController` (HP restante como métrica).
- `SessionStateMachine` (progresso da sessão).
- `DifficultyConfig` (parâmetros de ajuste: thresholds, deltas).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnDifficultyChanged` | `DifficultyChangedData` (previousLevel, newLevel, reason) | Nível de dificuldade muda |
| `OnDifficultyMetricUpdated` | `DifficultyMetricData` (metric, value, timestamp) | Métrica de performance atualizada |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnWaveCompleted` | `EnemySpawner` | Avalia performance da onda e ajusta dificuldade |
| `OnCoreDamaged` | `CoreController` | Reduz dificuldade se Core com HP baixo |
| `OnComboChanged` | `ComboSystem` | Aumenta dificuldade se combo alto |
| `OnFragmentKilled` | `FragmentController` | Rastreia taxa de morte como métrica |
| `OnGameOver` | `GameManager` | Registra resultado final para ajuste futuro |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `DifficultyDirector` | Lógica DDA: coleta métricas, calcula ajustes |
| `DifficultyConfig` | Configuração: thresholds, deltas, limites |
| `WaveGenerator` | Usa nível de dificuldade para gerar ondas |
| `RiftSpawnDirector` | Usa nível de dificuldade para spawn de Rifts |

## Referências
- GDD: `05_ai.md §5.1`, `05_ai.md §5.2`
- Backlog: `AI-001`, `AI-002`, `AI-003`
