# ⚠️ Permissão Necessária para Adicionar Workflow

## Problema Identificado

Seu token GitHub **não possui a permissão `workflow`** necessária para criar arquivos de workflow via API.

**Scopes atuais:** `gist`, `read:org`, `repo`
**Scopes necessários:** `gist`, `read:org`, `repo`, **`workflow`**

---

## Solução: Adicionar Workflow Manualmente

### Passo 1: Acessar o GitHub
1. Abra: https://github.com/YanVGBdev/breach-ar
2. Clique em **"Add file"** > **"Create new file"**

### Passo 2: Nomear o Arquivo
No campo de nome, digite exatamente:
```
.github/workflows/unity-build.yml
```

### Passo 3: Colar o Conteúdo
Copie e cole todo o conteúdo abaixo:

```yaml
name: Unity Build

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  workflow_dispatch:
    inputs:
      buildTarget:
        description: 'Build target'
        required: true
        default: 'Android'
        type: choice
        options:
          - Android
          - iOS
          - WebGL

env:
  UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}

jobs:
  test:
    name: Run Tests
    runs-on: ubuntu-latest
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true

      - name: Cache Unity Library
        uses: actions/cache@v4
        with:
          path: Library
          hashFiles: Library/** 

      - name: Test Mode
        id: testsMode
        uses: game-ci/unity-test-runner@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
        with:
          projectPath: .
          testMode: EditMode
          artifactsPath: artifacts/test-results
          githubToken: ${{ secrets.GITHUB_TOKEN }}
          checkName: EditMode Test Results

      - name: Upload Test Results
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: Test Results (EditMode)
          path: artifacts/test-results

  buildAndroid:
    name: Build Android
    runs-on: ubuntu-latest
    needs: test
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true

      - name: Cache Unity Library
        uses: actions/cache@v4
        with:
          path: Library
          hashFiles: Library/** 

      - name: Build Android Player
        id: buildAndroid
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          targetPlatform: Android
          androidExportType: androidPackage
          versioning: Custom
          version: 0.1.0

      - name: Upload Android Build
        uses: actions/upload-artifact@v4
        with:
          name: Android Build
          path: build/Android
          retention-days: 7

  buildWebGL:
    name: Build WebGL
    runs-on: ubuntu-latest
    needs: test
    if: github.event.inputs.buildTarget == 'WebGL'
    steps:
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true

      - name: Cache Unity Library
        uses: actions/cache@v4
        with:
          path: Library
          hashFiles: Library/** 

      - name: Build WebGL Player
        id: buildWebGL
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          targetPlatform: WebGL
          versioning: Custom
          version: 0.1.0

      - name: Upload WebGL Build
        uses: actions/upload-artifact@v4
        with:
          name: WebGL Build
          path: build/WebGL
          retention-days: 7
```

### Passo 4: Commit
1. Clique em **"Commit new file"**
2. Confirme o commit

---

## Próximo Passo: Configurar Secrets

Após adicionar o workflow, configure os secrets em:
https://github.com/YanVGBdev/breach-ar/settings/secrets/actions

### Secrets Necessários:

| Nome | Descrição | Como obter |
|------|-----------|------------|
| `UNITY_LICENSE` | Licença Unity | Exportar do Unity Editor |
| `UNITY_EMAIL` | Email da conta Unity | Sua conta Unity |
| `UNITY_PASSWORD` | Senha da conta Unity | Sua senha Unity |

---

## Para Obter a Licença Unity

### Opção 1: Via Unity Editor
1. Abra o Unity
2. Vá em **Edit > Manage License**
3. Clique em **Manual activation**
4. Exporte como arquivo `.ulf`
5. Copie o conteúdo

### Opção 2: Via Terminal (se Unity instalado)
```bash
cat ~/.local/share/unity3d/Unity/Unity_lic.ulf
```

---

## Verificar se Funcionou

Após adicionar o workflow e os secrets:
1. Vá para: https://github.com/YanVGBdev/breach-ar/actions
2. Clique em **"Unity Build"**
3. Clique em **"Run workflow"**
4. Selecione **"Android"**
5. Aguarde o build (~15-30 minutos)
6. Baixe o APK em **Artifacts**
