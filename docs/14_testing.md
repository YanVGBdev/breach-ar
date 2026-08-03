# 14. Testes

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 14.1 Estratégia Geral

Pirâmide de testes adaptada para jogo AR mobile solo-dev:
- Base ampla: **testes unitários** de lógica pura (combat math, combo, economia, DDA).
- Meio: **testes de integração** (Unity Test Framework, PlayMode) para sistemas interagindo (SpawnSystem + ComboSystem + Events).
- Topo estreito: **testes manuais em dispositivo real** para AR (não automatizável de forma confiável no estado atual das ferramentas).

## 14.2 Casos de Teste (exemplos representativos)

| ID | Caso | Resultado esperado |
|---|---|---|
| TC-001 | Lançar Orbe sem alvo no caminho | Orbe segue trajetória balística e expira após N ricochetes/timeout |
| TC-002 | Orbe atinge Fragment diretamente | Fragment recebe dano correto conforme `OrbDefinitionSO`; morre se HP ≤ 0 |
| TC-003 | Fragment alcança o Core | Core perde HP correspondente; combo é resetado |
| TC-004 | Combo em janela válida (< 2.5s entre kills) | Multiplicador incrementa +0.1 corretamente |
| TC-005 | Combo fora da janela | Multiplicador reseta para x1.0 |
| TC-006 | HP do Core chega a 0 | Estado transiciona para `GameOver`/`RunFailed` corretamente |
| TC-007 | Scan AR sem superfícies suficientes | Mensagem de orientação exibida, não trava o fluxo |
| TC-008 | Rescan durante pausa | Ambiente é re-escaneado sem perder progresso da sessão atual |
| TC-009 | Boss perde todos weak points | Transição de fase ocorre corretamente |
| TC-010 | Compra de IAP falha (rede instável) | Estado de compra não é creditado localmente sem confirmação server-side |

*(Casos completos mantidos em planilha/ferramenta de test management vinculada; este arquivo mantém amostra representativa por categoria.)*

## 14.3 Testes Unitários

- Framework: Unity Test Framework (EditMode).
- Cobertura-alvo prioritária: `ComboSystem`, `DamageCalculator`, `DifficultyDirector`, `EconomyService`, `SaveRepository` (serialização/merge de conflito).
- Regra: toda lógica que não depende de `MonoBehaviour`/física deve ser extraída para classes puras testáveis em EditMode (reforça padrão de arquitetura, `04_architecture.md`).

## 14.4 Testes de Integração

- Framework: Unity Test Framework (PlayMode), cena de teste isolada sem dependência de sessão AR real (usa `IARSurfaceProvider` mockado com planos sintéticos).
- Cenários: fluxo completo de onda (spawn → combate → wave cleared), fluxo de Game Over completo, fluxo de compra de IAP mockada.

## 14.5 Testes de AR

- **Manuais, em dispositivo físico**, roteiro padronizado cobrindo variedade de ambientes:
  - Sala pequena (< 10m²) com pouca luz.
  - Sala grande e aberta.
  - Ambiente com muitos móveis/objetos pequenos (teste de classificação de superfícies).
  - Ambiente com superfícies reflexivas/vidro (caso de falha conhecida de ARCore — validar comportamento degradado).
  - Teste de oclusão: objeto real entre jogador e Fragment deve ocultar corretamente o Fragment.
  - Teste de perda de tracking (mover rapidamente o dispositivo) e recuperação.
- Checklist de regressão AR executado a cada build candidata a release (Alpha/Beta/Release).

## 14.6 Testes de Performance

- Profiling via Unity Profiler + Frame Debugger em dispositivos de referência por tier (`16_optimization.md`).
- Métricas-alvo: FPS estável (ver 16.1), sem GC spikes > 5ms durante gameplay ativo, tempo de carregamento de cena < 5s em dispositivo tier médio.
- Teste de sessão longa (30+ min contínuos) para detectar memory leaks (comparar uso de memória no início/fim).

## 14.7 Testes em Diferentes Dispositivos

- **Device matrix mínima** para QA manual: 1 dispositivo tier baixo (sem Depth API), 1 tier médio (com Depth API, GPU mid-range), 1 tier alto (flagship recente), coberto por pelo menos 2 fabricantes (Samsung + Google/Xiaomi) para variação de câmera/ARCore implementation.
- Firebase Test Lab (ou similar) para smoke tests automatizados de boot/crash em matriz ampliada de dispositivos antes de cada release.

## Changelog

- 2026-08-03 — Criação inicial do documento.
