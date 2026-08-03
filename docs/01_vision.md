# 01. Visão Geral

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 1.1 Nome Provisório

**BREACH AR**

Alternativas consideradas: *Rift Defense AR*, *Home Front AR*, *Fracture*. Nome final a validar com pesquisa de marca/App Store ASO antes do Beta.

## 1.2 Conceito

Fendas dimensionais ("**Rifts**") começam a se abrir em superfícies reais do ambiente do jogador — paredes, teto, chão e móveis — detectadas via ARCore. Pequenas criaturas de energia chamadas **Fragments** emergem dessas fendas e avançam em direção a um núcleo de energia que o jogador deve proteger (o **Core**, projetado visualmente no centro do ambiente escaneado).

O jogador usa gestos de lançamento (arrastar e soltar na tela) para disparar **Orbes de Energia** contra os Fragments e contra as próprias Rifts, usando a física real do ambiente (ricochetes em paredes, quedas por gravidade, colisão com móveis) como parte da estratégia.

É um híbrido de **tower-defense reverso + arcade shooter físico**, ambientado literalmente na casa do jogador.

## 1.3 Público-Alvo

- **Primário:** jogadores casuais de 16–35 anos, familiarizados com jogos arcade/mobile (Angry Birds, Fruit Ninja, Beat Saber-like AR), interessados em novidade tecnológica.
- **Secundário:** early adopters de AR/tech enthusiasts que compram dispositivos compatíveis (ARCore Depth API) e gostam de mostrar experiências imersivas.
- **Terciário:** criadores de conteúdo (streamers/TikTok) — o jogo precisa ser visualmente "compartilhável" (clipes curtos, momentos de destruição espetaculares).

Perfil demográfico-alvo para monetização: casual spenders, sensíveis a cosméticos e conveniência, não pay-to-win.

## 1.4 Plataformas

- **Android** (foco primário) — via **ARCore**, Unity **AR Foundation**.
- **iOS** (foco secundário, pós-MVP) — via **ARKit**, mesmo pipeline AR Foundation, para reuso de ~90% do código.
- Sem versão desktop/console prevista.

## 1.5 Diferenciais

1. **Uso real do ambiente físico completo**: não apenas o chão (padrão da maioria dos jogos AR), mas paredes, teto e móveis como parte ativa do level design procedural.
2. **Física real com o Depth API**: oclusão real (o sofá esconde o inimigo atrás dele), ricochetes calculados contra a geometria escaneada real.
3. **IA generativa para variação de sessão**: cada partida gera padrões de Rifts e ondas de Fragments adaptados ao layout específico do cômodo do jogador (ver `05_ai.md`).
4. **Dificuldade dinâmica** que aprende o padrão de acerto/erro do jogador em tempo real.
5. **Sessões curtas (3–6 min)** pensadas para mobile-first, mas com profundidade de progressão de longo prazo (metagame de upgrades e cosméticos).

## 1.6 Loop Principal de Gameplay

```
1. Jogador escaneia o ambiente (onboarding AR)
        ↓
2. Sistema posiciona o Core + gera Rifts nas superfícies detectadas
        ↓
3. Onda de Fragments emerge e avança
        ↓
4. Jogador lança Orbes (arrastar + soltar) para destruir Fragments / fechar Rifts
        ↓
5. Combos e power-ups aumentam pontuação e utilidade ofensiva
        ↓
6. Dificuldade dinâmica ajusta intensidade da próxima onda
        ↓
7. Onda de boss (a cada N ondas) — Rift Maior com padrão de ataque
        ↓
8. Fim de sessão (Core destruído OU todas as ondas concluídas)
        ↓
9. Recompensas (moeda, XP, itens) → Progressão de meta-game
        ↓
(loop retorna ao passo 1 em nova sessão / mesmo ambiente)
```

## 1.7 Objetivos do Jogador

- **Curto prazo (por sessão):** sobreviver às ondas, proteger o Core, maximizar combo e pontuação.
- **Médio prazo (por semana):** completar desafios diários/semanais, subir no ranking local e global, desbloquear novos Orbes e skins.
- **Longo prazo (meses):** evoluir arsenal completo de Orbes, completar todos os biomas temáticos de Rifts, dominar o modo Endless com dificuldade máxima, completar o Battle Pass sazonal.

## Changelog

- 2026-08-03 — Criação inicial do documento.
