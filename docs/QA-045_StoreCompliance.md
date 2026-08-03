# QA-045: Checklist de Compatibilidade com Lojas

**Status:** Draft | **Última atualização:** 2026-08-03

---

## 1. Google Play Store

### Conteúdo
- [ ] Classificação etária correta (IARC)
- [ ] Descrição precisa do jogo
- [ ] Screenshots representativas
- [ ] Ícone de alta qualidade (512x512)
- [ ] Video trailer (opcional mas recomendado)

### Privacidade
- [ ] Política de privacidade acessível
- [ ] Formulário de dados do usuário preenchido
- [ ] Consentimento LGPD/GDPR implementado
- [ ] Opção de exclusão de conta disponível

### Técnicas
- [ ] Target SDK mínimo: 33 (Android 13)
- [ ] Target API máximo: 34
- [ ] Suporte a 64-bit (ARM64)
- [ ] App Bundle (AAB) format
- [ ] Tamanho máximo: 150MB (APK) / sem limite (AAB)

### IAP
- [ ] Todos os IAPs declarados
- [ ] Preços corretos para região
- [ ] Fluxo de restauração implementado
- [ ] Política de reembolso clara

### Testes
- [ ] Build testada em dispositivo real
- [ ] Sem crashes no launch
- [ ] Performance aceitável

---

## 2. Apple App Store

### Conteúdo
- [ ] Classificação etária correta (Age Rating)
- [ ] Descrição precisa
- [ ] Screenshots para todos os tamanhos
- [ ] Ícone 1024x1024
- [ ] App Preview video

### Privacidade
- [ ] Privacy Nutrition Labels preenchidos
- [ ] App Tracking Transparency (ATT) implementado
- [ ] Política de privacidade acessível
- [ ] Opção de exclusão de conta

### Técnicas
- [ ] iOS 16.0+ mínimo
- [ ] Arquitetura: ARM64
- [ ].bitcode: Desabilitado
- [ ] Sem APIs destruídas

### IAP
- [ ] In-App Purchase configurado
- [ ] Receipt validation implementada
- [ ] Family Sharing habilitado (se aplicável)

### Testes
- [ ] TestFlight beta testada
- [ ] Sem warnings no Xcode
- [ ] Performance aceitável

---

## 3. LGPD/GDPR Compliance

### Dados Coletados
- [ ] Dados de gameplay (necessário para funcionamento)
- [ ] Dados de analytics (requer consentimento)
- [ ] Dados de ads (requer consentimento)

### Direitos do Usuário
- [ ] Consentimento explícito antes de coletar dados
- [ ] Opção de revogar consentimento
- [ ] Opção de exportar dados
- [ ] Opção de deletar conta e dados

### Documentação
- [ ] Política de privacidade atualizada
- [ ] Termos de uso claros
- [ ] Contato do DPO/encarregado

---

## 4. Processo de Submissão

### Android
1. Gerar AAB signed
2. Upload no Google Play Console
3. Preencher listing
4. Configurar pricing
5. Submeter para revisão

### iOS
1. Archive no Xcode
2. Upload no App Store Connect
3. Preencher metadata
4. Configurar pricing
5. Submeter para revisão

---

## 5. Checklist Final

### Pré-submissão
- [ ] Todos os testes passaram
- [ ] Performance dentro do budget
- [ ] Sem crashes
- [ ] Privacidade comply
- [ ] IAP funcionais

### Durante revisão
- [ ] Responder a perguntas do revisor
- [ ] Fornecer credenciais de teste
- [ ] Documentar features

### Pós-aprovação
- [ ] Monitorar reviews
- [ ] Responder feedback
- [ ] Preparar atualizações

---

## Changelog

- 2026-08-03 — Criação inicial do checklist.
