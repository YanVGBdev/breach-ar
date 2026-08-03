# 15. Backlog

**Status:** Draft | **Última atualização:** 2026-08-03

---

## Como Ler Este Backlog

- **Prioridade**: `P0` (bloqueante para MVP), `P1` (necessário até Beta/Release), `P2` (pós-lançamento/nice-to-have).
- **Complexidade**: `S` (≤4h), `M` (0.5–1.5 dia), `L` (2–4 dias), `XL` (>4 dias, considerar quebrar em subtarefas).
- **Tempo Estimado**: estimativa para 1 desenvolvedor solo com apoio de agente de IA para geração de boilerplate.
- **Dependências**: IDs de outras tarefas deste backlog ou de documentos referenciados.
- Este backlog é a **fonte de verdade de escopo**. Qualquer tarefa nova deve ser adicionada aqui antes de iniciada.

Categorias e prefixos de ID: `GP` Gameplay · `UI` Interface · `AR` Realidade Aumentada · `PH` Física · `AI` Inteligência Artificial · `AU` Áudio · `ART` Arte · `BK` Backend · `QA` Testes · `OPT` Otimização.

---

## 15.1 Gameplay (GP)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| GP-001 | Criar `LaunchConfigSO` e lógica de cálculo de vetor de arraste | P0 | AR-004 | M | 1d | Arraste gera vetor de força clamped conforme config; testável em EditMode |
| GP-002 | Implementar `OrbController` (voo, dano, expiração) | P0 | GP-001, PH-002 | L | 2d | Orbe voa, colide, aplica dano e expira após N ricochetes |
| GP-003 | Implementar preview de trajetória (linha pontilhada) | P0 | GP-001 | M | 1d | Linha reflete trajetória real calculada com gravidade do jogo |
| GP-004 | Implementar `CoreController` (HP, dano, eventos) | P0 | — | M | 1d | Core recebe dano, dispara `OnCoreDamaged`, Game Over ao HP=0 |
| GP-005 | Implementar `RiftController` (integridade, spawn timer, fechamento) | P0 | AR-006 | L | 2d | Rift spawna Fragments periodicamente e fecha ao HP=0 |
| GP-006 | Implementar `FragmentController` base + FSM (Spawn/Seek/Attack/Death) | P0 | AI-003 | L | 3d | Fragment segue estados corretamente, ataca Core ao alcançar range |
| GP-007 | Implementar `ComboSystem` (multiplicador, janela, decaimento) | P0 | GP-002 | M | 1d | Multiplicador sobe/reseta conforme regras de `02_gameplay.md` |
| GP-008 | Implementar `ScoreSystem` (pontuação por evento × combo) | P0 | GP-007 | S | 4h | Pontuação calculada corretamente para todos eventos da tabela 2.3 |
| GP-009 | Implementar `WaveDefinitionSO` e leitor de composição de onda | P0 | AI-006 | M | 1d | Onda spawna Fragments conforme SO configurado |
| GP-010 | Implementar `SessionStateMachine` (WaveIntro→...→RunComplete/Failed) | P0 | GP-004, GP-009 | L | 2d | Transições cobertas por teste de integração |
| GP-011 | Implementar sistema de multi-kill (ricochete atinge 2+ alvos) | P1 | GP-002 | M | 1d | Bônus de pontuação aplicado corretamente por alvo extra |
| GP-012 | Implementar fragmentação em cadeia (explosão contamina vizinhos) | P1 | GP-006 | M | 1d | Fragment com tag "chain" causa dano em raio ao morrer |
| GP-013 | Implementar captura de power-up em queda (segundo toque/orbe) | P1 | GP-002 | M | 1d | Power-up expira se não capturado em N segundos |
| GP-014 | Implementar Overcharge do Core (hold-to-charge, disparo de área) | P1 | GP-004 | M | 1d | Cooldown respeitado, dano em área aplicado |
| GP-015 | Implementar interação de Fragments com móveis (cobertura/rota alternativa) | P1 | AI-008, AR-005 | L | 2d | Fragments terrestres desviam de colliders de móveis |
| GP-016 | Implementar `PowerupDefinitionSO` + spawner de power-ups | P0 | GP-009 | M | 1d | Power-ups definidos na tabela 2.9 spawnam e aplicam efeito |
| GP-017 | Implementar efeito Orbe Múltiplo | P1 | GP-016 | S | 4h | Próximos 3 lançamentos disparam 3 orbes em leque |
| GP-018 | Implementar efeito Fenda Temporal (slow global) | P1 | GP-016 | S | 4h | Fragments reduzem velocidade em 50% por 8s |
| GP-019 | Implementar efeito Escudo do Core | P1 | GP-016 | S | 4h | Absorve 3 impactos antes de expirar |
| GP-020 | Implementar efeito Sobrecarga (dano em área no próximo orbe) | P1 | GP-016 | S | 4h | Próximo orbe causa explosão de área |
| GP-021 | Implementar efeito Ímã de Energia | P1 | GP-016 | S | 4h | Orbes atraem Fragments próximos por 10s |
| GP-022 | Implementar árvore de upgrades de Orbe (dano/velocidade/área/elemento) | P1 | BK-010 | XL | 5d | Upgrades persistem e afetam stats do `OrbDefinitionSO` runtime |
| GP-023 | Implementar economia de moeda soft (Fragmentos de Energia) | P0 | BK-009 | M | 1d | Moeda creditada ao fim de sessão e gasta em upgrades |
| GP-024 | Implementar economia de moeda hard (Cristais) | P1 | BK-011 | M | 1d | Cristais creditados via IAP/raramente via gameplay |
| GP-025 | Implementar Modo Campanha (sequência de ondas fixa + bioma) | P0 | GP-010 | L | 2d | Progressão linear de ondas com boss ao final |
| GP-026 | Implementar Modo Endless (dificuldade crescente sem teto) | P0 | AI-004 | L | 2d | Ondas geradas infinitamente, ranking de pontuação registrado |
| GP-027 | Implementar Modo Desafio Diário (seed fixa) | P1 | AI-006, BK-002 | L | 2d | Todos jogadores no mesmo dia recebem a mesma seed/padrão |
| GP-028 | Implementar Modo Zen (sem Game Over) | P2 | GP-010 | S | 4h | Core não perde HP; sem submissão de ranking |
| GP-029 | Implementar boss base: FSM multi-fase + weak points | P1 | AI-005 | XL | 4d | Boss transiciona fases ao destruir weak points |
| GP-030 | Implementar boss: exigência de combo x3.0 para dano crítico na fase final | P1 | GP-029, GP-007 | M | 1d | Dano reduzido se combo < x3.0 na fase final |
| GP-031 | Implementar Evento de Enxame (onda massiva de Fragments fracos) | P2 | GP-009 | M | 1d | Onda especial spawna volume alto de inimigos fracos |
| GP-032 | Implementar Rift Rara (Golden Rift) | P2 | GP-005 | S | 4h | Chance configurável de spawn, recompensa de moeda hard |
| GP-033 | Implementar Evento Comunitário (meta agregada global) | P2 | BK-013 | L | 2d | Progresso global sincronizado via backend, recompensa distribuída |
| GP-034 | Implementar Evento Sazonal (temas/Orbes exclusivos) | P2 | ART-018, BK-012 | L | 3d | Conteúdo sazonal ativa/desativa via Remote Config |
| GP-035 | Implementar sistema de indicador de ameaça fora de tela | P0 | UI-005 | M | 1d | Seta aponta corretamente para ameaças fora do FOV |
| GP-036 | Implementar cálculo de "onda perfeita" (bônus sem dano ao Core) | P1 | GP-004, GP-008 | S | 4h | Bônus de +1000 aplicado corretamente |
| GP-037 | Implementar sistema de Revive (anúncio/Cristais) | P1 | MON (10_monetization), BK-011 | M | 1d | Core restaura HP parcial, sessão continua |
| GP-038 | Implementar object pooling para Fragments/Orbes/VFX | P0 | GP-002, GP-006 | M | 1d | Sem `Instantiate/Destroy` em runtime de gameplay ativo |
| GP-039 | Implementar sistema de replay de inputs (base p/ multiplayer assíncrono) | P2 | GP-010 | L | 3d | Sequência de lançamentos serializável e reproduzível |
| GP-040 | Implementar Modo Multiplayer Assíncrono (fantasma) | P2 | GP-039, AR-014 | XL | 5d | Jogador compete contra replay de outro jogador no próprio ambiente |
| GP-041 | Balancear curva de dano vs. HP de Fragments por bioma | P1 | AI-002 | M | 1d | Dados registrados em `FragmentDefinitionSO` por bioma |
| GP-042 | Implementar tutorial interativo (primeiras 2 ondas guiadas) | P0 | UI-010 | L | 2d | Novo jogador completa onboarding sem instrução externa |

## 15.2 Interface (UI)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| UI-001 | Criar Canvas World Space ancorado à câmera para HUD | P0 | AR-002 | M | 1d | HUD acompanha câmera sem jitter perceptível |
| UI-002 | Implementar barra de HP do Core na HUD | P0 | GP-004 | S | 4h | Barra reflete HP em tempo real com gradiente de cor |
| UI-003 | Implementar contador de onda na HUD | P0 | GP-010 | S | 2h | Exibe onda atual/total corretamente por modo |
| UI-004 | Implementar contador de pontuação com animação de incremento | P0 | GP-008 | S | 4h | Número anima suavemente ao incrementar |
| UI-005 | Implementar indicador visual de multiplicador de combo | P0 | GP-007 | S | 4h | Muda cor/escala conforme multiplicador sobe |
| UI-006 | Implementar barra de power-ups ativos com cooldown radial | P1 | GP-016 | M | 1d | Ícones mostram cooldown e estado ativo corretamente |
| UI-007 | Implementar botão de pausa e fluxo de pausa | P0 | GP-010 | S | 4h | Pausa física/spawn sem interromper AR tracking |
| UI-008 | Implementar barra de Integridade da Rift (World Space) | P0 | GP-005 | S | 4h | Barra ancorada à Rift, reflete dano em tempo real |
| UI-009 | Criar tela de Menu Principal (navegação base) | P0 | — | M | 1d | Todos botões navegam para telas corretas (placeholder ok) |
| UI-010 | Implementar fluxo de onboarding/tutorial visual | P0 | GP-042 | L | 2d | Instruções claras guiam scan + primeiro lançamento |
| UI-011 | Criar tela de Seleção de Modo de Jogo | P0 | GP-025, GP-026 | S | 4h | Todos os modos disponíveis listados e navegáveis |
| UI-012 | Criar tela de Loja (estrutura base) | P1 | MON | L | 2d | Exibe cosméticos/moedas/Battle Pass com placeholders de preço |
| UI-013 | Criar tela de Perfil/Progressão | P1 | BK-010 | M | 1d | Exibe nível, XP, upgrades desbloqueados |
| UI-014 | Criar tela de Ranking (abas Global/Amigos/Diário) | P1 | BK-002 | L | 2d | Lista carregada do backend, posição do jogador destacada |
| UI-015 | Criar tela de Configurações (áudio/gráficos/controles) | P0 | — | M | 1d | Todas as opções da seção 7.3 funcionais e persistidas |
| UI-016 | Implementar aviso de compatibilidade de dispositivo | P0 | AR-016 | S | 4h | Mensagem exibida corretamente em dispositivo incompatível |
| UI-017 | Implementar overlay de Pausa completo (opções + stats parciais) | P0 | UI-007 | M | 1d | Todas ações (continuar, rescan, reiniciar, sair) funcionam |
| UI-018 | Implementar tela de Game Over (resumo de run) | P0 | GP-010 | L | 2d | Exibe todas as estatísticas da seção 7.5 corretamente |
| UI-019 | Implementar comparação com recorde pessoal no Game Over | P1 | BK-005 | S | 4h | Destaque visual se novo recorde |
| UI-020 | Implementar animação de "reveal" de recompensas | P1 | UI-018 | M | 1d | Recompensas reveladas com animação sequencial |
| UI-021 | Implementar CTA de compartilhamento (clipe/imagem) | P2 | ART-025 | L | 2d | Gera imagem/clipe compartilhável nas redes |
| UI-022 | Implementar CTA de Revive na tela de Game Over | P1 | GP-037 | S | 4h | Botão dispara fluxo de anúncio/Cristais corretamente |
| UI-023 | Implementar acessibilidade: alto contraste de HUD | P2 | UI-001 | M | 1d | Toggle aplica esquema de cor alternativo |
| UI-024 | Implementar acessibilidade: tamanho de fonte ajustável | P2 | UI-001 | S | 4h | Fonte escala conforme preferência salva |
| UI-025 | Implementar acessibilidade: redução de vibração/shake de câmera | P2 | UI-015 | S | 2h | Toggle reduz efeitos de câmera para conforto |
| UI-026 | Implementar consentimento de privacidade (LGPD/GDPR) no primeiro acesso | P0 | BK-014 | M | 1d | Consentimento bloqueia analytics/ads até aceite |
| UI-027 | Implementar fluxo de vínculo de conta (Google Play Games) | P1 | BK-001 | M | 1d | Conta vinculada persiste progresso entre dispositivos |
| UI-028 | Implementar botão "Restaurar Compras" | P1 | MON | S | 4h | IAPs não-consumíveis restaurados corretamente |
| UI-029 | Implementar feedback visual de toque (botões, microinterações) | P1 | ART-024 | M | 1d | Todos botões possuem feedback visual/tátil consistente |
| UI-030 | Implementar HUD adaptativo por tier de dispositivo (simplificação) | P2 | OPT-020 | M | 1d | Elementos não-essenciais ocultos em tier baixo |

## 15.3 Realidade Aumentada (AR)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| AR-001 | Configurar AR Foundation + ARCore XR Plugin no projeto | P0 | — | M | 1d | Sessão AR inicializa em dispositivo real com tracking |
| AR-002 | Implementar `ARSessionService` (wrapper injetável) | P0 | AR-001 | M | 1d | Serviço exposto via DI, sem acesso direto a AR Foundation fora dele |
| AR-003 | Implementar fluxo de scanning com feedback visual de progresso | P0 | AR-002 | L | 2d | UI mostra progresso de scan até critério mínimo atingido |
| AR-004 | Implementar detecção de planos horizontais/verticais | P0 | AR-001 | M | 1d | `ARPlaneManager` configurado, planos filtrados por tamanho mínimo |
| AR-005 | Implementar classificação semântica de superfícies (Floor/Ceiling/Wall/Furniture) | P0 | AR-004 | L | 2d | Heurística classifica corretamente em ≥ 90% dos testes manuais |
| AR-006 | Implementar posicionamento e anchor do Core (toque no chão) | P0 | AR-005 | M | 1d | Core ancorado corretamente ao toque válido no chão |
| AR-007 | Implementar geração/gerenciamento de Anchors para Rifts | P0 | AR-005, AI-006 | M | 1d | Rifts ancoradas corretamente por superfície, liberadas após 60s inativas |
| AR-008 | Implementar `AROcclusionManager` + Depth API | P0 | AR-001 | L | 2d | Depth texture disponível e atualizada em tempo real |
| AR-009 | Implementar shader customizado de oclusão AR (URP) | P0 | AR-008, ART-020 | L | 2d | Objetos virtuais ocultos corretamente atrás de geometria real |
| AR-010 | Implementar fallback de oclusão via plane mesh (sem Depth API) | P0 | AR-004 | M | 1d | Oclusão funcional (qualidade reduzida) em dispositivos sem Depth |
| AR-011 | Implementar Light Estimation (luz direcional + ambient probes) | P1 | AR-001 | M | 1d | Iluminação de objetos virtuais reage à luz real do ambiente |
| AR-012 | Implementar raycast de profundidade para colisão com objetos não planares | P1 | AR-008 | L | 2d | Orbes colidem corretamente com objetos irregulares detectados via depth |
| AR-013 | Implementar geração de mesh collider dinâmico para planos | P0 | AR-004 | M | 1d | Collider atualizado a cada mudança relevante de boundary (throttled) |
| AR-014 | Implementar suporte a Cloud Anchors (base multiplayer) | P2 | AR-007 | XL | 4d | Anchor compartilhado entre dois dispositivos no mesmo local |
| AR-015 | Implementar função "Rescan" acessível via menu de pausa | P0 | AR-003, UI-017 | M | 1d | Rescan não perde progresso da sessão atual |
| AR-016 | Implementar checagem de compatibilidade de dispositivo (ARCore/Depth) | P0 | AR-001 | M | 1d | Detecta corretamente suporte e define fallback apropriado |
| AR-017 | Implementar heurística de detecção de teto (horizontal-down + altura relativa) | P1 | AR-005 | M | 1d | Teto identificado corretamente em ≥ 85% dos testes manuais |
| AR-018 | Implementar distribuição procedural de Rifts nas superfícies (Poisson-disc) | P0 | AR-005, AI-006 | L | 2d | Rifts distribuídas sem clusters não naturais, respeitando distância mínima |
| AR-019 | Implementar critério mínimo de scan (área/tempo) com fallback para ambientes pequenos | P0 | AR-003 | M | 1d | Onboarding avança mesmo em ambientes pequenos após timeout |
| AR-020 | Implementar liberação automática de Anchors não utilizadas | P1 | AR-007 | S | 4h | Anchors inativas destruídas após 60s, sem vazamento de memória |
| AR-021 | Implementar toggle de qualidade de oclusão (Depth on/off) nas configurações | P2 | UI-015, AR-008 | S | 4h | Alternância aplica fallback correspondente em runtime |
| AR-022 | Implementar recuperação de perda de tracking (relocalização) | P0 | AR-002 | L | 2d | Sessão se recupera de perda breve de tracking sem crash |
| AR-023 | Escrever documentação de fallback para dispositivos não certificados | P1 | AR-016 | S | 2h | Documento cobre comportamento esperado por tier de suporte |
| AR-024 | Implementar limite de distância mínima entre Rifts na distribuição procedural | P0 | AR-018 | S | 4h | Rifts nunca spawnam sobrepostas ou coladas |
| AR-025 | Implementar visualização de debug de planos detectados (modo dev) | P1 | AR-004 | S | 4h | Overlay de debug ativável para diagnóstico em campo |
| AR-026 | Implementar métricas de qualidade de tracking (confidence score) | P1 | AR-002 | M | 1d | Score exposto para uso em analytics/QA (`AR-... /12_analytics`) |
| AR-027 | Implementar suporte a múltiplas sessões de scan no mesmo local (persistência de layout) | P2 | AR-014 | L | 2d | Layout reconhecido ao reabrir o app no mesmo ambiente |

## 15.4 Física (PH)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| PH-001 | Configurar camadas de física e matriz de colisão | P0 | — | S | 4h | Colisões ocorrem apenas entre camadas permitidas |
| PH-002 | Implementar cálculo de força de lançamento (`AddForce` via drag) | P0 | GP-001 | M | 1d | Força aplicada de forma consistente independente de frame rate |
| PH-003 | Implementar gravidade escalável por tipo de Orbe | P0 | AR-... (n/a) | S | 4h | `GravityScale` de `OrbDefinitionSO` afeta Rigidbody corretamente |
| PH-004 | Implementar materiais físicos por tipo de superfície (bounciness/fricção) | P0 | PH-001 | M | 1d | Ricochete varia corretamente por material (parede/móvel/chão) |
| PH-005 | Implementar limite de ricochetes por Orbe com falloff de dano | P1 | PH-004 | M | 1d | Orbe expira após N ricochetes; dano reduz por ricochete conforme config |
| PH-006 | Implementar detecção e resolução de colisão Orbe-Fragment | P0 | PH-001, GP-002 | M | 1d | Dano aplicado corretamente, sem duplo-hit no mesmo frame |
| PH-007 | Implementar sequência de destruição de Fragment (dissolve + pool) | P0 | GP-038 | M | 1d | Fragment retorna ao pool após VFX, sem instanciar/destruir |
| PH-008 | Implementar sequência de fechamento de Rift (implosão) | P0 | GP-005 | M | 1d | Animação de 1s + destruição de anchor ao final |
| PH-009 | Implementar física de queda de power-ups (float suave) | P1 | GP-016 | S | 4h | Power-up cai com gravidade reduzida, colisor de captura maior |
| PH-010 | Implementar colliders independentes para weak points de boss | P1 | GP-029 | M | 1d | Cada weak point recebe dano independentemente |
| PH-011 | Implementar cooldown mínimo entre lançamentos (anti-spam) | P0 | PH-002 | S | 2h | Lançamentos limitados a intervalo mínimo configurável |
| PH-012 | Implementar sistema de partículas de impacto (Shuriken, pool) | P1 | ART-021 | M | 1d | Partículas de impacto poolizadas, sem alloc em runtime |
| PH-013 | Implementar VFX Graph para explosões/implosões (GPU particles) | P1 | ART-021 | L | 2d | Efeitos rodam performáticos em dispositivo tier médio |
| PH-014 | Implementar budget de partículas simultâneas por device tier | P1 | OPT-005 | M | 1d | Densidade de partículas reduzida automaticamente em tier baixo |
| PH-015 | Implementar simulação analítica de trajetória para preview | P0 | GP-003 | M | 1d | Linha de preview corresponde à trajetória real simulada |
| PH-016 | Implementar dano em área (Overcharge/Sobrecarga) | P1 | GP-014, GP-020 | M | 1d | Todos alvos no raio recebem dano correto |
| PH-017 | Implementar reação em cadeia de Fragments explosivos | P1 | GP-012 | M | 1d | Explosão contamina vizinhos dentro do raio configurado |
| PH-018 | Implementar decals temporários de impacto em superfícies reais | P2 | ART-022 | S | 4h | Decal aparece e desaparece sem afetar geometria real |
| PH-019 | Implementar testes de física determinística (fixed timestep) | P0 | PH-002 | M | 1d | Resultados de física consistentes entre execuções/dispositivos |
| PH-020 | Otimizar geração de mesh collider (throttle e simplificação) | P1 | AR-013 | M | 1d | Custo de CPU de atualização de collider dentro do budget definido |
| PH-021 | Implementar física de weak points destrutíveis em sequência (boss) | P1 | PH-010 | M | 1d | Weak points só recebem dano na ordem/condição definida pela fase |
| PH-022 | Implementar comportamento de física para lançamento em arco (Rifts de teto) | P1 | GP-001 | M | 1d | Preview e trajetória suportam ângulos altos corretamente |

## 15.5 Inteligência Artificial (AI)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| AI-001 | Implementar `DifficultyDirector` (coleta de sinais: HitRate, ReactionTime etc.) | P0 | GP-002 | L | 2d | Sinais calculados corretamente em janela deslizante |
| AI-002 | Implementar algoritmo heurístico de DDA (skill_score → difficulty_delta) | P0 | AI-001 | L | 2d | Delta aplicado apenas no início de onda, dentro de ±15% |
| AI-003 | Implementar FSM base de Fragment (Spawn/Seek/Attack/Death/Staggered) | P0 | GP-006 | L | 2d | Transições cobertas por teste de integração |
| AI-004 | Implementar escalonamento de dificuldade sem teto para modo Endless | P0 | AI-002 | M | 1d | Dificuldade cresce geometricamente após ponto configurado |
| AI-005 | Implementar Utility AI para escolha de ataque de boss por fase | P1 | GP-029 | XL | 4d | Boss escolhe ataques ponderando weak points/cooldowns/posição do jogador |
| AI-006 | Implementar geração procedural de composição de onda (Wave Budget) | P0 | GP-009 | L | 2d | Composição respeita orçamento crescente por onda |
| AI-007 | Implementar seed determinística por sessão (base do Desafio Diário) | P1 | AI-006 | M | 1d | Mesma seed produz exatamente o mesmo padrão de Rifts/ondas |
| AI-008 | Implementar flow field leve para pathfinding sobre geometria real | P1 | AR-013 | XL | 3d | Fragments desviam corretamente de obstáculos reais mapeados |
| AI-009 | Implementar Utility AI de Fragment comum (Seek/Flank/Retreat) | P1 | AI-003 | L | 2d | Decisão pondera distância, HP e Orbes próximos corretamente |
| AI-010 | Implementar comportamento evasivo de Fragment elite | P1 | AI-009 | M | 1d | Elite evade com maior assertividade que Fragment comum |
| AI-011 | Implementar habilidade de Fragment elite de "quebrar" combo do jogador | P2 | AI-010 | M | 1d | Fragment se interpõe estrategicamente na trajetória prevista |
| AI-012 | Implementar distribuição procedural de Rifts (Poisson-disc) por sessão | P0 | AR-018 | L | 2d | (Referência cruzada — implementação compartilhada com AR-018) |
| AI-013 | Implementar seleção procedural de bioma visual por progressão/sazonalidade | P1 | ART-018 | M | 1d | Bioma selecionado respeita regras de progressão/rotação |
| AI-014 | Configurar pipeline offline de geração de texto (tooltips/flavor text) via LLM | P2 | — | M | 1d | Textos gerados revisados manualmente antes de entrar em build |
| AI-015 | Implementar dashboard/processo de análise de telemetria para sugestão de balanceamento | P1 | BK-... /12_analytics | L | 2d | Relatório periódico sugere ajustes de curva de dificuldade |
| AI-016 | Implementar consumo de Remote Config para parâmetros de DDA/economia | P0 | BK-012 | M | 1d | Parâmetros carregados do backend sobrepõem defaults locais |
| AI-017 | Escrever testes unitários do algoritmo de DDA (casos extremos) | P0 | AI-002 | M | 1d | Casos de skill muito alto/baixo não geram overflow/underflow |
| AI-018 | Implementar sistema de "orçamento" de spawn por onda (budget-based) | P0 | AI-006 | M | 1d | Custo total de Fragments spawnados respeita orçamento definido |
| AI-019 | Documentar e versionar heurística de classificação de superfícies como parte do pipeline de IA de conteúdo | P1 | AR-005 | S | 4h | Documento describe critérios e limites de confiança |
| AI-020 | Implementar recalculo periódico de flow field (throttle 1-2s) | P1 | AI-008 | M | 1d | Recalculo não gera spikes de CPU perceptíveis |
| AI-021 | Implementar comportamento de voo para Fragments aéreos (Rifts de teto) | P1 | AI-003 | M | 1d | Fragments aéreos ignoram obstáculos de chão e miram o Core em arco |
| AI-022 | Implementar sistema de "orçamento" adaptativo por tipo de superfície disponível | P1 | AI-018, AR-005 | M | 1d | Ambientes com poucas paredes compensam com mais Rifts de chão/móveis |
| AI-023 | Escrever ADR sobre escolha entre Utility AI própria vs. framework externo | P1 | — | S | 2h | ADR registrado em `/docs/adr/` com justificativa |
| AI-024 | Implementar testes de regressão para geração procedural de ondas | P1 | AI-006 | M | 1d | Seeds fixas geram sempre a mesma composição em builds diferentes |
| AI-025 | Implementar exportação de logs de decisão de IA para depuração (modo dev) | P2 | AI-005 | S | 4h | Log exibe decisão de ataque de boss/Fragment em tempo real |

## 15.6 Áudio (AU)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| AU-001 | Integrar FMOD ao projeto Unity | P0 | — | M | 1d | Eventos FMOD disparáveis a partir de C# |
| AU-002 | Implementar `AudioManager` (injetável, gerencia volumes/mixagem) | P0 | AU-001 | M | 1d | Volumes master/música/SFX ajustáveis e persistidos |
| AU-003 | Implementar trilha adaptativa por camadas (vertical layering) | P1 | AU-001 | L | 2d | Camadas entram/saem conforme intensidade de onda/combo |
| AU-004 | Compor/licenciar trilha temática por bioma | P1 | ART-018 | XL | — | 1 trilha completa por bioma, loop sem repetição perceptível < 3min |
| AU-005 | Implementar transição crossfade entre biomas | P1 | AU-004 | S | 4h | Transição de 2s sem corte abrupto perceptível |
| AU-006 | Compor música de menu | P1 | — | M | — | Loop longo, tom distinto da trilha de gameplay |
| AU-007 | Implementar SFX de lançamento (whoosh, overcharge) | P0 | AU-002 | S | 4h | Sons disparam corretamente por evento de gameplay |
| AU-008 | Implementar SFX de impacto variando por material de superfície | P0 | PH-004 | M | 1d | Som varia corretamente entre parede/móvel/chão |
| AU-009 | Implementar SFX de morte de Fragment (comum/elite, variações) | P0 | GP-006 | M | 1d | Pool de variações evita repetição perceptível em spam |
| AU-010 | Implementar SFX de Rift (abertura/fechamento/dano) | P0 | GP-005 | S | 4h | Sons sincronizados com eventos correspondentes |
| AU-011 | Implementar SFX de dano/destruição do Core (escalonado por %HP) | P0 | GP-004 | S | 4h | Intensidade sonora aumenta conforme HP diminui |
| AU-012 | Implementar stinger de combo a cada +1.0 no multiplicador | P0 | GP-007 | S | 4h | Stinger toca corretamente em cada marco de combo |
| AU-013 | Implementar SFX de power-up (coleta/ativação/expiração) | P1 | GP-016 | S | 4h | Sons disparam nos eventos corretos |
| AU-014 | Implementar SFX de UI (toque, transição, notificação) | P0 | UI-009 | S | 4h | Todos elementos interativos possuem feedback sonoro |
| AU-015 | Implementar SFX de boss (aparição, ataques, weak point, derrota) | P1 | GP-029 | L | 2d | Sons por fase implementados e sincronizados com FSM |
| AU-016 | Integrar plugin de áudio espacial (Resonance Audio/Steam Audio) | P0 | AU-001 | L | 2d | HRTF funcional, direção sonora perceptível corretamente |
| AU-017 | Configurar `spatialBlend` e oclusão sonora por superfícies reais | P0 | AU-016 | M | 1d | Som abafado corretamente quando fonte está atrás de parede real |
| AU-018 | Implementar ducking automático de música em eventos críticos | P1 | AU-003 | S | 4h | Música reduz volume durante boss intro/Game Over |
| AU-019 | Implementar feedback sonoro direcional para ameaças fora de tela | P0 | GP-035 | M | 1d | Som sutil acompanha indicador visual de direção |
| AU-020 | Validar latência de feedback sonoro (< 50ms) | P0 | AU-002 | S | 2h | Testes manuais confirmam responsividade adequada |

## 15.7 Arte (ART)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| ART-001 | Definir style guide visual completo (paleta, tipografia, iconografia) | P0 | — | L | 2d | Documento de referência aprovado, usado por toda produção de arte |
| ART-002 | Modelar Fragment comum (tipo 1) low-poly | P0 | ART-001 | M | 1d | ≤ 800 tris, dentro do budget definido |
| ART-003 | Modelar Fragment comum (tipo 2) low-poly | P0 | ART-001 | M | 1d | ≤ 800 tris |
| ART-004 | Modelar Fragment elite (tipo 1) | P1 | ART-001 | M | 1d | ≤ 1500 tris, silhueta distinta de comuns |
| ART-005 | Modelar Fragment elite (tipo 2) | P1 | ART-001 | M | 1d | ≤ 1500 tris |
| ART-006 | Modelar boss (bioma 1) com LOD0/1/2 | P1 | ART-001 | XL | 4d | ≤ 8000 tris LOD0, LODs funcionais |
| ART-007 | Modelar Rift (variação parede) | P0 | ART-001 | M | 1d | Visual legível como "fenda", com pontos de anchor definidos |
| ART-008 | Modelar Rift (variação chão) | P0 | ART-001 | M | 1d | Idem, adaptado à orientação horizontal |
| ART-009 | Modelar Rift (variação teto) | P1 | ART-001 | M | 1d | Idem, adaptado à orientação invertida |
| ART-010 | Modelar Rift (variação móvel/elite) | P1 | ART-001 | M | 1d | Visualmente diferenciada como "elite" |
| ART-011 | Modelar Core (visual base) | P0 | ART-001 | M | 1d | Silhueta claramente "aliada", distinta de elementos hostis |
| ART-012 | Modelar Orbe base + variações elementais (4 tipos) | P0 | ART-001 | M | 1d | 4 variações visuais claramente distintas |
| ART-013 | Criar shader procedural emissivo/fresnel (base de todos elementos virtuais) | P0 | ART-001 | L | 2d | Shader reutilizável, parametrizável por cor/intensidade |
| ART-014 | Animar Fragment: idle/seek, attack, death, staggered | P0 | ART-002 | L | 2d | Todas animações integradas à FSM correspondente |
| ART-015 | Animar boss: intro, idle, ataques por fase, death sequence | P1 | ART-006 | XL | 4d | Animações sincronizadas com FSM de boss |
| ART-016 | Animar Rift: idle/spawn/damaged/closing | P0 | ART-007 | M | 1d | Todas transições visuais suaves e claras |
| ART-017 | Criar VFX de explosão de morte de Fragment | P0 | ART-013 | M | 1d | Efeito performático em VFX Graph, respeitando shader de oclusão |
| ART-018 | Criar `BiomeThemeSO` + paleta/tema visual do bioma 1 | P0 | ART-001 | L | 2d | Tema aplicado consistentemente a Rifts/Fragments/UI do bioma |
| ART-019 | Criar tema visual do bioma 2 | P1 | ART-018 | L | 2d | Idem, tema distinto do bioma 1 |
| ART-020 | Implementar shader de oclusão AR customizado (URP) | P0 | ART-013, AR-008 | L | 2d | Objetos ocultos corretamente atrás de depth real |
| ART-021 | Criar VFX Graph de trilha de Orbe em voo | P1 | ART-013 | M | 1d | Trilha visível e performática, poolizada |
| ART-022 | Criar decals de impacto temporários | P2 | ART-013 | S | 4h | Decal aparece/desaparece sem custo excessivo |
| ART-023 | Criar ícones de UI (combate/economia/progressão) | P0 | ART-001 | L | 2d | Iconografia consistente, Style Guide único (Figma referenciado) |
| ART-024 | Criar microinterações visuais de UI (feedback de toque) | P1 | ART-023 | M | 1d | Todos botões com resposta visual consistente |
| ART-025 | Criar template de imagem/clipe compartilhável (Game Over) | P2 | UI-021 | M | 1d | Template gera imagem com branding e stats da run |
| ART-026 | Criar cosméticos de skin de Orbe (3 iniciais) | P1 | ART-012 | L | 2d | 3 skins visualmente distintas, sem afetar física/dano |
| ART-027 | Criar cosméticos de skin de Core (2 iniciais) | P2 | ART-011 | M | 1d | 2 skins visualmente distintas |
| ART-028 | Criar assets visuais de Battle Pass (trilha grátis/premium) | P1 | ART-018 | L | 2d | Assets integrados à UI de progressão sazonal |
| ART-029 | Otimizar texturas/materiais para budget de memória mobile | P0 | OPT-... | M | 1d | Uso de VRAM dentro do budget por device tier |
| ART-030 | Criar splash screen e ícone do app | P0 | ART-001 | M | 1d | Assets aprovados conforme guidelines de loja (Google Play) |

## 15.8 Backend (BK)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| BK-001 | Configurar projeto Supabase (ou Firebase) + autenticação anônima | P0 | — | M | 1d | Login anônimo funcional no primeiro acesso |
| BK-002 | Implementar tabelas de leaderboard (Campaign/Endless/Daily) | P1 | BK-001 | M | 1d | Submissão e leitura de score funcionais |
| BK-003 | Implementar Edge Function de validação de score (anti-cheat básico) | P0 | BK-002, SEC | L | 2d | Scores implausíveis rejeitados conforme regras definidas |
| BK-004 | Implementar `ISaveRepository` local (criptografado) | P0 | — | M | 1d | Save local funcional offline, resistente a corrupção básica |
| BK-005 | Implementar sincronização de save em nuvem (debounced) | P1 | BK-001, BK-004 | L | 2d | Save espelhado corretamente sem perda em race condition |
| BK-006 | Implementar estratégia de merge de conflito (last-write-wins + maior valor cumulativo) | P1 | BK-005 | M | 1d | Testes de conflito simulado resolvem sem perda de progresso |
| BK-007 | Implementar endpoint de estatísticas agregadas globais | P2 | BK-001 | M | 1d | Endpoint retorna estatísticas consumíveis pela UI |
| BK-008 | Implementar tabela `players` (perfil) | P0 | BK-001 | S | 4h | Perfil criado automaticamente no primeiro acesso |
| BK-009 | Implementar tabela `player_economy` (moeda soft/hard) | P0 | BK-008 | M | 1d | Saldo persistido e sincronizado corretamente |
| BK-010 | Implementar tabela `player_inventory` (Orbes/skins/upgrades) | P1 | BK-008 | M | 1d | Itens desbloqueados refletidos corretamente no cliente |
| BK-011 | Implementar validação server-side de transações de IAP | P0 | BK-009, SEC | L | 2d | Créditos de moeda hard nunca aplicados sem confirmação server-side |
| BK-012 | Implementar serviço de Remote Config (balanceamento/feature flags) | P0 | BK-001 | L | 2d | Cliente consome config no boot com fallback offline |
| BK-013 | Implementar backend do Evento Comunitário (meta agregada) | P2 | BK-007 | L | 2d | Progresso global sincronizado em tempo quase real |
| BK-014 | Implementar registro de consentimento de privacidade (LGPD/GDPR) | P0 | BK-008 | M | 1d | Consentimento persistido e auditável |
| BK-015 | Implementar API de vínculo de conta (Google Play Games/Game Center) | P1 | BK-001 | L | 2d | Progresso portável entre dispositivos após vínculo |
| BK-016 | Implementar rate limiting de submissão de score/ranking | P0 | BK-002 | M | 1d | Submissões excessivas bloqueadas conforme regra definida |
| BK-017 | Implementar logs de auditoria de transações econômicas | P1 | BK-009 | M | 1d | Toda alteração de saldo é rastreável |
| BK-018 | Implementar deploy automatizado de Edge Functions (CI básico) | P1 | BK-001 | M | 1d | Pipeline de deploy documentado e funcional |
| BK-019 | Implementar cache local de ranking com sincronização periódica | P1 | BK-002 | M | 1d | Ranking exibido offline com dados levemente desatualizados |
| BK-020 | Documentar schema completo do banco de dados | P1 | BK-002..BK-014 | M | 1d | Documento/ERD atualizado e versionado no repositório |
| BK-021 | Implementar exclusão de conta/dados a pedido do jogador (LGPD/GDPR) | P0 | BK-014 | M | 1d | Fluxo de exclusão remove/anonimiza dados conforme exigido |
| BK-022 | Implementar backup automático periódico do banco de dados | P0 | BK-001 | M | 1d | Backups retidos por ≥ 30 dias, restauração testada |
| BK-023 | Implementar monitoramento/alertas de saúde do backend (uptime, latência) | P1 | BK-001 | M | 1d | Alertas disparados em caso de degradação de serviço |
| BK-024 | Implementar versionamento de API de backend (compatibilidade entre builds) | P1 | BK-001 | M | 1d | Builds antigas continuam funcionais durante janela de transição |
| BK-025 | Implementar ambiente de staging separado de produção | P0 | BK-001 | M | 1d | Testes de backend não afetam dados reais de jogadores |

## 15.9 Testes (QA)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| QA-001 | Configurar Unity Test Framework (EditMode + PlayMode) | P0 | — | S | 4h | Suite de testes executável via CLI/CI |
| QA-002 | Escrever testes unitários de `ComboSystem` | P0 | GP-007 | M | 1d | Cobre incremento, reset, cap máximo |
| QA-003 | Escrever testes unitários de `ScoreSystem`/DamageCalculator | P0 | GP-008 | M | 1d | Cobre todos eventos da tabela de pontuação |
| QA-004 | Escrever testes unitários de `DifficultyDirector` (DDA) | P0 | AI-002 | M | 1d | Cobre casos extremos de skill_score |
| QA-005 | Escrever testes unitários de `EconomyService` | P0 | GP-023 | M | 1d | Cobre crédito/débito de moeda soft/hard |
| QA-006 | Escrever testes unitários de `SaveRepository` (serialização) | P0 | BK-004 | M | 1d | Cobre serialização/desserialização sem perda de dados |
| QA-007 | Escrever testes unitários de merge de conflito de save | P1 | BK-006 | M | 1d | Cobre cenários de conflito simulado |
| QA-008 | Escrever testes de integração: fluxo completo de onda | P0 | GP-010 | L | 2d | Spawn → combate → wave cleared validado ponta a ponta |
| QA-009 | Escrever testes de integração: fluxo completo de Game Over | P0 | GP-010 | M | 1d | Core destruído → transição correta de estado |
| QA-010 | Escrever testes de integração: fluxo de compra de IAP (mockado) | P0 | BK-011 | M | 1d | Fluxo completo validado sem chamadas reais de billing |
| QA-011 | Criar mock de `IARSurfaceProvider` para testes sem sessão AR real | P0 | AR-002 | M | 1d | Testes PlayMode rodam sem dispositivo físico |
| QA-012 | Definir roteiro de teste manual de AR (ambientes variados) | P0 | — | M | 1d | Roteiro documentado cobre todos cenários da seção 14.5 |
| QA-013 | Executar teste manual: sala pequena com pouca luz | P0 | QA-012 | S | 2h | Resultado documentado, bugs registrados |
| QA-014 | Executar teste manual: sala grande e aberta | P0 | QA-012 | S | 2h | Resultado documentado |
| QA-015 | Executar teste manual: ambiente com muitos móveis pequenos | P0 | QA-012 | S | 2h | Classificação de superfícies validada |
| QA-016 | Executar teste manual: superfícies reflexivas/vidro | P1 | QA-012 | S | 2h | Comportamento degradado documentado |
| QA-017 | Executar teste manual: oclusão real (objeto entre jogador e Fragment) | P0 | QA-012, AR-009 | S | 2h | Oclusão correta confirmada visualmente |
| QA-018 | Executar teste manual: perda e recuperação de tracking | P0 | QA-012, AR-022 | S | 2h | Recuperação sem crash confirmada |
| QA-019 | Executar checklist de regressão AR (build candidata) | P0 | QA-012 | M | 1d | Checklist completo sem falhas críticas |
| QA-020 | Configurar profiling de performance (Unity Profiler) | P0 | — | S | 4h | Profiler configurado para captura em dispositivo real |
| QA-021 | Testar FPS estável em dispositivo tier baixo | P0 | OPT-001 | M | 1d | FPS dentro da meta definida em `16_optimization.md` |
| QA-022 | Testar FPS estável em dispositivo tier médio | P0 | OPT-001 | M | 1d | Idem |
| QA-023 | Testar FPS estável em dispositivo tier alto | P0 | OPT-001 | M | 1d | Idem |
| QA-024 | Testar GC spikes durante gameplay ativo | P0 | GP-038 | M | 1d | Nenhum spike > 5ms detectado em sessão típica |
| QA-025 | Testar sessão longa (30+ min) para memory leak | P1 | — | M | 1d | Uso de memória estável entre início/fim da sessão |
| QA-026 | Testar tempo de carregamento de cena | P1 | — | S | 4h | < 5s em dispositivo tier médio |
| QA-027 | Configurar device matrix mínima de QA manual (3 dispositivos, 2 fabricantes) | P0 | — | S | 4h | Matriz documentada e disponível para testes recorrentes |
| QA-028 | Configurar Firebase Test Lab (smoke tests automatizados) | P1 | — | M | 1d | Smoke test de boot/crash roda em matriz ampliada |
| QA-029 | Escrever casos de teste de economia (double-spend, race condition) | P0 | BK-009 | M | 1d | Casos cobrem tentativas de exploit conhecidas |
| QA-030 | Escrever casos de teste de anti-cheat de submissão de score | P0 | BK-003 | M | 1d | Scores implausíveis rejeitados nos testes |
| QA-031 | Testar fluxo de onboarding/tutorial com usuários reais (playtest) | P0 | GP-042 | L | 2d | ≥ 80% dos playtesters completam sem instrução externa |
| QA-032 | Testar balanceamento de dificuldade com playtesters de skill variado | P1 | AI-002 | L | 2d | Feedback qualitativo coletado e documentado |
| QA-033 | Testar todos os power-ups individualmente | P0 | GP-016..GP-021 | M | 1d | Cada power-up funciona conforme especificação |
| QA-034 | Testar boss completo (todas as fases) | P1 | GP-029 | L | 2d | Todas transições de fase validadas manualmente |
| QA-035 | Testar Modo Endless por sessão longa (validar escalonamento) | P1 | GP-026 | M | 1d | Dificuldade escala conforme esperado sem quebra |
| QA-036 | Testar Modo Desafio Diário (seed consistente entre dispositivos) | P1 | GP-027 | M | 1d | Mesma seed gera mesmo padrão em dispositivos diferentes |
| QA-037 | Testar fluxo completo de Battle Pass (grátis + premium) | P1 | GP-034 | M | 1d | Progresso e recompensas corretas em ambas trilhas |
| QA-038 | Testar fluxo de anúncios recompensados (revive, dobro de recompensa) | P0 | MON | M | 1d | Recompensa creditada apenas após confirmação de watch-to-completion |
| QA-039 | Testar consentimento de privacidade bloqueando analytics/ads | P0 | UI-026 | S | 4h | Nenhum evento disparado antes do consentimento |
| QA-040 | Testar localização (PT-BR/EN/ES) — strings não cortadas na UI | P1 | — | M | 1d | Textos revisados em todos os idiomas suportados |
| QA-041 | Testar acessibilidade (alto contraste, fonte, redução de shake) | P2 | UI-023..UI-025 | S | 4h | Todas opções aplicam efeito esperado |
| QA-042 | Testar sincronização de save em nuvem (troca de dispositivo) | P1 | BK-005 | M | 1d | Progresso restaurado corretamente em novo dispositivo |
| QA-043 | Testar comportamento offline completo (sem rede) | P0 | BK-004 | M | 1d | Jogo funcional offline, sync ocorre ao reconectar |
| QA-044 | Executar teste de carga de backend (ranking/economia) | P1 | BK-002 | L | 2d | Backend suporta carga estimada de lançamento sem degradação |
| QA-045 | Validar compatibilidade com política de lojas (Google Play/App Store) | P0 | — | M | 1d | Checklist de políticas de conteúdo/IAP/privacidade aprovado |
| QA-046 | Executar testes de regressão completa antes de cada release | P0 | QA-019 | L | 2d | Checklist completo sem bugs críticos pendentes |
| QA-047 | Testar exportação/geração de clipe compartilhável | P2 | UI-021 | S | 4h | Clipe/imagem gerado corretamente com dados da run |
| QA-048 | Testar comportamento em dispositivo sem suporte a Depth API | P0 | AR-010 | M | 1d | Fallback de oclusão funcional, sem crash |
| QA-049 | Testar consistência de física entre dispositivos (fixed timestep) | P0 | PH-019 | M | 1d | Resultados de trajetória equivalentes entre dispositivos testados |
| QA-050 | Documentar processo de bug tracking e triagem de severidade | P1 | — | S | 4h | Processo documentado e adotado pela equipe/desenvolvedor solo |

## 15.10 Otimização (OPT)

| ID | Tarefa | Prioridade | Dependências | Complexidade | Tempo | Critério de Aceite |
|---|---|---|---|---|---|---|
| OPT-001 | Definir Device Tiers (baixo/médio/alto) e critérios de classificação | P0 | — | M | 1d | Critérios documentados e aplicáveis via detecção em runtime |
| OPT-002 | Implementar detecção automática de Device Tier no boot | P0 | OPT-001 | M | 1d | Tier detectado corretamente na maioria dos dispositivos testados |
| OPT-003 | Definir metas de FPS por tier (ex: 30/45/60) | P0 | OPT-001 | S | 2h | Metas documentadas em `16_optimization.md` |
| OPT-004 | Implementar ajuste automático de qualidade gráfica por tier | P0 | OPT-002 | L | 2d | Qualidade se ajusta automaticamente sem input do jogador |
| OPT-005 | Implementar budget de partículas simultâneas por tier | P1 | PH-014 | M | 1d | Densidade de partículas respeita budget por tier |
| OPT-006 | Otimizar shaders para mobile (reduzir instruções, evitar overdraw) | P0 | ART-013 | L | 2d | Overdraw dentro de limite aceitável no Frame Debugger |
| OPT-007 | Implementar object pooling abrangente (Fragments/Orbes/VFX/UI popups) | P0 | GP-038 | M | 1d | Sem `Instantiate/Destroy` em hot paths de gameplay |
| OPT-008 | Otimizar geração/atualização de mesh collider AR (throttle) | P0 | AR-013 | M | 1d | Custo de CPU dentro do budget definido por tier |
| OPT-009 | Otimizar texturas (compressão ASTC, mipmaps, tamanho por tier) | P0 | ART-029 | M | 1d | Uso de VRAM dentro do budget por tier |
| OPT-010 | Reduzir alocações de GC em Update loops críticos | P0 | — | L | 2d | Profiler confirma ausência de spikes de GC recorrentes |
| OPT-011 | Implementar LOD para bosses e Rifts | P1 | ART-006 | M | 1d | LODs trocam corretamente por distância da câmera |
| OPT-012 | Otimizar Depth API sampling (frequência/resolução por tier) | P0 | AR-008 | L | 2d | Custo de Depth API dentro do budget de CPU/GPU por tier |
| OPT-013 | Implementar throttling de flow field/pathfinding por tier | P1 | AI-020 | M | 1d | Frequência de recálculo reduzida em tiers baixos |
| OPT-014 | Reduzir footprint de memória de áudio (streaming vs. in-memory) | P1 | AU-001 | M | 1d | Uso de RAM de áudio dentro do budget definido |
| OPT-015 | Otimizar build size (Addressables, compressão de assets) | P0 | — | L | 2d | Build final dentro da meta de tamanho (< 200MB inicial, ex.) |
| OPT-016 | Implementar Addressables para carregamento assíncrono de biomas | P1 | ART-018 | L | 2d | Assets de bioma carregados sob demanda, sem travar main thread |
| OPT-017 | Perfilar e otimizar consumo de bateria (perfil de energia do dispositivo) | P1 | — | L | 2d | Consumo de bateria por sessão dentro de referência de mercado |
| OPT-018 | Otimizar uso de câmera AR (resolução de captura vs. performance) | P1 | AR-001 | M | 1d | Resolução ajustada por tier sem perda perceptível de qualidade AR |
| OPT-019 | Reduzir custo de CPU de UI World Space (canvas batching) | P1 | UI-001 | M | 1d | Draw calls de UI dentro do budget definido |
| OPT-020 | Implementar simplificação de HUD para tier baixo | P2 | UI-030 | M | 1d | Elementos não essenciais ocultos automaticamente |
| OPT-021 | Otimizar animações (compressão de curvas, redução de bones) | P1 | ART-014 | M | 1d | Footprint de memória de animação reduzido sem perda perceptível |
| OPT-022 | Implementar culling agressivo de objetos fora do FOV da câmera AR | P1 | — | M | 1d | Objetos fora de vista não processam lógica custosa (Update leve) |
| OPT-023 | Otimizar chamadas de rede (batching de eventos de analytics) | P1 | 12_analytics | M | 1d | Eventos agrupados em lote, reduzindo overhead de rede |
| OPT-024 | Implementar cache de assets remotos (Remote Config, ranking) | P1 | BK-012 | M | 1d | Cache reduz chamadas redundantes de rede |
| OPT-025 | Validar consumo de memória total em sessão típica por tier | P0 | OPT-009 | M | 1d | Uso de RAM dentro do limite seguro por tier (evitar OOM kill) |
| OPT-026 | Otimizar tempo de boot/inicialização do app | P1 | — | M | 1d | Tempo até menu principal dentro de meta definida (< 5s) |
| OPT-027 | Implementar telemetria de performance em produção (crash-free rate, FPS real) | P0 | 12_analytics | M | 1d | Dados de performance real coletados pós-lançamento |
| OPT-028 | Revisar e otimizar VFX Graph para tiers baixos (fallback simplificado) | P1 | PH-013 | M | 1d | Efeitos simplificados mantêm legibilidade de gameplay |
| OPT-029 | Otimizar geração procedural de Rifts (custo de Poisson-disc em runtime) | P1 | AR-018 | M | 1d | Geração de padrão de Rifts não causa hitch perceptível |
| OPT-030 | Executar auditoria final de performance pré-release | P0 | QA-021..QA-023 | L | 2d | Todas metas de FPS/memória/bateria atingidas nos 3 tiers |

---

## Resumo de Escopo

| Categoria | Qtd. de Tarefas |
|---|---|
| Gameplay (GP) | 42 |
| Interface (UI) | 30 |
| Realidade Aumentada (AR) | 27 |
| Física (PH) | 22 |
| Inteligência Artificial (AI) | 25 |
| Áudio (AU) | 20 |
| Arte (ART) | 30 |
| Backend (BK) | 25 |
| Testes (QA) | 50 |
| Otimização (OPT) | 30 |
| **Total** | **301** |

## Changelog

- 2026-08-03 — Criação inicial do backlog (parte 1: Gameplay + UI).
- 2026-08-03 — Adicionadas seções AR e Física.
- 2026-08-03 — Adicionadas seções IA e Áudio.
- 2026-08-03 — Adicionadas seções Arte e Backend.
- 2026-08-03 — Adicionadas seções Testes e Otimização; backlog completo com 301 tarefas.
- 2026-08-03 — Tarefas adicionais em AR, IA, Backend e Física para consolidar escopo final (301 tarefas).
