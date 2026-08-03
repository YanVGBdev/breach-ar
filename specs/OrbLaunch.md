# OrbLaunch

## Objetivo
Gerenciar o lançamento e voo de Orbes, convertendo input do jogador em força de lançamento e controlando o ciclo de vida do Orbe.

## Responsabilidades
- Capturar input de arraste (toque/mouse) e calcular vetor de lançamento.
- Mostrar preview de trajetória durante o arraste.
- Instanciar e lançar Orbes com força baseada no arraste.
- Controlar ricochete, dano e expiração do Orbe.
- Aplicar dano a Fragments, Rifts e superfícies do mundo real.
- Gerenciar ciclo de vida via object pooling.
- **Não** faz: cálculo de combo (é `ComboSystem`), pontuação (é `ScoreSystem`), nem spawn de Fragments (é `EnemySpawner`).

## Dependências
- `OrbDefinitionSO` (configuração do Orbe: massa, dano, ricochetes).
- `InputHandler` (captura de input unificado).
- `PhysicsManager` (configurações de física e colliders).
- `PoolManager` (para gerenciamento de ciclo de vida via pooling).
- `Camera` (para projeção de input em coordenadas do mundo).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnOrbLaunched` | `OrbLaunchData` (orbId, direction, force) | Orbe é lançado |
| `OnOrbHit` | `OrbHitData` (orbId, hitPosition, targetId, isRift, isFragment, isCore) | Orbe atinge um alvo |
| `OnOrbRicochet` | `OrbRicochetData` (orbId, position, ricochetCount) | Orbe ricocheteia em superfície |
| `OnOrbExpired` | `OrbExpiredData` (orbId, reason) | Orbe expira (ricochetes máximos ou tempo) |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnWaveStarted` | `EnemySpawner` | Pode mudar tipo de Orbe disponível |
| `OnPowerUpCollected` | `PowerUpController` | Aplica efeito temporal ao próximo Orbe |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `LaunchSystem` | Input de arraste, cálculo de força, preview de trajetória |
| `OrbController` | Comportamento do Orbe em voo (física, ricochete, dano, expiração) |
| `OrbDefinitionSO` | Dados de configuração do Orbe |
| `TrajectoryPreview` | Renderização da linha de trajetória |
| `PoolManager` | Gerenciamento de ciclo de vida via object pooling |

## Object Pooling (GP-038)
- `OrbController` usa `PoolManager` via DI para retornar ao pool após expiração.
- Pool tag padrão: `"Orb"`.
- `ReturnToPool()` chamado após delay de 0.5s para efeitos visuais.

## Referências
- GDD: `02_gameplay.md §2.1.1`, `06_physics.md §6.2`
- Backlog: `GP-001`, `GP-002`, `GP-003`, `GP-038`, `PH-001`, `PH-002`
