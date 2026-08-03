# CoreSystem

## Objetivo
Gerenciar o HP do Core (objeto a ser protegido) e determinar a condição de derrota do jogador.

## Responsabilidades
- Rastrear HP atual e máximo do Core.
- Processar dano recebido de Fragments que chegam ao Core.
- Aplicar cura e efeitos temporários (invulnerabilidade).
- Detectar destruição do Core e sinalizar Game Over.
- Fornecer dados para HUD (barra de HP, porcentagem).
- **Não** faz: spawn de Fragments (é `EnemySpawner`), nem cura via power-ups (é `PowerUpController`).

## Dependências
- `CoreDefinitionSO` (configuração: HP base, caps, regen).
- `ComboSystem` (pode afetar condições de vitória/derrota).
- `GameManager` (sinaliza Game Over quando Core é destruído).
- `HUDManager` (exibe HP na tela).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnCoreDamaged` | `CoreDamagedData` (damageAmount, currentHealth, maxHealth, sourceFragmentId) | Core recebe dano |
| `OnCoreHealed` | `CoreHealedData` (healAmount, currentHealth) | Core é curado |
| `OnCoreDestroyed` | `CoreDestroyedData` (finalHealth, cause) | HP chega a 0 |
| `OnCoreRevived` | `CoreRevivedData` (healthPercentage) | Core é revivido (se aplicável) |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnPowerUpCollected` | `PowerUpController` | Aplica cura ou invulnerabilidade ao Core |
| `OnWaveCompleted` | `EnemySpawner` | Pode aplicar regen parcial entre ondas |
| `OnGameOver` | `GameManager` | Desativa processamento de dano |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `CoreController` | MonoBehaviour: HP, dano, cura, invulnerabilidade |
| `CoreDefinitionSO` | Configuração: HP base, caps, regen rate |
| `HUDManager` | Exibe barra de HP e texto de saúde |

## Referências
- GDD: `02_gameplay.md §2.1.5`, `07_ui.md §7.2`
- Backlog: `GP-004`, `GP-043`, `UI-002`
