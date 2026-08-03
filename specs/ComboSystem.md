# ComboSystem

## Objetivo
Gerenciar o multiplicador de combo baseado em acertos consecutivos dentro de uma janela de tempo, recompensando habilidade do jogador.

## Responsabilidades
- Rastrear acertos consecutivos e incrementar multiplicador.
- Aplicar penalidade de tempo (janela de combo) que reseta se não houver acerto.
- Fornecer multiplicador atual para cálculo de pontuação.
- Emitir eventos de mudança de combo para UI e outros sistemas.
- **Não** faz: cálculo de pontuação (é `ScoreSystem`), nem spawn de Fragments (é `EnemySpawner`).

## Dependências
- `ComboSystemConfig` (configuração: janela de tempo, incremento, máximo).
- `ScoreSystem` (usa multiplicador para calcular pontuação).
- `HUDManager` (exibe multiplicador na tela).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnComboChanged` | `ComboChangedData` (multiplier, comboCount, wasReset) | Multiplicador muda ou é resetado |
| `OnComboMilestone` | `ComboMilestoneData` (milestone, multiplier) | Multiplicador atinge marco (2x, 3x, etc.) |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnFragmentKilled` | `FragmentController` | Incrementa combo via `RegisterHit()` |
| `OnRiftClosed` | `RiftController` | Incrementa combo |
| `OnBossDefeated` | `BossController` | Incrementa combo significativamente |
| `OnWaveStarted` | `EnemySpawner` | Mantém combo entre ondas (opcional) |
| `OnGameOver` | `GameManager` | Desativa combo system |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `ComboSystem` | Lógica principal: tracking, multiplicador, janela de tempo |
| `ComboSystemConfig` | Configuração: comboWindow, comboIncrement, maxMultiplier |
| `HUDManager` | Exibe multiplicador e progresso da janela |

## Referências
- GDD: `02_gameplay.md §2.1.4`, `07_ui.md §7.3`
- Backlog: `GP-007`, `UI-006`
