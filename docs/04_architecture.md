# 04. Arquitetura de Software

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 4.1 Organização do Projeto

- **Engine**: Unity 6 LTS, Render Pipeline: **URP** (Universal Render Pipeline) — necessário para o shader customizado de oclusão AR e para performance mobile.
- **Linguagem**: C# (.NET Standard 2.1).
- **Pacotes-chave**: AR Foundation, ARCore XR Plugin, Input System, Addressables, Cinemachine (câmera AR gerenciada), UniTask (async/await performático sem GC alloc excessivo), Zenject/VContainer (DI — ver 4.9).

## 4.2 Estrutura de Pastas

```
Assets/
  _Project/
    Scripts/
      Core/                # Bootstrapping, GameManager, ServiceLocator/DI
      AR/                  # Wrappers de AR Foundation, ScannedSurface, AnchorService
      Gameplay/
        Orbs/
        Fragments/
        Rifts/
        Combo/
        Core/               # "Core" do jogo (objeto a proteger) — não confundir com pasta Core acima
        Powerups/
        Bosses/
      AI/
        DifficultyDirector/
        ProceduralGeneration/
        EnemyBehaviour/
      Physics/
      UI/
        HUD/
        Menus/
        Screens/
      Audio/
      Analytics/
      Backend/
        Networking/
        Save/
      Utils/
      Tests/
        EditMode/
        PlayMode/
    Prefabs/
      Orbs/
      Fragments/
      Rifts/
      VFX/
      UI/
    ScriptableObjects/
      Configs/
      Balancing/
      Waves/
      Orbs/
    Art/
      Models/
      Materials/
      Shaders/
      Animations/
    Audio/
      Music/
      SFX/
      Ambience/
    Scenes/
      Boot.unity
      MainMenu.unity
      Gameplay.unity
  Plugins/
  StreamingAssets/
```

## 4.3 Padrões de Projeto

| Padrão | Uso |
|---|---|
| **ScriptableObject-based Architecture** | Configuração de dados (waves, orbes, dificuldade) desacoplada de código; permite tuning sem recompilar. |
| **Observer / Event Channels (SO Events)** | Comunicação entre sistemas independentes sem referências diretas (ver 4.7). |
| **State Machine** | Fluxo de jogo (MainMenu → Scanning → Playing → Paused → GameOver), comportamento de bosses, comportamento de Fragments. |
| **Object Pooling** | Fragments, Orbes, partículas — evita GC spikes em mobile. |
| **Strategy Pattern** | Tipos de Orbe (dano físico, elemental, área) implementam `IOrbBehaviour`. |
| **Command Pattern** | Ações do jogador (lançar Orbe, ativar power-up) encapsuladas para suportar replay/analytics/anti-cheat. |
| **Dependency Injection** | Serviços globais injetados via container (VContainer), não Singletons estáticos "mágicos". |
| **Repository Pattern** | Acesso a dados de save local e remoto abstraído atrás de interface (`ISaveRepository`). |

## 4.4 Componentes (exemplos-chave)

- `OrbController` (MonoBehaviour): física de voo, detecção de colisão, delega dano via `IDamageable`.
- `FragmentController`: FSM de comportamento (Spawn → Move → Attack → Death), implementa `IDamageable`, `IPathfindingAgent`.
- `RiftController`: gerencia Integridade, spawn timer de Fragments, estado visual (aberta/fechando/fechada).
- `CoreController`: gerencia HP do Core, dispara eventos de dano/Game Over.
- `ComboSystem`: escuta eventos de kill, gerencia multiplicador e timer de decaimento.

## 4.5 Sistemas Independentes

Cada sistema abaixo deve ser testável isoladamente (sem depender de cena AR real, via mocks de `IARSurfaceProvider`):

- **AR Session System** (abstrai AR Foundation)
- **Spawn System** (decide onde/quando spawnar Rifts e Fragments)
- **Combat System** (dano, colisões, morte)
- **Difficulty Director** (ver `05_ai.md`)
- **Economy System** (moedas, upgrades)
- **Save System**
- **Analytics System**
- **Audio System**

## 4.6 Gerenciadores Globais

Registrados no container de DI na cena `Boot`, injetados onde necessário (evitar `Singleton.Instance` estático):

- `GameStateManager` — máquina de estados macro do app.
- `SessionManager` — estado da run atual (onda, score, combo).
- `ARSessionService` — wrapper de AR Foundation.
- `AudioManager`
- `SaveService`
- `AnalyticsService`
- `EconomyService`
- `RemoteConfigService` (balanceamento remoto, ver `05_ai.md`)

## 4.7 Eventos

- **Event Channels via ScriptableObject** (`GameEvent`, `GameEvent<T>`) — desacoplam Producer/Listener sem referência direta em código, editáveis/depuráveis no Inspector.
- Exemplos: `OnFragmentKilled(FragmentData)`, `OnRiftClosed(RiftData)`, `OnCoreDamaged(float amount)`, `OnComboChanged(float multiplier)`, `OnWaveStarted(int waveIndex)`.
- Convenção de nomenclatura: `On<Sujeito><Ação>` para eventos; `Request<Ação>` para comandos que outro sistema deve executar.

## 4.8 Scriptable Objects

| SO | Propósito |
|---|---|
| `OrbDefinitionSO` | Dano base, velocidade, massa, efeito elemental, prefab, custo de upgrade. |
| `FragmentDefinitionSO` | HP, velocidade, dano ao Core, tipo de movimento, VFX de morte. |
| `WaveDefinitionSO` | Composição de onda (quais Fragments, quantidade, intervalo de spawn). |
| `DifficultyCurveSO` | Curvas de escalonamento (AnimationCurve) para HP/velocidade/spawn rate. |
| `BiomeThemeSO` | Paleta visual, música, tipos de Rift por bioma. |
| `PowerupDefinitionSO` | Efeito, duração, raridade, VFX. |
| `BossDefinitionSO` | Fases, weak points, padrões de ataque (referencia FSM data). |

## 4.9 Dependency Injection

- Biblioteca: **VContainer** (leve, performático em mobile, menor overhead que Zenject).
- `ProjectLifetimeScope` (raiz, cena Boot) registra serviços singleton (Audio, Save, Analytics, ARSessionService).
- `GameplayLifetimeScope` (cena Gameplay) registra serviços por-run (SessionManager, ComboSystem, SpawnSystem).
- Regra: nenhum `MonoBehaviour` de gameplay deve buscar serviços via `FindObjectOfType` ou Singleton estático — sempre injeção via `[Inject]`.

## 4.10 Máquina de Estados

### Macro (App-level) — `GameStateManager`
```
Boot → MainMenu → Scanning → Placement → Playing → Paused ⇄ Playing → GameOver → (MainMenu | Playing[retry])
```

### Gameplay (Session-level) — `SessionStateMachine`
```
WaveIntro → WaveActive → WaveCleared → (WaveIntro[próxima] | BossIntro → BossActive → BossCleared → WaveIntro | RunComplete)
                                     ↘ CoreDestroyed → RunFailed
```

### Fragment AI — `FragmentStateMachine`
```
Spawning → Seeking (pathfind até o Core) → Attacking (ao alcançar range do Core) → Dead
                     ↘ Staggered (ao sofrer dano crítico, breve stun) ↗
```

Implementação recomendada: FSM leve própria (interface `IState` + `StateMachine<T>` genérica) em vez de framework pesado externo, para manter controle total de performance em mobile.

## 4.11 Architecture Decision Records (ADR)

Toda decisão estrutural relevante (troca de padrão, biblioteca, pipeline) deve ser registrada como ADR em `/docs/adr/NNN-titulo.md` com formato: Contexto → Decisão → Consequências. Agentes de IA implementando features não devem alterar decisões de ADRs `Accepted` sem propor um novo ADR substituto.

## Changelog

- 2026-08-03 — Criação inicial do documento.
