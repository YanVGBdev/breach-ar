# 16. Otimização

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 16.1 Device Tiers

### Classificação de Dispositivos

| Tier | RAM | GPU Memory | CPU Cores | Exemplos |
|------|-----|------------|-----------|----------|
| **Low** | < 4GB | < 1GB | < 4 | Moto G5, Samsung J7 |
| **Medium** | 4-6GB | 1-2GB | 4-6 | Samsung A52, Pixel 4a |
| **High** | > 6GB | > 2GB | > 8 | Samsung S21, iPhone 12 |

### Critérios de Detecção

A detecção é feita em runtime via `DeviceTierDetector`:

```csharp
public enum DeviceTier
{
    Low,
    Medium,
    High
}
```

---

## 16.2 Metas de FPS por Tier

| Tier | Meta FPS | Tolerância | Prioridade |
|------|----------|------------|------------|
| **Low** | 30 FPS | ±5 FPS | Estabilidade > Qualidade |
| **Medium** | 45 FPS | ±5 FPS | Balanceado |
| **High** | 60 FPS | ±3 FPS | Máxima fluidez |

### Configuração de Quality Settings

| Tier | AntiAliasing | Shadows | Textures | Particles |
|------|--------------|---------|----------|-----------|
| **Low** | Off | Off | Quarter | 50 |
| **Medium** | 2x | Hard Only | Half | 150 |
| **High** | 4x | Soft + High | Full | 300 |

---

## 16.3 Budgets de Performance

### CPU Budget (por frame a 30 FPS = 33ms)

| Sistema | Budget Low | Budget Medium | Budget High |
|---------|------------|---------------|-------------|
| **Gameplay Logic** | 10ms | 12ms | 15ms |
| **Physics** | 5ms | 5ms | 5ms |
| **AI/Pathfinding** | 3ms | 5ms | 8ms |
| **Rendering** | 10ms | 8ms | 5ms |
| **Total** | 28ms | 30ms | 33ms |

### GPU Budget

| Metrica | Low | Medium | High |
|---------|-----|--------|------|
| **Draw Calls** | < 50 | < 100 | < 200 |
| **Triangles** | < 50k | < 100k | < 200k |
| **Overdraw** | < 2x | < 3x | < 4x |
| **Texture Memory** | < 100MB | < 200MB | < 400MB |

### Memory Budget

| Tipo | Low | Medium | High |
|------|-----|--------|------|
| **Total RAM** | < 300MB | < 500MB | < 800MB |
| **GC Heap** | < 50MB | < 100MB | < 150MB |
| **Textures** | < 50MB | < 100MB | < 200MB |
| **Audio** | < 20MB | < 40MB | < 80MB |
| **Meshes** | < 20MB | < 40MB | < 80MB |

---

## 16.4 Otimizações por Tier

### Low Tier (30 FPS)

- ✅ Desabilitar sombras
- ✅ Texturas em quarter resolution
- ✅ Partículas reduzidas (50 máximo)
- ✅ Oclusão via mesh (sem Depth API)
- ✅ LOD agressivo
- ✅ Pooling otimizado
- ✅ UI simplificada

### Medium Tier (45 FPS)

- ✅ Sombras hard only
- ✅ Texturas em half resolution
- ✅ Partículas moderadas (150 máximo)
- ✅ Oclusão via Depth API (se disponível)
- ✅ LOD moderado
- ✅ UI completa

### High Tier (60 FPS)

- ✅ Sombras soft + high quality
- ✅ Texturas full resolution
- ✅ Partículas completas (300 máximo)
- ✅ Oclusão via Depth API
- ✅ LOD mínimo
- ✅ Efeitos visuais completos
- ✅ UI com animações

---

## 16.5 Profiling

### Ferramentas Recomendadas

1. **Unity Profiler** - CPU/GPU profiling
2. **Memory Profiler** - Análise de memória
3. **Frame Debugger** - Análise de draw calls
4. **XR Profiler** - Performance AR

### Métricas a Monitorar

| Métrica | Target | Alerta |
|---------|--------|--------|
| **FPS** | > 30 | < 25 |
| **Frame Time** | < 33ms | > 40ms |
| **GC Allocation** | < 1KB/frame | > 5KB/frame |
| **Draw Calls** | < 100 | > 150 |
| **Memory** | < 500MB | > 700MB |

---

## 16.6 Checklist de Otimização

### Pré-Release

- [ ] FPS estável em todos os tiers
- [ ] Sem memory leaks em sessão de 30min
- [ ] GC spikes < 5ms
- [ ] Build size < 200MB
- [ ] Tempo de boot < 5s

### Performance

- [ ] Object pooling ativo
- [ ] Shaders otimizados para mobile
- [ ] Texturas comprimidas (ASTC)
- [ ] Áudio streaming habilitado
- [ ] LOD funcionando

### AR

- [ ] Tracking estável
- [ ] Oclusão funcionando
- [ ] Mesh colliders otimizados
- [ ] Anchors limpos corretamente

---

## Changelog

- 2026-08-03 — Criação inicial do documento de otimização.
