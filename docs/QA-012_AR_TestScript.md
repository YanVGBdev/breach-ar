# QA-012: Roteiro de Teste Manual de AR

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 1. Pré-requisitos

### Dispositivos Necessários
- [ ] Android com ARCore (ex: Samsung S10 ou superior)
- [ ] iOS com ARKit (ex: iPhone XS ou superior)
- [ ] Dispositivo tier baixo para teste de fallback

### Configuração
- [ ] Build instalada no dispositivo
- [ ] Permissões de câmera concedidas
- [ ] Ambiente com iluminação adequada

---

## 2. Cenários de Teste

### 2.1 Sala Pequena (3x3m)
**Objetivo:** Validar detecção de planos em espaço limitado

| # | Ação | Resultado Esperado | Status |
|---|------|-------------------|--------|
| 1 | Apontar câmera para o chão | Plano horizontal detectado | ☐ |
| 2 | Mover câmera lentamente | Progresso de scan atualiza | ☐ |
| 3 | Apontar para parede | Plano vertical detectado | ☐ |
| 4 | Completar scan | UI mostra "Scan Complete" | ☐ |
| 5 | Tocar no chão para posicionar Core | Core ancorado corretamente | ☐ |

### 2.2 Sala Grande (6x6m+)
**Objetivo:** Validar distribuição de Rifts

| # | Ação | Resultado Esperado | Status |
|---|------|-------------------|--------|
| 1 | Completar scan | Múltiplos planos detectados | ☐ |
| 2 | Iniciar jogo | Rifts distribuídas uniformemente | ☐ |
| 3 | Verificar distância mínima | Rifts não sobrepostas | ☐ |
| 4 | Jogar por 5 minutos | Performance estável | ☐ |

### 2.3 Ambiente com Móveis
**Objetivo:** Validar classificação de superfícies

| # | Ação | Resultado Esperado | Status |
|---|------|-------------------|--------|
| 1 | Apontar para mesa | Classificado como Furniture | ☐ |
| 2 | Apontar para chão | Classificado como Floor | ☐ |
| 3 | Apontar para teto | Classificado como Ceiling | ☐ |
| 4 | Verificar Rifts | Spawnam em superfícies válidas | ☐ |

### 2.4 Teste de Oclusão
**Objetivo:** Validar oclusão AR

| # | Ação | Resultado Esperado | Status |
|---|------|-------------------|--------|
| 1 | Colocar objeto real entre câmera e Fragment | Fragment oculto corretamente | ☐ |
| 2 | Mover objeto real | Oclusão atualiza em tempo real | ☐ |
| 3 | Remover objeto real | Fragment visível novamente | ☐ |

### 2.5 Teste de Tracking
**Objetivo:** Validar recuperação de tracking

| # | Ação | Resultado Esperado | Status |
|---|------|-------------------|--------|
| 1 | Cobrir câmera com mão | Tracking perdido detectado | ☐ |
| 2 | Remover mão | Tracking recuperado automaticamente | ☐ |
| 3 | Mover dispositivo rapidamente | Sem crash ou freeze | ☐ |

---

## 3. Coleta de Dados

### Performance
| Métrica | Tier Baixo | Tier Médio | Tier Alto |
|---------|------------|------------|-----------|
| FPS médio | | | |
| FPS mínimo | | | |
| Tempo de scan | | | |
| Memória usada | | | |

### Bugs Encontrados
| # | Descrição | Severidade | Reprodutível |
|---|-----------|------------|--------------|
| | | | |

---

## 4. Checklist Final

- [ ] Todos os cenários executados
- [ ] Performance dentro do budget
- [ ] Sem crashes
- [ ] Bugs registrados
- [ ] Screenshots/vídeos documentados

---

## Changelog

- 2026-08-03 — Criação inicial do roteiro de teste.
