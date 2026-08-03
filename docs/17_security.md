# 17. Segurança

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 17.1 Anti-Cheat

- **Princípio central**: nenhum dado que afete economia, ranking ou desbloqueios deve ser confiado exclusivamente ao cliente.
- Submissões de score para ranking passam por validação server-side (Edge Function) antes de persistência: checagem de plausibilidade (limite teórico de pontuação por tempo de sessão, taxa máxima de eventos/segundo, sequência lógica de eventos).
- `run_signature`: hash calculado a partir de uma sequência resumida de eventos da sessão (waves, kills, timestamps), enviado junto ao score — usado para detectar replays manipulados ou submissões forjadas sem gameplay real associado.
- Rate limiting de submissões por `player_id` (evita brute-force de tentativas de score falso).
- Cliente ofuscado/hardened (IL2CPP + ofuscação básica de assemblies) para dificultar engenharia reversa de lógica de economia local.

## 17.2 Validação

- Toda transação de IAP validada server-side via API oficial da loja (Google Play Billing Library / receipt validation) antes de creditar qualquer item/moeda — nunca confiar apenas no callback `OnPurchaseComplete` local.
- Validação de integridade do save local (checksum) para detectar edição manual de arquivo de save; em caso de inconsistência, save local é ignorado em favor do save em nuvem (se disponível) ou tratado como corrompido.
- Validação de parâmetros de Remote Config recebidos (nunca aplicar valores fora de faixas seguras predefinidas, mesmo que o backend seja comprometido).

## 17.3 Integridade dos Dados

- Comunicação cliente-backend sempre via HTTPS/TLS.
- Dados sensíveis (saldo de moeda hard, inventário de IAP) tratados como **read-mostly no cliente**: cliente exibe o estado, mas alterações relevantes (crédito de compra, desbloqueio) só são confirmadas após resposta autoritativa do backend.
- Backups periódicos automáticos do banco de dados (retenção mínima de 30 dias) para recuperação em caso de incidente.
- Logs de auditoria para toda alteração de saldo econômico (`BK-017`), permitindo rastreabilidade em disputas de suporte ao jogador.
- Conformidade com LGPD/GDPR: consentimento explícito antes de qualquer coleta de analytics/ads personalizados (`UI-026`, `BK-014`), direito de exclusão de conta/dados implementado via fluxo de suporte.
- Nenhum dado de PII desnecessário é coletado (autenticação anônima por padrão; vínculo de conta social é opcional e explícito).

## Changelog

- 2026-08-03 — Criação inicial do documento.
