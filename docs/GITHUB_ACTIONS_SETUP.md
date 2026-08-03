# Guia de Setup - GitHub Actions para Unity

## Pré-requisitos

1. **Conta GitHub** (gratuita)
2. **Unity License** (Personal ou Plus)
3. **Repositório GitHub** com o projeto

---

## Passo 1: Criar Repositório

```bash
# Inicializar git
cd /home/yan/breach-ar
git init
git add .
git commit -m "Initial commit"
git remote add origin https://github.com/SEU_USERNAME/breach-ar.git
git push -u origin main
```

---

## Passo 2: Configurar Secrets no GitHub

Vá em `Settings > Secrets and variables > Actions` e adicione:

### Unity Secrets

| Nome | Descrição | Como obter |
|------|-----------|------------|
| `UNITY_LICENSE` | Licença Unity (JSON) | Exportar do Unity Editor |
| `UNITY_EMAIL` | Email da conta Unity | Sua conta Unity |
| `UNITY_PASSWORD` | Senha da conta Unity | Sua senha Unity |

### Android Secrets (para build APK)

| Nome | Descrição | Como obter |
|------|-----------|------------|
| `ANDROID_KEYSTORE_NAME` | Nome do keystore | Criar keystore |
| `ANDROID_KEYSTORE_BASE64` | Keystore em base64 | `base64 -w 0 my.keystore` |
| `ANDROID_KEYSTORE_PASS` | Senha do keystore | Ao criar keystore |
| `ANDROID_KEYALIAS_NAME` | Nome do alias | Ao criar keystore |
| `ANDROID_KEYALIAS_PASS` | Senha do alias | Ao criar keystore |

---

## Passo 3: Exportar Unity License

### Opção A: Via Unity Editor
1. Abra o Unity
2. Vá em `Edit > Manage License`
3. Clique em `Manual activation`
4. Exporte a licença como arquivo `.ulf`
5. Copie o conteúdo do arquivo

### Opção B: Via Terminal
```bash
# Se já tem Unity instalado
cat ~/.local/share/unity3d/Unity/Unity_lic.ulf
```

### Configurar no GitHub
1. Copie todo o conteúdo do arquivo `.ulf`
2. No GitHub, vá em `Settings > Secrets > Actions`
3. Adicione `UNITY_LICENSE` com o conteúdo copiado

---

## Passo 4: Criar Keystore Android (Opcional)

```bash
# Gerar keystore
keytool -genkey -v -keystore breachar.keystore \
  -alias breachar -keyalg RSA -keysize 2048 -validity 10000

# Converter para base64
base64 -w 0 breachar.keystore > breachar.keystore.base64

# Criar secrets no GitHub com os valores
```

---

## Passo 5: Disparar Build

### Build Automático
- **Push para main**: Build Android automático
- **Pull Request**: Testes automáticos

### Build Manual
1. Vá em `Actions`
2. Selecione `Unity Build`
3. Clique em `Run workflow`
4. Selecione o target (Android/iOS/WebGL)
5. Clique em `Run workflow`

---

## Passo 6: Baixar Build

1. Vá em `Actions`
2. Clique na build desejada
3. Em `Artifacts`, baixe o build gerado

---

## Estrutura de Arquivos

```
.github/
└── workflows/
    └── unity-build.yml
```

---

## Troubleshooting

### Erro: "License not found"
- Verifique se `UNITY_LICENSE` está correto
- Exporte novamente a licença do Unity

### Erro: "Build failed"
- Verifique os logs no GitHub Actions
- Certifique-se de que o projeto compila localmente

### Erro: "Android keystore"
- Verifique se todos os secrets estão configurados
- Teste o keystore localmente primeiro

---

## Custos

| Serviço | Limite Gratuito |
|---------|-----------------|
| GitHub Actions | 2.000 minutos/mês |
| Unity Build Service | 100 minutos/mês (Personal) |
| GitHub Pages | 1GB Storage |

---

## Referências

- [GameCI - Unity on GitHub Actions](https://game.ci/docs/github/quick-start)
- [Unity Manual - Build Automation](https://docs.unity3d.com/Manual/UnityCloudBuild.html)
- [GitHub Actions Docs](https://docs.github.com/en/actions)
