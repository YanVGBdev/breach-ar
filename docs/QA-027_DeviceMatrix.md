# QA-027: Device Matrix de QA

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 1. Matriz Mínima (3 dispositivos, 2 fabricantes)

### Android
| Dispositivo | Fabricante | Tier | RAM | GPU | ARCore | Depth API |
|-------------|------------|------|-----|-----|--------|-----------|
| **Samsung Galaxy A52** | Samsung | Medium | 4GB | Adreno 618 | ✅ | ❌ |
| **Google Pixel 4a** | Google | Medium | 6GB | Adreno 618 | ✅ | ✅ |
| **Moto G5 Plus** | Motorola | Low | 2GB | Adreno 506 | ✅ | ❌ |

### iOS (se disponível)
| Dispositivo | Fabricante | Tier | RAM | ARKit | LiDAR |
|-------------|------------|------|-----|-------|-------|
| **iPhone 12** | Apple | High | 4GB | ✅ | ❌ |
| **iPhone 12 Pro** | Apple | High | 6GB | ✅ | ✅ |
| **iPhone SE 2020** | Apple | Medium | 3GB | ✅ | ❌ |

---

## 2. Cenários de Teste por Tier

### Tier Low (Moto G5 Plus)
- [ ] FPS ≥ 30 estável
- [ ] Sem crashes em 10 min
- [ ] Oclusão via mesh fallback
- [ ] Partículas reduzidas
- [ ] Texturas quarter resolution

### Tier Medium (Galaxy A52, Pixel 4a)
- [ ] FPS ≥ 45 estável
- [ ] Oclusão via Depth API (Pixel 4a)
- [ ] Partículas moderadas
- [ ] Texturas half resolution

### Tier High (iPhone 12 Pro)
- [ ] FPS ≥ 60 estável
- [ ] Oclusão via Depth API
- [ ] Partículas completas
- [ ] Texturas full resolution

---

## 3. Testes por Dispositivo

### Samsung Galaxy A52
| Teste | Resultado | Notas |
|-------|-----------|-------|
| Boot time | | |
| Scan speed | | |
| FPS médio | | |
| Memory usage | | |
| Battery drain (10min) | | |

### Google Pixel 4a
| Teste | Resultado | Notas |
|-------|-----------|-------|
| Boot time | | |
| Scan speed | | |
| FPS médio | | |
| Depth API funcional | | |
| Memory usage | | |

### Moto G5 Plus
| Teste | Resultado | Notas |
|-------|-----------|-------|
| Boot time | | |
| Scan speed | | |
| FPS médio | | |
| Fallback oclusão | | |
| Memory usage | | |

---

## 4. Prioridade de Teste

### Alta (MVP)
1. Samsung Galaxy A52 (representante tier médio)
2. Google Pixel 4a (representante tier médio com Depth)
3. Moto G5 Plus (representante tier baixo)

### Média (Beta)
4. Samsung Galaxy S21 (tier alto Android)
5. iPhone 12 (tier alto iOS)
6. Samsung Galaxy J7 (tier baixo Android)

### Baixa (Pós-lançamento)
7. Tablets (diferente aspect ratio)
8. Dispositivos mais antigos
9. Dispositivos exóticos

---

## 5. Checklist de Dispositivo

### Pré-teste
- [ ] Dispositivo com bateria > 50%
- [ ] Armazenamento livre > 1GB
- [ ] Permissões de câmera concedidas
- [ ] ARCore/ARKit instalado

### Durante teste
- [ ] Notificações desabilitadas
- [ ] Wi-Fi desabilitado (para teste offline)
- [ ] Brilho da tela em 50%

### Pós-teste
- [ ] Logs coletados
- [ ] Screenshots documentados
- [ ] Bugs registrados

---

## Changelog

- 2026-08-03 — Criação inicial da device matrix.
