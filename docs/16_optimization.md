# 16. Otimização

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 16.1 FPS

- **Device Tiers** (classificação em runtime via `SystemInfo` + benchmark rápido no boot):
  - **Tier Baixo**: sem Depth API, GPU Adreno 5xx/Mali-G7x — meta **30 FPS**.
  - **Tier Médio**: com Depth API, GPU Adreno 6xx/Mali-G7x+ — meta **45 FPS**.
  - **Tier Alto**: flagship recente — meta **60 FPS**.
- Qualidade gráfica (sombras, resolução de VFX, densidade de partículas, resolução de render) ajustada automaticamente por tier, com override manual disponível em Configurações.
- Frame pacing consistente priorizado sobre pico de FPS (evitar variação perceptível frame a frame, especialmente crítico em AR pois tracking degrada com jitter).

## 16.2 Memória

- Budget de RAM alvo: Tier Baixo ≤ 700MB, Tier Médio ≤ 1GB, Tier Alto ≤ 1.5GB (margem de segurança contra OOM kill do SO).
- Texturas comprimidas (ASTC), mipmaps habilitados, atlas de textura para reduzir draw calls e footprint.
- Áudio: streaming para música (não carregada inteira em memória), in-memory apenas para SFX curtos.
- Monitoramento contínuo via Unity Memory Profiler durante desenvolvimento; telemetria de memória real em produção (`OPT-027`).

## 16.3 Bateria

- Depth API e câmera AR são os maiores consumidores de energia — sampling rate reduzido em tier baixo (`OPT-012`).
- Evitar renderização desnecessária em telas estáticas (menus) — reduzir `Application.targetFrameRate` fora do gameplay ativo.
- Alertar o jogador (opcional, configurável) se sessão de AR ultrapassar tempo prolongado (aquecimento de dispositivo é fator real em apps AR contínuos).

## 16.4 GPU

- Overdraw controlado (especialmente crítico com materiais translúcidos/emissivos do estilo visual do jogo) — revisão via Frame Debugger (`OPT-006`).
- VFX Graph (GPU particles) preferido sobre CPU particles para efeitos de alto volume, com fallback simplificado em tier baixo.
- Shader de oclusão AR otimizado para custo mínimo por pixel (é executado em praticamente todo objeto virtual da cena).

## 16.5 CPU

- Object pooling obrigatório para todos os objetos de spawn frequente (Fragments, Orbes, VFX, popups de UI).
- Mesh collider de planos AR atualizado com throttle (não a cada frame).
- Pathfinding (flow field) recalculado em intervalo (1–2s), não por frame.
- Física em fixed timestep consistente, evitando picos de custo em `FixedUpdate`.

## 16.6 Assets

- Uso de **Addressables** para carregamento assíncrono de conteúdo por bioma — apenas o bioma ativo é mantido em memória.
- Modelos com budget de poly rígido por categoria (ver `09_art.md` §9.4).
- Auditoria periódica de assets não utilizados (referências órfãs) antes de cada build de release.

## 16.7 Build Size

- Meta de build inicial (download): **< 200MB** (fundamental para conversão de instalação em mercados com dados móveis limitados).
- Conteúdo adicional (biomas futuros, eventos sazonais) entregue via Addressables remoto pós-instalação, não embutido na build inicial.
- Compressão de textura por plataforma (ASTC Android, adequado equivalente iOS) e remoção de assets de debug/editor em builds de release.

## Changelog

- 2026-08-03 — Criação inicial do documento.
