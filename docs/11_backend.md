# 11. Backend

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 11.1 Visão Geral da Stack

Recomendação para desenvolvedor solo: **BaaS gerenciado** em vez de infraestrutura própria, para minimizar overhead operacional.

- **Provedor recomendado**: Supabase (Postgres + Auth + Edge Functions) OU Firebase (Firestore + Auth + Cloud Functions) — decisão a formalizar via ADR (`04_architecture.md` §4.11). Este documento assume Supabase como padrão de referência por afinidade com stack já validada pelo desenvolvedor.
- **Autenticação**: anônima por padrão (login automático no primeiro acesso) com opção de vincular Google Play Games / Game Center para portabilidade de progresso entre dispositivos.

## 11.2 Ranking (Leaderboards)

- Tabelas: `leaderboard_campaign`, `leaderboard_endless`, `leaderboard_daily_challenge` (particionada por data da seed diária).
- Cada submissão de score inclui: `player_id`, `score`, `wave_reached`, `max_combo`, `timestamp`, `run_signature` (hash anti-cheat, ver `17_security.md`).
- Validação server-side antes de aceitar submissão (Edge Function): checagem de plausibilidade (score máximo teórico por tempo de sessão, taxa de eventos) antes de persistir.
- Cache local com sincronização a cada abertura de tela de ranking + após submissão de novo score.

## 11.3 Salvamento (Save System)

- **Save local primário** (funcionamento offline garantido): arquivo criptografado local (`ISaveRepository` local implementation) contendo progresso, economia, configurações.
- **Save em nuvem (sync)**: espelha save local no backend a cada mudança relevante de estado (debounced, não a cada frame) + ao pausar/fechar o app; usado para restauração em troca de dispositivo.
- Estratégia de conflito: **last-write-wins com timestamp**, com merge simples para contadores cumulativos (moeda, XP) preferindo o maior valor em caso de divergência (evita perda de progresso por race condition).

## 11.4 Estatísticas

- Endpoint agregado (Edge Function) para estatísticas globais exibidas ao jogador (ex: "Fragments destruídos pela comunidade hoje" — usado no Evento Comunitário, `02_gameplay.md` §2.11).
- Estatísticas pessoais completas (histórico de sessões, melhores runs) armazenadas por `player_id`, consultáveis pela tela de Perfil.

## 11.5 Perfil do Jogador

- Tabela `players`: `id`, `display_name`, `level`, `xp`, `created_at`, `last_seen_at`, `platform`, `device_tier` (para segmentação de analytics/otimização).
- Tabela `player_inventory`: itens possuídos (Orbes desbloqueados, skins, upgrades) — fonte de verdade para o que o cliente pode usar (nunca confiar apenas no save local para desbloqueios de IAP).
- Tabela `player_economy`: saldo de moeda soft/hard — **saldo de moeda hard é sempre validado/atualizado server-side** em transações de IAP (nunca incrementado apenas localmente).

## 11.6 Remote Config

- Tabela/serviço de configuração remota para: curvas de dificuldade, frequência de anúncios, parâmetros de economia, feature flags (rollout gradual de novas features/eventos sazonais).
- Cliente busca config no boot (com fallback para valores default embutidos caso offline).

## Changelog

- 2026-08-03 — Criação inicial do documento.
