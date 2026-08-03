# 03. Realidade Aumentada

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 3.1 Como o Ambiente Será Escaneado

Fluxo de onboarding AR (primeira execução em um novo ambiente):

1. **Tela de instrução**: pedir ao jogador para mover o dispositivo lentamente, apontando para paredes, chão e móveis.
2. **Scanning ativo**: `ARPlaneManager` acumula planos horizontais e verticais em background; UI mostra malha de pontos (feedback visual de progresso de scan).
3. **Critério de scan mínimo**: pelo menos 1 plano horizontal (chão) com área ≥ 2m² e 1 plano vertical (parede) com área ≥ 1.5m², OU 8 segundos de scan contínuo com ao menos 1 plano válido (fallback para ambientes pequenos).
4. **Confirmação de posicionamento do Core**: jogador toca no chão detectado para ancorar o Core.
5. **Geração procedural de Rifts** nas superfícies válidas ao redor do Core (ver `05_ai.md`).
6. Scan pode ser refeito a qualquer momento via botão "Rescan" no menu de pausa.

## 3.2 Uso do ARCore

- Engine: **Unity 6 LTS** + **AR Foundation** (abstração cross-platform) + provider **ARCore XR Plugin** (Android) / **ARKit XR Plugin** (iOS, pós-MVP).
- Features ARCore utilizadas: Plane Detection, Depth API, Anchors, Light Estimation, Cloud Anchors (opcional, multiplayer futuro), Augmented Faces (não utilizado).
- Versão mínima ARCore: **1.42+** (suporte a Depth API em mais dispositivos).

## 3.3 Detecção de Planos

- `ARPlaneManager` configurado para `PlaneDetectionMode.HorizontalAndVertical`.
- Planos horizontais classificados por `PlaneAlignment`: `HorizontalUp` (chão/mesas) vs `HorizontalDown` (teto — requer heurística adicional de altura relativa à câmera inicial, pois ARCore nem sempre distingue teto corretamente).
- Planos pequenos (< 0.3 m²) são ignorados para evitar spawns em superfícies inúteis (ex: livros, objetos pequenos).
- Sistema de **classificação semântica de planos** (heurística própria, ver `ScannedSurface` em `04_architecture.md`):
  - `Floor`: maior plano horizontal-up, próximo à altura inicial da câmera - offset.
  - `Ceiling`: plano horizontal-down OU horizontal-up muito acima da câmera.
  - `Wall`: plano vertical.
  - `Furniture`: plano horizontal-up pequeno/médio, elevado do chão (mesa, sofá, prateleira).

## 3.4 Detecção de Profundidade (Depth API)

- `AROcclusionManager` com `EnvironmentDepthMode.Fastest` (fallback para dispositivos não suportados) ou `.Best` (dispositivos high-end, detectado via `Device Capability Tier`, ver `16_optimization.md`).
- Depth texture usada para:
  1. **Oclusão em tempo real** (seção 3.5).
  2. **Raycast físico contra geometria real** para colisões de Orbes com objetos não mapeados como plano (ex: cadeiras, plantas).
  3. **Pathfinding de Fragments** — evitar que criaturas atravessem objetos sólidos não planares.

## 3.5 Oclusão

- Shader de oclusão baseado em `AROcclusionManager.humanStencilTexture` (não usado — sem detecção de pessoas neste jogo) e `environmentDepthTexture` para occlusion geral.
- Todos os materiais de Fragments, Orbes e VFX usam o shader customizado `URP/AROcclusion` que compara depth da cena real com depth da geometria virtual (ver `09_art.md` para pipeline de shaders).
- Fallback sem Depth API: usar apenas oclusão baseada em planos (occlusion mesh gerada a partir dos `ARPlane` detectados) — qualidade inferior mas funcional em dispositivos sem suporte a Depth.

## 3.6 Anchors

- `ARAnchor` usado para:
  - Core (anchor fixo no plano de chão escolhido).
  - Cada Rift (anchor individual por superfície).
  - Power-ups em queda (anchor temporário no ponto de "pouso" após física).
- Anchors não utilizadas por > 60s (ex: Rift fechada) são liberadas (`Destroy`) para evitar acúmulo de overhead de tracking.
- **Cloud Anchors** (Google ARCore Cloud Anchor API) reservados para o modo Multiplayer Assíncrono pós-MVP — permitem compartilhar posição do ambiente entre dispositivos diferentes no mesmo local físico.

## 3.7 Light Estimation

- `ARCameraManager.currentLightEstimation` usado para ajustar:
  - Intensidade e cor da luz direcional principal da cena (`Directional Light` sincronizada com `AverageBrightness` / `ColorCorrection`).
  - Ambient probes via `AREnvironmentProbeManager` (reflexos realistas de Orbes metálicos/brilhantes nos VFX).
- Objetivo: Fragments e Rifts devem parecer "iluminados pelo ambiente real" para reforçar a sensação de presença física.

## 3.8 Uso de Paredes, Teto, Chão e Móveis

| Superfície | Uso no gameplay |
|---|---|
| **Chão** | Ancoragem do Core; spawn de Fragments terrestres; Rifts de chão (menor frequência, mais fáceis de mirar). |
| **Paredes** | Principal fonte de Rifts (maior frequência); superfícies de ricochete para Orbes. |
| **Teto** | Rifts aéreas (spawnam Fragments voadores); requer lançamento em arco alto. |
| **Móveis** | Rifts "elite" com bônus de pontuação (mais difíceis de mirar por ângulo); cobertura tática para Fragments terrestres (oclusão de gameplay). |

## 3.9 Requisitos Mínimos do Dispositivo

| Requisito | Mínimo | Recomendado |
|---|---|---|
| SO | Android 9 (API 28) | Android 12+ |
| Compatibilidade | Certificado ARCore ([lista oficial Google](https://developers.google.com/ar/devices)) | Idem + suporte a Depth API |
| RAM | 4 GB | 6 GB+ |
| GPU | Adreno 530 / Mali-G71 ou superior | Adreno 660+ / Mali-G78+ |
| Sensores | Giroscópio, acelerômetro | + sensor de profundidade dedicado (ToF) |
| Espaço livre | 500 MB | 1 GB |
| Conectividade | Não obrigatória para modo offline; necessária para ranking/IAP | Wi-Fi/4G+ estável |

- Dispositivos sem suporte a Depth API rodam em **AR Compatibility Mode**: oclusão simplificada baseada em planos, sem raycast de profundidade fina (ver fallback em 3.5).
- Checagem de compatibilidade via `ARCoreSessionSubsystem` no boot; se indisponível, exibir tela de "dispositivo não suportado" com lista de alternativas.

## Changelog

- 2026-08-03 — Criação inicial do documento.
