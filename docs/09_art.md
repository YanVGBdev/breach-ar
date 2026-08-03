# 09. Arte

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 9.1 Direção Artística

Conceito visual: **"energia digital invadindo o mundo físico"** — contraste forte entre o ambiente real (câmera passthrough, sem estilização) e os elementos virtuais (estética low-poly emissiva/holográfica, translúcida, com bordas luminosas nítidas). Isso reforça legibilidade (jogabilidade clara mesmo sobre fundos reais caóticos) e resolve o desafio de composição AR (elementos precisam "pop" contra qualquer ambiente).

## 9.2 Estilo Gráfico

- **Low-poly estilizado + shading emissivo (fresnel/rim light)** — barato de renderizar em mobile e visualmente distinto do mundo real.
- Silhuetas simples e legíveis (importante: jogador reconhece tipo de Fragment à distância/periferia).
- Sem texturas fotorrealistas — prioriza shaders proceduais (gradientes, ruído, fresnel) para reduzir footprint de memória de textura.

## 9.3 Paleta de Cores

- **Rifts/Fragments comuns**: tons de violeta/ciano (energia "hostil" mas não agressiva).
- **Fragments elite**: acento em magenta/vermelho para destaque de ameaça.
- **Orbes do jogador**: paleta quente (âmbar/dourado) por padrão, com variações elementais (azul = gelo, vermelho = fogo, verde = veneno/corrosão, roxo = energia pura) desbloqueáveis.
- **Core**: azul-branco puro (elemento "aliado" claramente distinto de tudo hostil).
- **UI**: paleta neutra escura translúcida (glassmorphism leve) para não competir visualmente com o mundo real captado pela câmera.

## 9.4 Modelos

- Budget de poly por Fragment comum: ≤ 800 tris; elite: ≤ 1500 tris; boss: ≤ 8000 tris (LOD0).
- Sistema de **LOD** (LOD0/LOD1/LOD2) obrigatório para bosses e Rifts (objetos grandes/próximos da câmera por mais tempo).
- Orbes: geometria simples (esfera estilizada + shader), poly count irrelevante (< 200 tris).
- Todos os modelos exportados em FBX, escala real-world (1 unidade Unity = 1 metro) — crítico para consistência de escala em AR.

## 9.5 Animações

- Fragments: idle/seek (loop), attack, death (dissolve), staggered (hit reaction).
- Bosses: por fase — intro, idle, ataque(s) específico(s) por padrão, weak point exposed/destroyed, death sequence (mais elaborada, "climática").
- Rifts: idle (pulsação sutil), spawn-fragment (breve "cuspir"), damaged (shake), closing (implosão).
- Rig leve (poucos bones) para animações de Fragments — priorizar shader-driven animation (vertex displacement) sobre skeletal quando possível, para reduzir custo de CPU skinning em mobile.

## 9.6 VFX

- VFX Graph (GPU) para: explosões de morte, implosão de Rift, trilhas de Orbe, impacto de ricochete, ativação de power-up.
- Todos os VFX devem respeitar o shader de oclusão AR customizado (`03_ar_system.md` §3.5) — nenhum efeito pode "vazar" através de superfícies reais de forma inconsistente.
- Sistema de **qualidade escalável de VFX** por Device Tier (`16_optimization.md`): reduz contagem de partículas e desliga efeitos secundários (ex: sparks decorativos) em tiers baixos.

## 9.7 UI (Visual)

- Estilo: painéis translúcidos com leve blur de fundo (glassmorphism), ícones flat com contorno luminoso sutil consistente com a estética "energia digital".
- Tipografia: fonte sans-serif geométrica, alta legibilidade em telas pequenas e sobre fundos variados (contorno/sombra em todo texto sobreposto à câmera AR).
- Iconografia consistente por categoria (combate, economia, progressão) definida em um Style Guide de ícones único (arquivo fonte Figma referenciado no repositório de assets).

## Changelog

- 2026-08-03 — Criação inicial do documento.
