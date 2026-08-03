# Workflow para GitHub Actions

## Como Adicionar

1. Vá para: https://github.com/YanVGBdev/breach-ar/new/main
2. Nome do arquivo: `.github/workflows/unity-build.yml`
3. Cole o conteúdo abaixo
4. Clique em "Commit new file"

---

## Conteúdo do Arquivo

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
          GOOGLE_PLAY_SERVICE_ACCOUNT_JSON: ${{ secrets.GOOGLE_PLAY_SERVICE_ACCOUNT_JSON }}
        with:
          projectPath: .
          targetPlatform: Android
          androidExportType: androidPackage
          androidKeystoreName: ${{ secrets.ANDROID_KEYSTORE_NAME }}
          androidKeystoreBase64: ${{ secrets.ANDROID_KEYSTORE_BASE64 }}
          androidKeystorePass: ${{ secrets.ANDROID_KEYSTORE_PASS }}
          androidKeyaliasName: ${{ secrets.ANDROID_KEYALIAS_NAME }}
          androidKeyaliasPass: ${{ secrets.ANDROID_KEYALIAS_PASS }}
          versioning: Custom
          version: 0.1.0
          buildMethod: ''

      - name: Upload Android Build
        uses: actions/upload-artifact@v4
        with:
          name: Android Build
          path: build/Android
          retention-days: 7

  buildiOS:
    name: Build iOS
    runs-on: macos-latest
    needs: test
    if: github.event.inputs.buildTarget == 'iOS' || github.event_name == 'push'
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

      - name: Build iOS Player
        id: buildiOS
        uses: game-ci/unity-builder@v4
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          projectPath: .
          targetPlatform: iOS
          iosSigningCert: ${{ secrets.IOS_SIGNING_CERT }}
          iosSigningProfileID: ${{ secrets.IOS_SIGNING_PROFILE_ID }}
          iosSigningTeamID: ${{ secrets.IOS_SIGNING_TEAM_ID }}
          versioning: Custom
          version: 0.1.0

      - name: Upload iOS Build
        uses: actions/upload-artifact@v4
        with:
          name: iOS Build
          path: build/iOS
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

---

## Próximos Passos

1. **Adicionar secrets no GitHub:**
   - Vá para: https://github.com/YanVGBdev/breach-ar/settings/secrets/actions
   - Adicione: `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`

2. **Disparar build:**
   - Vá para: https://github.com/YanVGBdev/breach-ar/actions
   - Clique em "Unity Build" > "Run workflow"

3. **Baixar APK:**
   - Após build completo, baixe o artifact "Android Build"
