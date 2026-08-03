# 12. Analytics

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 12.1 Ferramenta

Recomendação: Firebase Analytics (ou Unity Gaming Services Analytics) para funil/engajamento + export bruto para BigQuery/Supabase para análises de balanceamento customizadas (`05_ai.md` §5.3).

## 12.2 Eventos

| Evento | Parâmetros-chave |
|---|---|
| `session_start` | device_tier, platform, ar_depth_supported |
| `ar_scan_completed` | duration_seconds, surfaces_detected (floor/wall/ceiling/furniture counts) |
| `ar_scan_failed` | reason |
| `run_started` | game_mode, biome_id |
| `wave_started` | wave_index, difficulty_delta_acumulado |
| `wave_completed` | wave_index, time_taken, core_hp_remaining |
| `fragment_killed` | fragment_type, orb_type, combo_multiplier_at_kill, via_ricochet (bool) |
| `rift_closed` | rift_surface_type (wall/floor/ceiling/furniture) |
| `powerup_collected` / `powerup_used` | powerup_type |
| `boss_defeated` / `boss_run_failed` | boss_id, time_taken |
| `core_destroyed` | wave_index, final_score |
| `run_ended` | outcome (completed/failed/quit), score, max_combo, waves_cleared |
| `iap_purchase` | item_id, price_tier, currency |
| `ad_watched` | ad_placement (revive/double_reward/refresh) |
| `battle_pass_tier_unlocked` | tier_index, track (free/premium) |

## 12.3 Métricas

- **Engajamento**: DAU/WAU/MAU, sessões por usuário/dia, duração média de sessão.
- **Gameplay**: taxa de acerto média por skill bracket, onda média alcançada, distribuição de causa de morte (tipo de Fragment que mais causou Game Over).
- **AR-específicas**: taxa de sucesso de scan, tempo médio de scan, distribuição de tipos de superfície detectados (para validar se o design está funcionando em ambientes reais variados).
- **Monetização**: ARPDAU, taxa de conversão para pagante, LTV por coorte, taxa de watch-to-completion de anúncios recompensados.
- **Performance técnica**: FPS médio por device tier, crash rate, ANR rate.

## 12.4 Retenção

- Coortes D1/D7/D30 padrão de indústria mobile.
- Metas de referência (benchmark de mercado para casual/arcade F2P): D1 ≥ 35%, D7 ≥ 12%, D30 ≥ 4% (a validar/ajustar após soft launch).
- Gatilhos de retenção: notificações push (Core "sob ameaça" — desafio diário disponível, Battle Pass prestes a expirar), recompensa de login consecutivo.

## 12.5 Sessões

- Duração-alvo de sessão: 3–6 minutos por run (Campanha/Endless), permitindo múltiplas sessões em janelas curtas (transporte público, intervalos).
- Monitorar `session_length` vs. `waves_cleared` para calibrar se a duração real está alinhada ao design (`02_gameplay.md`).

## 12.6 Conversão

- Funil de onboarding: `app_open` → `ar_scan_started` → `ar_scan_completed` → `run_started` → `run_ended` — identificar drop-off (especialmente em `ar_scan_completed`, ponto crítico de fricção específico de AR).
- Funil de monetização: `store_opened` → `item_viewed` → `purchase_initiated` → `iap_purchase`.
- A/B testing via Remote Config (`11_backend.md` §11.6) para preço, posicionamento de oferta e frequência de anúncios.

## 12.7 Privacidade e Consentimento

- Todos os eventos de analytics/ads condicionados a consentimento explícito (LGPD/GDPR/COPPA quando aplicável) coletado no primeiro acesso — ver `17_security.md`.

## Changelog

- 2026-08-03 — Criação inicial do documento.
