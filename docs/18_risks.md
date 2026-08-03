# 18. Riscos do Projeto

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 18.1 Riscos Técnicos

| Risco | Impacto | Probabilidade | Mitigação |
|---|---|---|---|
| Fragmentação de dispositivos ARCore (comportamento inconsistente entre fabricantes) | Alto | Alta | Device matrix de teste (`14_testing.md` §14.7), fallback robusto sem Depth API, checagem de compatibilidade explícita antes do onboarding |
| Baixa taxa de sucesso de scan em ambientes pequenos/mal iluminados | Alto | Média | Critério de scan mínimo flexível com fallback por tempo (`AR-019`), tutorial claro orientando o jogador |
| Performance instável em dispositivos de entrada (tier baixo) | Alto | Média | Sistema de Device Tiers com qualidade escalável (`16_optimization.md`), testes de FPS obrigatórios por tier antes de cada release |
| Aquecimento de dispositivo em sessões AR prolongadas | Médio | Média | Otimização de sampling de Depth API/câmera, sessões curtas por design (3–6 min), alerta opcional ao jogador |
| Complexidade de pathfinding sobre geometria real dinâmica | Médio | Média | Abordagem simplificada (flow field throttled) em vez de NavMesh completo; escopo reduzido para MVP (Fragments voadores ignoram obstáculos inicialmente) |
| Dependência de bibliotecas de terceiros (FMOD, plugin de áudio espacial, DI) | Médio | Baixa | Avaliação de licenciamento antes da adoção; abstrair via interfaces internas para permitir substituição futura sem reescrever gameplay |
| Escopo de multiplayer assíncrono (Cloud Anchors) subestimado | Médio | Média | Classificado como P2/pós-MVP explicitamente; não bloqueia roadmap principal |

## 18.2 Riscos Financeiros

| Risco | Impacto | Probabilidade | Mitigação |
|---|---|---|---|
| Custo de aquisição de usuário (UA) alto para categoria AR de nicho | Alto | Alta | Priorizar orgânico/viral (mecânica de compartilhamento, `UI-021`) antes de investir pesado em UA pago; validar CPI em soft launch antes de escalar |
| Monetização insuficiente (ARPDAU abaixo do necessário para sustentar operação solo) | Alto | Média | Modelo híbrido (ads + IAP + Battle Pass) diversifica fontes de receita; testes A/B de preço/oferta via Remote Config antes de release global |
| Custo de infraestrutura de backend escalando com base de usuários | Médio | Baixa | BaaS com pricing por uso (Supabase/Firebase) evita custo fixo alto no início; monitorar métricas de uso desde o Alpha |
| Dependência de plataformas de terceiros (Google Play policies, AdMob) para receita | Médio | Média | Diversificar mediação de anúncios (múltiplas redes), acompanhar mudanças de política de loja proativamente |
| Orçamento de arte/áudio subestimado para múltiplos biomas | Médio | Média | Priorização de 1 bioma completo para Vertical Slice antes de comprometer orçamento total; considerar assets terceirizados/freelance pontual |

## 18.3 Riscos de Design

| Risco | Impacto | Probabilidade | Mitigação |
|---|---|---|---|
| Core loop não se sustenta divertido além de poucas sessões (fadiga de novidade AR) | Alto | Média | Validação antecipada via MVP e Vertical Slice com playtesters reais antes de investir em conteúdo adicional |
| Dificuldade dinâmica mal calibrada gera frustração ou tédio | Alto | Média | Limite de ajuste ±15% por onda, playtests dedicados de balanceamento (`QA-032`), telemetria contínua pós-lançamento |
| Jogabilidade dependente de espaço físico adequado (jogadores com ambientes pequenos/lotados excluídos) | Médio | Alta | Fallback de scan flexível, modos que funcionam bem em espaços reduzidos, comunicação clara de requisitos na loja de apps |
| Percepção de pay-to-win mesmo com modelo cosmético-only mal comunicado | Médio | Baixa | Transparência de design (upgrades de poder sempre obteníveis via gameplay), comunicação clara em loja/marketing |
| Complexidade de onboarding AR afasta jogadores casuais logo no início | Alto | Média | Tutorial interativo obrigatório (`GP-042`), métricas de funil monitoradas de perto (`12_analytics.md` §12.6) |
| Conteúdo insuficiente no lançamento gera baixa retenção D7/D30 | Alto | Média | Roadmap de conteúdo pós-lançamento definido antes do Release (`13_roadmap.md` §13.6), cadência de eventos sazonais planejada |

## 18.4 Riscos Operacionais (Desenvolvedor Solo)

| Risco | Impacto | Probabilidade | Mitigação |
|---|---|---|---|
| Sobrecarga de escopo para 1 desenvolvedor (burnout) | Alto | Alta | Uso extensivo de agentes de IA para boilerplate/testes (`05_ai.md` §5.7), backlog priorizado rigoramente por P0/P1/P2, cortar escopo P2 sem culpa se necessário |
| Falta de revisão externa de código/design (pontos cegos) | Médio | Média | Buscar playtesters externos regularmente, considerar consultoria pontual em áreas críticas (AR, backend, monetização) |
| Dependência de conhecimento não documentado (bus factor 1) | Médio | Alta | Documentação exaustiva (este próprio conjunto de docs) como mitigação direta; ADRs para decisões críticas |

## Changelog

- 2026-08-03 — Criação inicial do documento.
