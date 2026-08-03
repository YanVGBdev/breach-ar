# BREACH AR — Documentação Oficial do Projeto

> Nome provisório: **BREACH AR**
> Tagline: *"Sua casa é o campo de batalha."*

Este repositório contém a fonte única de verdade (Single Source of Truth) para o desenvolvimento de **BREACH AR**, um jogo mobile de Realidade Aumentada com mecânicas de lançamento físico, defesa de território e progressão orientada por IA.

A documentação foi escrita para ser consumida tanto por um desenvolvedor humano solo quanto por agentes de IA (LLMs) atuando como pair-programmers, geradores de código, geradores de assets e QA automatizado. Cada arquivo é modular e autocontido, mas referencia os demais via links relativos.

## Como usar esta documentação com agentes de IA

1. Sempre forneça ao agente o arquivo `01_vision.md` como contexto raiz antes de qualquer tarefa.
2. Para tarefas de implementação, forneça o arquivo de domínio relevante (`03_ar_system.md`, `06_physics.md`, etc.) + `04_architecture.md`.
3. Para geração de tarefas/sprints, use `15_backlog.md` como fonte de verdade de escopo — não invente tarefas fora dele sem atualizar o backlog.
4. Nunca altere a arquitetura (`04_architecture.md`) sem propor um ADR (Architecture Decision Record) — ver seção correspondente.
5. Todo código gerado deve respeitar os padrões de projeto e nomenclatura definidos em `04_architecture.md`.

## Índice de Arquivos

| Arquivo | Conteúdo |
|---|---|
| [01_vision.md](01_vision.md) | Visão geral, conceito, público-alvo, diferenciais |
| [02_gameplay.md](02_gameplay.md) | Mecânicas, progressão, modos, dificuldade |
| [03_ar_system.md](03_ar_system.md) | ARCore, planos, profundidade, oclusão, anchors |
| [04_architecture.md](04_architecture.md) | Arquitetura de software, padrões, pastas |
| [05_ai.md](05_ai.md) | Uso de IA em conteúdo, balanceamento, inimigos |
| [06_physics.md](06_physics.md) | Sistema de física, lançamento, colisões |
| [07_ui.md](07_ui.md) | HUD, menus, telas |
| [08_audio.md](08_audio.md) | Música, SFX, áudio espacial |
| [09_art.md](09_art.md) | Direção de arte, estilo, VFX |
| [10_monetization.md](10_monetization.md) | Modelo de negócio, IAP, ads, battle pass |
| [11_backend.md](11_backend.md) | Backend, ranking, save, perfil |
| [12_analytics.md](12_analytics.md) | Eventos, métricas, retenção |
| [13_roadmap.md](13_roadmap.md) | MVP → Release → pós-lançamento |
| [14_testing.md](14_testing.md) | Estratégia de QA e testes |
| [15_backlog.md](15_backlog.md) | Backlog completo (300+ tarefas) |
| [16_optimization.md](16_optimization.md) | Performance, memória, bateria |
| [17_security.md](17_security.md) | Anti-cheat, validação, integridade |
| [18_risks.md](18_risks.md) | Riscos e mitigação |

## Convenções do Repositório

```
/docs                  → esta documentação
/src (futuro)           → código-fonte Unity
  /Assets
    /_Project
      /Scripts
      /Prefabs
      /ScriptableObjects
      /Scenes
      /Art
      /Audio
```

## Versionamento da Documentação

- Cada arquivo possui um cabeçalho `Status: Draft | Review | Approved` e `Última atualização`.
- Mudanças estruturais relevantes devem ser registradas na seção "Changelog" ao final de cada arquivo.
- Use Pull Requests para qualquer alteração em arquivos já `Approved`.
