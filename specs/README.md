# specs/ — Especificações Curtas por Sistema

Cada arquivo nesta pasta descreve **um único sistema** do jogo em menos de 1 página, para ser consumido por agentes de IA sem precisar carregar o GDD inteiro. Specs não substituem `04_architecture.md` (padrões gerais) nem `15_backlog.md` (escopo/tarefas) — eles são o "resumo executável" de um sistema específico.

## Quando criar um spec novo

- Ao implementar um sistema novo listado em `15_backlog.md` que ainda não tem spec.
- Um spec = um sistema coeso (ex: `RiftSystem`, não `RiftController` isoladamente).

## Quando atualizar um spec existente

- Sempre que a implementação divergir do spec durante o desenvolvimento (mesma tarefa/commit).
- Sempre que um sistema passar a emitir/consumir um evento novo.

## Template

```markdown
# <NomeDoSistema>

## Objetivo
1–2 frases: por que este sistema existe, que problema de gameplay ele resolve.

## Responsabilidades
- O que este sistema FAZ (lista curta).
- O que este sistema explicitamente NÃO faz (evita escopo ambíguo).

## Dependências
- Outros sistemas/serviços dos quais este depende (injetados via DI).
- ScriptableObjects que este sistema lê.

## Eventos Emitidos
| Evento | Payload | Quando |
|---|---|---|

## Eventos Consumidos
| Evento | Origem | Reação |
|---|---|---|

## Classes/Componentes Envolvidos
| Classe | Papel |
|---|---|

## Referências
- Doc(s) do GDD relevante(s): `0X_arquivo.md §Y.Z`
- Tarefas de backlog: `XX-###`
```

## Índice de Specs

| Spec | Sistema | Status |
|---|---|---|
| [RiftSystem.md](RiftSystem.md) | Rifts (fendas, integridade, spawn de Fragments) | ✅ |
| [EnemySpawner.md](EnemySpawner.md) | Composição e spawn de ondas de Fragments | ✅ |
| [OrbLaunch.md](OrbLaunch.md) | Lançamento e voo de Orbes (input + física) | ✅ |
| [ComboSystem.md](ComboSystem.md) | Multiplicador de combo | ✅ |
| [CoreSystem.md](CoreSystem.md) | HP do Core e condição de derrota | ✅ |
| [DifficultyDirector.md](DifficultyDirector.md) | Ajuste dinâmico de dificuldade | ✅ |
| [ARSurfaceService.md](ARSurfaceService.md) | Scan, classificação e anchors de superfícies | ✅ |
| [HUD.md](HUD.md) | Interface durante gameplay ativo | ✅ |

*(Todos os specs foram criados conforme o plano original.)*
