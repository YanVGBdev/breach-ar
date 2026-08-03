# 08. Áudio

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 8.1 Música

- Trilha adaptativa (**vertical layering**): camadas de instrumentação se somam conforme intensidade da onda/combo sobe (ex: base ambiente → +percussão a partir de combo x2 → +sintetizador tenso em fase de boss).
- Uma trilha temática por bioma visual (`BiomeThemeSO`), com transição crossfade de 2s ao trocar de bioma.
- Música de menu distinta, mais calma, loop longo (evitar repetição perceptível < 3 min).
- Implementação via **FMOD** (recomendado sobre AudioMixer nativo puro) para parametrização dinâmica (intensidade, combo) sem necessidade de recompilar áudio a cada ajuste — desenvolvedor solo se beneficia da iteração rápida em ferramenta externa.

## 8.2 Sons (SFX)

| Categoria | Exemplos |
|---|---|
| Lançamento | Whoosh de Orbe, carregamento de Overcharge |
| Impacto | Colisão Orbe-Fragment, Orbe-parede (variação por material: madeira/tecido/concreto), Orbe-Rift |
| Morte de inimigo | Explosão de Fragment comum/elite (variação por tipo) |
| Rift | Abertura, fechamento (implosão), dano |
| Core | Dano recebido (escalona intensidade com % de HP restante), destruição (Game Over) |
| Combo | Stinger a cada +1.0 no multiplicador, som de quebra de combo |
| Power-up | Coleta, ativação, expiração |
| UI | Toque de botão, transição de tela, notificação de recompensa |
| Boss | Rugido/aparição, ataques por padrão, weak point destruído, derrota |

- Pool de variações (3–4 por evento comum) para evitar repetição perceptível em spam de ações (ex: múltiplos impactos por segundo).

## 8.3 Áudio Espacial

- **Spatial Audio 3D** via `AudioSource.spatialBlend = 1` para todos os SFX de gameplay (Fragments, Rifts, impactos) — essencial em AR: o jogador deve **ouvir de qual direção física a ameaça vem**, inclusive fora do campo de visão da câmera.
- HRTF (Head-Related Transfer Function) habilitado via plugin de áudio espacial (Google Resonance Audio ou Steam Audio, avaliar licenciamento) para simular oclusão sonora por "paredes reais" (efeito de abafamento quando a fonte está atrás de uma superfície detectada).
- Áudio de UI e música permanecem 2D (`spatialBlend = 0`), não afetados por posição da câmera.

## 8.4 Feedback Sonoro

- Cada ação do jogador tem resposta sonora imediata (< 50ms de latência perceptível) — crítico para sensação de "game feel" responsivo em mobile.
- Feedback sonoro de alerta direcional (som sutil e distinto) acompanha o indicador visual de ameaça fora de tela (`07_ui.md` §7.1).
- Hierarquia de mixagem: SFX de gameplay > alertas de UI > música, com **ducking** automático da música durante eventos críticos (boss intro, Game Over).

## Changelog

- 2026-08-03 — Criação inicial do documento.
