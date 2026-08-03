# 06. Física

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 6.1 Sistema de Lançamento

- Input via `Input System` (touch drag): ponto de início (posição do dispenser virtual, fixo relativo ao Core) e ponto de soltura definem vetor.
- Fórmula de força: `launchForce = clamp(dragVector.magnitude, minDrag, maxDrag) * ForceMultiplier` (definido em `LaunchConfigSO`).
- Aplicado via `Rigidbody.AddForce(direction * launchForce, ForceMode.VelocityChange)` para resposta consistente independente de frame rate.
- Trajetória previsualizada calculada por simulação analítica (equação de tiro parabólico) considerando a gravidade real ajustada (ver 6.2), renderizada como linha pontilhada com `LineRenderer`.
- Limite de lançamentos por segundo (cooldown mínimo de 0.3s) para evitar spam e preservar performance de física.

## 6.2 Gravidade

- Gravidade padrão do mundo Unity (`Physics.gravity = (0, -9.81, 0)`) escalada por um fator de jogo (`GravityScale`, default 0.6) para dar sensação "arcade" mais controlável em espaços pequenos (quartos, salas).
- `GravityScale` configurável por `OrbDefinitionSO` — Orbes "pesados" (dano alto) têm gravidade maior; Orbes "leves"/aéreos têm gravidade reduzida.

## 6.3 Colisões

- Camadas de física (`Layers`): `Orb`, `Fragment`, `RealWorldSurface` (mesh de colisão gerada a partir dos `ARPlane`), `Core`, `Powerup`.
- Matriz de colisão: `Orb` colide com `Fragment`, `RealWorldSurface`, `Rift`; `Fragment` colide com `RealWorldSurface` e `Core`; `Powerup` colide apenas com `Orb`/toque direto.
- Mesh de colisão de superfícies reais gerada dinamicamente via `ARPlaneMeshVisualizer`-like approach: `MeshCollider` (convex quando necessário) atualizado a cada mudança relevante de boundary do plano (throttled a cada 0.5s para performance).
- Para objetos não planares (móveis irregulares), fallback usa **Depth API raycast** pontual no momento da colisão prevista (ver 3.4) em vez de collider físico completo — evita custo de gerar mesh collider complexo em tempo real.

## 6.4 Ricochetes

- Material físico customizado (`PhysicMaterial`) por tipo de superfície: `Wall` (bounciness alta ~0.7), `Furniture` (bounciness média ~0.4), `Floor` (bounciness baixa ~0.2, mais fricção).
- Número máximo de ricochetes por Orbe antes de "expirar": 3 (configurável por `OrbDefinitionSO`), após o qual o Orbe se dissipa com VFX.
- Cada ricochete pode reduzir levemente o dano do Orbe (`damageFalloffPerBounce`), incentivando uso tático em vez de spam de ricochete infinito.

## 6.5 Destruição

- Fragments ao morrer: desabilita collider imediatamente, dispara VFX de partículas + animação de dissolução (shader dissolve), retorna ao pool após 1.5s.
- Rifts ao fechar: sequência de animação de "implosão" (shader + partículas convergentes) de 1s, depois destrói o `ARAnchor` associado.
- Móveis/superfícies reais **nunca são destruídos** (são o mundo real) — apenas recebem feedback visual de impacto (decals temporários, partículas de "faísca") para reforçar a sensação de física real sem implicar dano ao ambiente do jogador.

## 6.6 Partículas

- Sistema: Unity VFX Graph (GPU particles) para efeitos de alto volume (explosões, implosões de Rift); Shuriken (`ParticleSystem`) para efeitos simples e baratos (trilha de Orbe, faísca de impacto).
- Budget de partículas simultâneas definido em `16_optimization.md` (Device Tier) — sistema de prioridade que reduz densidade de partículas em dispositivos de tier baixo.
- Pool dedicado por tipo de efeito (evitar `Instantiate/Destroy` em runtime).

## 6.7 Objetos Interativos

- **Power-ups em queda**: Rigidbody com gravidade reduzida (float suave), colisor de "captura" maior que o visual para facilitar toque em mobile.
- **Weak points de boss**: colliders independentes no boss, cada um com seu próprio HP, expostos/ocultos conforme fase da FSM do boss.
- **Elementos de cenário reagentes** (pós-MVP): pequenos objetos decorativos que reagem a ricochetes próximos (leve shake visual) sem afetar gameplay — puramente cosmético/imersivo.

## Changelog

- 2026-08-03 — Criação inicial do documento.
