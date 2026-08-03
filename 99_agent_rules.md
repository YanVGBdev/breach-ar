# 99. Regras para Agentes de IA

**Status:** Draft | **Última atualização:** 2026-08-03

---

Este arquivo é **leitura obrigatória** para qualquer agente de IA (Claude Code, Cursor, Copilot Workspace, etc.) antes de executar qualquer tarefa de implementação neste projeto. Ele complementa — nunca substitui — `04_architecture.md` e o `15_backlog.md`.

## 99.1 Escopo da Tarefa

1. **Nunca altere código que não esteja relacionado à tarefa atual.** Se notar um bug ou débito técnico fora do escopo, registre como comentário no PR/commit ou como nova entrada de backlog — não corrija "de passagem".
2. **Sempre consulte `15_backlog.md` antes de começar.** Se a tarefa não tem um ID de backlog correspondente, crie a entrada primeiro (mesma tabela/categoria, próximo ID sequencial) antes de escrever código.
3. **Sempre consulte o `specs/` do sistema envolvido** (ver seção 99.5) antes de tocar em um sistema existente. Se o spec não existir ainda, crie-o como parte da tarefa (é mais barato que o agente ler o projeto inteiro).
4. Uma tarefa = um commit/PR coeso. Não misture múltiplos IDs de backlog no mesmo commit sem necessidade clara.

## 99.2 Código

5. **Nunca crie scripts duplicados.** Antes de criar uma nova classe/arquivo, procure por algo equivalente já existente na pasta correspondente (`04_architecture.md` §4.2).
6. **Sempre reutilize componentes, serviços e ScriptableObjects existentes** em vez de recriar lógica equivalente.
7. **Nunca adicione uma nova dependência (pacote/asset/lib)** sem justificar explicitamente por que a stack atual (`04_architecture.md`) não resolve, e sem propor um ADR quando a dependência for estrutural.
8. **Siga os princípios SOLID** e os padrões definidos em `04_architecture.md` §4.3 (DI via VContainer, eventos via SO, sem Singleton estático, Strategy/Command onde aplicável).
9. **Use ScriptableObjects para qualquer dado configurável/balanceável** (dano, custo, curvas, waves) — nunca hardcode valores de balanceamento em código.
10. **Nunca deixe código comentado** ("código morto"). Remova ou não inclua; histórico fica no Git.
11. Nomenclatura de eventos e classes deve seguir as convenções já estabelecidas em `04_architecture.md` §4.7 (`On<Sujeito><Ação>`, `Request<Ação>`).

## 99.3 Testes

12. **Sempre gere testes quando possível** — no mínimo, testes unitários (EditMode) para qualquer lógica pura nova (cálculo, sistema de dados, regra de negócio), conforme `14_testing.md` §14.3.
13. Toda tarefa de sistema que interage com outros sistemas via eventos deve incluir (ou atualizar) um teste de integração (PlayMode) correspondente.
14. Não marque uma tarefa do backlog como concluída sem que os critérios de aceite definidos na tabela sejam verificáveis (por teste automatizado ou passo manual documentado).

## 99.4 Documentação

15. **Sempre atualize a documentação relevante após concluir uma tarefa**: o `Changelog` do arquivo de domínio afetado (`0X_*.md`), o spec correspondente em `specs/` (seção 99.5) e, se aplicável, o status da tarefa no `15_backlog.md`.
16. Decisões estruturais (nova lib, mudança de padrão, mudança de pipeline) exigem um ADR em `/docs/adr/NNN-titulo.md` — não altere decisões de ADRs `Accepted` sem propor um substituto.
17. Nunca deixe um spec desatualizado em relação ao código — se a implementação divergir do spec durante o desenvolvimento, atualize o spec como parte da mesma tarefa.

## 99.5 Uso da Pasta `specs/`

- `specs/` contém um documento curto por sistema (não por arquivo de código) — ver estrutura em `specs/README.md`.
- Para qualquer tarefa que **modifique** um sistema existente: leia apenas `04_architecture.md` (contexto geral) + o spec específico do sistema — **não** é necessário ler o GDD completo.
- Para qualquer tarefa que **crie** um sistema novo: crie o spec correspondente como parte da tarefa, seguindo o template em `specs/README.md`.
- Specs devem permanecer curtos (idealmente < 1 página) — se um spec cresce demais, é sinal de que o sistema deveria ser dividido.

## 99.6 O Que Fazer Quando Algo Não Está Claro

- Se a tarefa do backlog for ambígua, escolha a interpretação mais alinhada ao `01_vision.md` e ao spec do sistema, documente a suposição feita no commit/PR, e prossiga — não bloqueie a implementação esperando esclarecimento, a menos que a ambiguidade envolva economia, monetização ou segurança (`17_security.md`), casos em que deve-se sinalizar explicitamente antes de prosseguir.
- Nunca invente escopo novo não presente no backlog "porque parecia uma boa ideia" — proponha como nova entrada de backlog em vez de implementar direto.

## Changelog

- 2026-08-03 — Criação inicial do documento.
