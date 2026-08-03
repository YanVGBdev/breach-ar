# ARSurfaceService

## Objetivo
Gerenciar o scan, classificação e ancoragem de superfícies do mundo real para posicionamento de Rifts e outros elementos de gameplay.

## Responsabilidades
- Iniciar e gerenciar sessão AR Foundation.
- Detectar e classificar planos do mundo real (chão, parede, teto, móvel).
- Fornecer superfícies válidas para spawn de Rifts.
- Gerenciar anchors AR para ancoragem de objetos.
- Validar qualidade das superfícies (área mínima, estabilidade).
- Exibir feedback visual de progresso durante scan.
- **Não** faz: spawn de Rifts (é `RiftSpawnDirector`), nem física de Orbs (é `LaunchSystem`).

## Dependências
- AR Foundation / ARCore (SDK de AR).
- `ARSessionConfig` (configuração: timeout, área mínima, planos mínimos).
- `DeviceTierDetector` (qualidade do scan baseado no tier do dispositivo).

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|
| `OnSurfaceDetected` | `SurfaceDetectedData` (surfaceId, type, area, position) | Nova superfície detectada |
| `OnSurfaceLost` | `SurfaceLostData` (surfaceId) | Superfície perde rastreamento |
| `OnScanComplete` | `ScanCompleteData` (surfaceCount, duration, hasFloor, hasWall) | Scan atinge mínimo ou timeout |
| `OnAnchorCreated` | `AnchorCreatedData` (anchorId, position, surfaceId) | Novo anchor AR criado |

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|
| `OnSessionStarted` | `GameManager` | Inicia scan AR |
| `OnRescanRequested` | `UI` | Reinicia scan |
| `OnRiftSpawned` | `RiftSpawnDirector` | Registra uso de anchor |

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|
| `ARSessionService` | Wrapper de AR Foundation: sessão, scan, classificação |
| `ScannedSurface` | Dados de superfície detectada (tipo, área, posição, normal) |
| `ARDeviceCapability` | Resultado de capability check do dispositivo |
| `ARAnchor` | Ancoragem de objetos no mundo real |
| `ARScanUI` | Interface de feedback visual durante scan (barra de progresso, indicadores de superfície) |

## Referências
- GDD: `03_ar_system.md §3.1`, `03_ar_system.md §3.2`, `03_ar_system.md §3.3`
- Backlog: `AR-001`, `AR-002`, `AR-003`, `AR-004`
