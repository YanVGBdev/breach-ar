# QA-020: Guia de Profiling de Performance

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 1. Configuração do Unity Profiler

### Instalação
1. Abrir Unity
2. Window > Analysis > Profiler
3. Conectar dispositivo via USB
4. Selecionar "Development Build" no Player Settings

### Configuração Recomendada
```
- Deep Profile: Habilitado
- Autoconnect Profiler: Habilitado
- Script Debugging: Habilitado
- Strip Engine Code: Desabilitado (para debug)
```

---

## 2. Métricas a Monitorar

### CPU
| Métrica | Target | Alerta |
|---------|--------|--------|
| **Geral** | < 33ms | > 40ms |
| **Gameplay Logic** | < 10ms | > 15ms |
| **Physics** | < 5ms | > 8ms |
| **AI** | < 5ms | > 8ms |
| **Rendering** | < 10ms | > 15ms |
| **GC** | < 1ms | > 5ms |

### GPU
| Métrica | Target | Alerta |
|---------|--------|--------|
| **Draw Calls** | < 100 | > 150 |
| **Triangles** | < 100k | > 200k |
| **Overdraw** | < 2x | > 3x |
| **Fill Rate** | < 100 Mpixels | > 200 Mpixels |

### Memory
| Métrica | Target | Alerta |
|---------|--------|--------|
| **Total** | < 500MB | > 700MB |
| **GC Heap** | < 100MB | > 150MB |
| **Textures** | < 100MB | > 200MB |
| **Audio** | < 40MB | > 80MB |

---

## 3. Ferramentas

### Unity Profiler
- **CPU Profiler**: Identificar gargalos de CPU
- **GPU Profiler**: Analisar draw calls e fill rate
- **Memory Profiler**: Detectar memory leaks
- **Audio Profiler**: Monitorar uso de áudio

### Frame Debugger
- Analisar cada draw call
- Identificar overdraw
- Verificar batching

### Memory Profiler
- Snapshot de memória
- Comparar snapshots
- Identificar alocações

---

## 4. Comandos Úteis

### Profiling via CLI
```bash
# Build com profiling habilitado
unity -batchmode -nographics -projectPath . -executeMethod BuildScript.BuildWithProfiling

# Capturar profiler data
adb shell am profile start <package> /data/local/tmp/profile.data
```

### Análise de Logs
```bash
# Filtrar logs de performance
adb logcat | grep -i "fps\|memory\|gc\|performance"
```

---

## 5. Checklist de Profiling

### Pré-Profiling
- [ ] Build comDevelopment Build habilitado
- [ ] Profiler conectado ao dispositivo
- [ ] Cenário de teste definido

### Durante Profiling
- [ ] Rodar cenário por 5+ minutos
- [ ] Capturar múltiplos frames
- [ ] Testar diferentes tiers de dispositivo

### Pós-Profiling
- [ ] Analisar gargalos identificados
- [ ] Documentar findings
- [ ] Criar tickets para correções

---

## 6. Relatório de Profiling Template

```markdown
## Relatório de Profiling - [Data]

### Dispositivo
- Modelo: 
- Tier: 
- OS: 

### Resultados
| Métrica | Valor | Target | Status |
|---------|-------|--------|--------|
| FPS Médio | | 30/45/60 | |
| Frame Time | | < 33ms | |
| Memory | | < 500MB | |
| GC Alloc | | < 1KB/frame | |

### Gargalos Identificados
1. 
2. 

### Recomendações
1. 
2. 
```

---

## Changelog

- 2026-08-03 — Criação inicial do guia de profiling.
