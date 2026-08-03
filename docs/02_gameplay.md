# 02. Gameplay

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 2.1 Mecânicas Principais

### 2.1.1 Lançamento de Orbes (Core Mechanic)
- Input: toque e arraste ("slingshot") a partir de um dispenser virtual ancorado próximo ao Core.
- Vetor de força = direção e distância do arraste (clamp configurável via `LaunchConfig` SO).
- Trajetória previsualizada com linha pontilhada (assistida por gravidade real do Rigidbody).
- Soltar o dedo = lançamento; física real (`06_physics.md`) governa o resto.

### 2.1.2 Defesa do Core
- O Core é um objeto virtual ancorado no centro do ambiente escaneado (ponto médio dos planos detectados).
- Cada Fragment que alcança o Core reduz a vida do Core (ver Sistema de Vidas).
- Perda total de vida do Core = Game Over.

### 2.1.3 Fechamento de Rifts
- Rifts são ancoradas (`ARAnchor`) em superfícies verticais (paredes), horizontais (chão/teto) ou em móveis (via detecção de planos secundários).
- Cada Rift possui uma barra de "Integridade". Acertos diretos de Orbes reduzem a Integridade.
- Ao chegar a 0, a Rift se fecha (para de spawnar Fragments) com efeito visual de implosão.

### 2.1.4 Combos
- Acertos consecutivos sem falha (multi-kill em um único Orbe via ricochete, ou kills em sequência dentro de uma janela de tempo) aumentam o multiplicador.
- Ver seção 2.3 (Sistema de Combos).

## 2.2 Mecânicas Secundárias

- **Ricochete estratégico**: usar paredes reais para atingir Fragments atrás de cobertura (oclusão).
- **Fragmentação em cadeia**: certos Fragments explodem e danificam vizinhos (reação em cadeia).
- **Captura de Power-ups em queda**: power-ups derrubados por Fragments precisam ser "pegos" com um segundo Orbe ou gesto de toque antes de expirarem.
- **Overcharge do Core**: segurar o toque no Core carrega um disparo de área (uso limitado por cooldown).
- **Interação com móveis reais**: usar mesas/sofás como "cobertura" que bloqueia Fragments terrestres, forçando rotas alternativas (pathfinding dos inimigos reage à geometria real).

## 2.3 Sistema de Pontuação

| Ação | Pontos base |
|---|---|
| Fragment comum destruído | 100 |
| Fragment elite destruído | 250 |
| Multi-kill (por alvo extra no mesmo Orbe) | +50 cada |
| Rift fechada | 500 |
| Boss derrotado | 5000 |
| Power-up coletado | 25 |
| Onda perfeita (sem dano ao Core) | +1000 bônus |

Pontuação final = soma dos eventos × multiplicador de combo ativo no momento do evento.

## 2.4 Sistema de Combos

- Multiplicador inicia em `x1.0`, incrementa `+0.1` a cada acerto dentro de uma janela de **2.5s** desde o último acerto.
- Multiplicador máximo: `x5.0`.
- Combo quebra (reset para `x1.0`) se:
  - Nenhum acerto em 2.5s, OU
  - Um Fragment atinge o Core.
- Feedback visual/sonoro escalonado a cada +1.0 no multiplicador (ver `08_audio.md`).

## 2.5 Progressão

### Meta-game (entre sessões)
- **XP de Conta**: sobe nível de perfil, desbloqueia slots de loadout.
- **Moeda Soft (Fragmentos de Energia)**: comprada com gameplay, usada para upgrades de Orbes.
- **Moeda Hard (Cristais)**: comprada com dinheiro real ou ganha raramente, usada para cosméticos/premium.
- **Árvore de Upgrades por Orbe**: dano, velocidade, tamanho de área, efeitos elementais (ver `06_physics.md` para tipos de Orbe).

### Progressão por sessão (in-run)
- Onda 1–N com dificuldade crescente.
- Power-ups temporários coletados durante a run não persistem entre sessões (roguelite-lite).

## 2.6 Modos de Jogo

1. **Modo Campanha (Waves)**: sequência fixa de ondas com boss ao final, ambientado em "biomas" temáticos de Rift.
2. **Modo Endless**: ondas infinitas com dificuldade dinâmica crescente, foco em ranking (leaderboard global).
3. **Modo Desafio Diário**: seed fixa (mesmo padrão de Rifts/ondas para todos os jogadores no dia), ranking comparativo justo.
4. **Modo Zen/Prático** (não pontua no ranking): sem Game Over, para o jogador testar Orbes e explorar o próprio ambiente.
5. **Modo Multiplayer Assíncrono (pós-MVP)**: jogador desafia "fantasma" de outro jogador (replay de inputs) no próprio ambiente.

## 2.7 Sistema de Dificuldade Dinâmica

Ver detalhamento algorítmico em `05_ai.md` (seção IA para Dificuldade). Resumo:
- Métricas monitoradas em tempo real: taxa de acerto, tempo de reação, dano recebido pelo Core, uso de power-ups.
- Ajustes possíveis: velocidade dos Fragments, frequência de spawn, quantidade de Rifts simultâneas, agressividade de bosses.
- Ajuste é gradual (não abrupto) para evitar frustração — máximo de ±15% por onda.

## 2.8 Sistema de Vidas

- Core possui **100 pontos de Integridade** (escalável por upgrades de meta-game, cap em 150).
- Fragment comum causa 5–10 de dano ao alcançar o Core; elite 15–25; boss ataques especiais 20–40.
- Sem "vidas" tradicionais (não é sistema de tentativas) — é sistema de HP único do Core por sessão.
- Modo Campanha permite 1 "Revive" gratuito por dia (assista anúncio ou gaste Cristais) — ver `10_monetization.md`.

## 2.9 Power-ups

| Power-up | Efeito | Duração |
|---|---|---|
| Orbe Múltiplo | Próximos 3 lançamentos disparam 3 orbes em leque | Até uso |
| Fenda Temporal | Reduz velocidade de todos os Fragments em 50% | 8s |
| Escudo do Core | Absorve os próximos 3 impactos ao Core | Até uso |
| Sobrecarga | Próximo Orbe causa dano em área (explosão) | 1 uso |
| Ímã de Energia | Orbes atraem Fragments próximos à trajetória | 10s |

## 2.10 Bosses

- Aparecem a cada 5 ondas (Campanha) ou a cada 8 ondas (Endless).
- **Rift Maior (Boss)**: Rift gigante ancorada na maior superfície plana detectada, com múltiplos pontos fracos (weak points) que devem ser destruídos em sequência.
- Padrões de ataque variam por bioma (ver `09_art.md` para temas visuais); comportamento definido via Máquina de Estados (`04_architecture.md`).
- Fase final do boss: exige combo ativo mínimo de x3.0 para causar dano crítico (incentiva domínio da mecânica de combo).

## 2.11 Eventos Especiais

- **Evento de Enxame**: onda extra com número massivo de Fragments fracos (foco em multi-kill/combo).
- **Evento Sazonal**: temas visuais e Orbes exclusivos por tempo limitado (Halloween, Ano Novo, etc.), atrelado ao Battle Pass.
- **Evento Comunitário**: meta global (soma de Fragments destruídos por todos os jogadores) libera recompensa para todos.
- **Rift Rara (Golden Rift)**: chance pequena por sessão de spawnar uma Rift dourada com recompensa de moeda hard.

## Changelog

- 2026-08-03 — Criação inicial do documento.
