# 10. Monetização

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 10.1 Modelo Geral

**Free-to-play, não pay-to-win.** Monetização baseada em cosméticos, conveniência e Battle Pass sazonal. Poder de combate (dano/HP de Orbes) é obtenível via gameplay normal; compras aceleram progressão ou desbloqueiam variações visuais, nunca vantagem numérica exclusiva.

## 10.2 Gratuito

- Acesso completo a todos os modos de jogo.
- Progressão completa da árvore de upgrades de Orbes via moeda soft (Fragmentos de Energia) ganha jogando.
- 1 Revive gratuito por dia via anúncio recompensado.

## 10.3 Premium

- **Remoção de anúncios** (compra única, ex: R$ 14,90).
- **Pacote "Fundador"**: cosmético exclusivo + moeda hard inicial + remoção de anúncios (bundle único de lançamento).

## 10.4 Cosméticos

- Skins de Orbe (trilha visual, som de impacto customizado) — não afetam dano/física.
- Skins de Core (aparência do núcleo protegido).
- Efeitos de vitória/Game Over customizados (para clipes compartilháveis).
- Temas de HUD.
- Todos vendidos via moeda hard (Cristais) comprável com dinheiro real ou ganha (raramente) via gameplay/Battle Pass.

## 10.5 Battle Pass

- Sazonal, duração de 4–6 semanas, alinhado a eventos especiais (`02_gameplay.md` §2.11).
- Trilha gratuita (recompensas moderadas: moeda soft, XP boost) + trilha premium (cosméticos exclusivos, moeda hard, Orbe temático exclusivo com variação visual/elemental — balanceado para não ser estritamente superior).
- Progressão via XP de desafios diários/semanais, não via gasto direto (evita percepção de "pay-to-progress").

## 10.6 Anúncios

- **Recompensados (opt-in)**: dobrar recompensa de fim de sessão, Revive extra, refresh de loja diária.
- **Intersticiais (limitados)**: no máximo 1 a cada 3 sessões completas, nunca durante gameplay ativo (apenas em transições de menu), com frequência configurável via `RemoteConfigService` para tuning pós-lançamento.
- Provedor: Google AdMob (Mediation) com fallback para rede secundária.
- Consentimento de rastreamento (ATT/GDPR) solicitado antes de qualquer anúncio personalizado — ver `12_analytics.md` e `17_security.md`.

## 10.7 Compras Internas (IAP)

| Item | Tipo |
|---|---|
| Pacotes de Cristais (moeda hard) | Consumível, múltiplos tiers de preço |
| Battle Pass Premium | Não-consumível, por temporada |
| Skins individuais | Não-consumível |
| Pacote Fundador | Não-consumível, único |
| Remoção de Anúncios | Não-consumível, permanente |

- Validação de compra **sempre server-side** (nunca confiar em callback local do cliente) — ver `11_backend.md` e `17_security.md`.
- Preços regionalizados via loja (Google Play Billing / App Store Connect), sem hardcode de valores no cliente.

## Changelog

- 2026-08-03 — Criação inicial do documento.
