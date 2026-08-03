# 13. Roadmap

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 13.1 MVP (Minimum Viable Product)

**Objetivo:** validar o core loop (scan → placement → onda → lançamento → combate → game over) em um dispositivo real, sem metagame nem monetização.

Escopo:
- AR: scan de chão + paredes, placement do Core, 1 tipo de Rift (parede).
- Gameplay: 1 tipo de Orbe, 2 tipos de Fragment, sistema de combo básico, sem power-ups, sem boss.
- UI: HUD mínimo, tela de Game Over simples, sem menu de configurações avançado.
- Sem backend (save local apenas), sem analytics, sem áudio final (placeholder).
- Critério de saída: 10 sessões de playtest interno completadas sem crash, core loop "divertido" validado subjetivamente.

## 13.2 Vertical Slice

**Objetivo:** demonstrar a experiência completa de ponta a ponta em um bioma único, com qualidade de arte/áudio representativa.

Escopo adicional sobre o MVP:
- Todos os tipos de superfície (chão, parede, teto, móveis) com Rifts funcionais.
- 3–4 tipos de Orbe (incluindo elementais), árvore de upgrade básica.
- Todos os power-ups (seção `02_gameplay.md` §2.9).
- 1 boss completo com FSM multi-fase.
- Sistema de dificuldade dinâmica funcional.
- Arte e áudio finais para 1 bioma completo.
- HUD e menus completos (exceto loja/monetização).
- Critério de saída: build jogável ponta a ponta, usada para validação externa (playtesters fora da equipe) e/ou pitch para publishers/investidores.

## 13.3 Alpha

**Objetivo:** feature-complete, conteúdo mínimo viável para todos os sistemas, foco em estabilidade.

Escopo adicional:
- Backend completo (save em nuvem, ranking, perfil).
- Analytics implementado.
- 3 biomas completos (arte/áudio).
- Modos Campanha, Endless e Zen funcionais.
- Sistema de economia (moeda soft) completo.
- Testes em múltiplos dispositivos (device matrix, `14_testing.md`).
- Sem monetização real ainda (placeholders de loja).
- Critério de saída: build interna estável, sem crashes críticos conhecidos, todos os sistemas do GDD implementados em ao menos versão mínima.

## 13.4 Beta

**Objetivo:** conteúdo completo, monetização ativa, preparação para lançamento.

Escopo adicional:
- Monetização completa (IAP, ads, Battle Pass temporada 1).
- Modo Desafio Diário.
- Todos os biomas planejados para lançamento.
- Testes de carga de backend, testes de compatibilidade de dispositivo em escala (closed/open beta via Google Play).
- Localização (mínimo: PT-BR, EN, ES).
- Critério de saída: soft launch em mercado de teste (ex: Filipinas/Canadá/Austrália conforme prática de indústria), métricas de retenção/monetização coletadas.

## 13.5 Release

**Objetivo:** lançamento global.

- Ajustes finais de balanceamento baseados em dados do soft launch.
- Marketing/ASO finalizados.
- Suporte a push notifications de retenção.
- Monitoramento ativo de crash/performance nas primeiras 2 semanas (resposta rápida a hotfixes).

## 13.6 Atualizações Futuras (Pós-Release)

- Novos biomas e tipos de Rift/Fragment (conteúdo sazonal contínuo via Battle Pass).
- Modo Multiplayer Assíncrono (Cloud Anchors).
- Suporte iOS/ARKit.
- Eventos comunitários recorrentes.
- Expansão de árvore de upgrades e novos tipos de Orbe.
- Avaliação de suporte a headsets AR dedicados (visão de longo prazo, não compromissada).

## Changelog

- 2026-08-03 — Criação inicial do documento.
