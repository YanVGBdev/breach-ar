# HUD

## Objetivo
Exibir informações essenciais durante o gameplay ativo, fornecendo feedback visual imediato ao jogador sobre estado do jogo, pontuação e ameaças.

## Responsabilidades
- Exibir HP do Core (barra de vida, texto, indicadores de cor).
- Mostrar pontuação atual e mudanças recentes (+delta).
- Exibir multiplicador de combo com progresso da janela.
- Mostrar progresso da onda atual.
- Criar indicadores de ameaça para Fragments off-screen.
- Exibir power-ups ativos e seus timers.
- Gerenciar menu de pausa com opções de jogo.
- Exibir resumo completo ao fim da sessão (Game Over).
- Gerenciar consentimento de privacidade (LGPD/GDPR).
- Gerenciar configurações do jogo (áudio, gráficos, controles).
- Navegar entre telas do menu principal.
- Selecionar modo de jogo.
- **Não** faz: lógica de combo (é `ComboSystem`), cálculo de score (é `ScoreSystem`).

## Dependências
- `CoreController` (HP atual para barra de vida).
- `ComboSystem` (multiplicador e contagem de combo).
- `ScoreSystem` (pontuação atual).
- `SessionStateMachine` (progresso da onda).
- `GameManager` (controle de pausa/resume, início de jogo).
- `ARSessionService` (rescan, verificação de compatibilidade).
- `EconomyService` (recompensas de sessão).
- `SaveService` (persistência de configurações).
- `AudioManager` (controle de volume).
- `AnalyticsService` (consentimento de analytics).
- `SceneLoader` (navegação entre cenas).
- `Camera` (projeção de world-to-screen para indicadores).
- `ThreatIndicatorUI` (prefab de indicador de ameaça).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnHUDVisibilityChanged` | `HUDVisibilityData` (isVisible) | HUD é mostrado/ocultado |
| `OnPauseToggled` | `PauseToggledData` (isPaused) | Jogo é pausado/resumido |
| `OnGameOver` | `GameOverData` (victory, score, waves, combo, fragments, rifts) | Sessão termina |
| `OnMenuOpened` | `MenuOpenedData` (menuName) | Menu é aberto |
| `OnMenuClosed` | `MenuClosedData` (menuName) | Menu é fechado |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnCoreDamaged` | `CoreController` | Atualiza barra de HP com animação |
| `OnComboChanged` | `ComboSystem` | Atualiza multiplicador e cor |
| `OnScoreChanged` | `ScoreSystem` | Atualiza pontuação e mostra delta |
| `OnWaveStarted` | `EnemySpawner` | Atualiza indicador de onda |
| `OnFragmentKilled` | `FragmentController` | Pode criar indicador de kill |
| `OnPowerUpCollected` | `PowerUpController` | Mostra ícone de power-up ativo |
| `OnRiftSpawned` | `RiftSpawnDirector` | Mostra indicador de ameaça se off-screen |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `HUDManager` | Controller principal: orquestra todos os elementos |
| `CoreHealthBar` | Barra de HP com animação de dano |
| `ScoreDisplay` | Texto de score com animação de delta |
| `ComboDisplay` | Multiplicador com cor e pulso |
| `WaveIndicator` | Texto de progresso da onda |
| `ThreatIndicatorUI` | Seta/diamond para ameaças off-screen |
| `PowerUpDisplay` | Ícones de power-ups ativos com timer |
| `PauseUI` | Menu de pausa com opções |
| `GameOverUI` | Tela de fim de jogo com resumo de run |
| `PrivacyConsentUI` | Dialog de consentimento LGPD/GDPR |
| `SettingsUI` | Tela de configurações (áudio, gráficos, controles, acessibilidade) |
| `MenuPrincipalUI` | Menu principal com navegação |
| `ModeSelectionUI` | Seleção de modo de jogo |

## Referências
- GDD: `07_ui.md §7.1`, `07_ui.md §7.2`, `07_ui.md §7.3`, `07_ui.md §7.4`, `07_ui.md §7.5`
- Backlog: `UI-001` a `UI-020`, `UI-026`
