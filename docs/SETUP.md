# BreachAR - Guia de Setup

## Pré-requisitos

- Unity 2022.3 LTS ou superior
- Android SDK (para Android)
- Xcode (para iOS)
- VContainer (instalado via Package Manager)

## 1. Instalação de Pacotes AR

Abra o Unity Package Manager (`Window > Package Manager`) e instale:

```
com.unity.xr.arfoundation
com.unity.xr.arcore (Android)
com.unity.xr.arkit (iOS)
com.unity.xr.openxr
```

## 2. Configuração de Plug-ins AR

Em `Edit > Project Settings > XR Plug-in Management`:

### Android
- ✅ ARCore
- ✅ OpenXR

### iOS
- ✅ ARKit
- ✅ OpenXR

## 3. Configuração de Layers

Em `Edit > Project Settings > Tags and Layers`:

| Layer | Nome |
|-------|------|
| 8 | RealWorldSurface |
| 9 | Orb |
| 10 | Fragment |
| 11 | Rift |
| 12 | Core |
| 13 | PowerUp |
| 14 | ARPlane |

## 4. Configuração de Tags

Adicione as seguintes tags:
- Orb
- Fragment
- Rift
- Core
- PowerUp
- VFX

## 5. Configuração de Physics

Em `Edit > Project Settings > Physics`:

### Layer Collision Matrix

| | RealWorld | Orb | Fragment | Rift | Core | PowerUp | ARPlane |
|---|-----------|-----|----------|------|------|---------|---------|
| **RealWorld** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Orb** | ✅ | ❌ | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Fragment** | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ | ✅ |
| **Rift** | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ✅ |
| **Core** | ✅ | ✅ | ✅ | ❌ | ❌ | ✅ | ✅ |
| **PowerUp** | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ | ✅ |
| **ARPlane** | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |

## 6. Configuração de Gravity

```csharp
Physics.gravity = new Vector3(0, -9.81f * 0.6f, 0); // 60% gravity for arcade feel
```

## 7. Criação de ScriptableObjects

Execute o wizard de setup: `BreachAR > Project Setup Wizard`

Ou crie manualmente:
1. Clique com botão direito em `Assets/_Project/ScriptableObjects/Configs`
2. Selecione `Create > BreachAR > AI > Difficulty Config`
3. Repita para outros configs

## 8. Configuração de PoolManager

No Inspector do PoolManager, adicione:

| Tag | Prefab | Initial | Max |
|-----|--------|---------|-----|
| Orb | OrbPrefab | 10 | 20 |
| Fragment | FragmentPrefab | 20 | 50 |
| Rift | RiftPrefab | 5 | 10 |
| PowerUp | PowerUpPrefab | 5 | 10 |
| VFX | VFXPrefab | 10 | 20 |

## 9. Configuração de VContainer

Os Scopes já estão configurados:
- `ProjectLifetimeScope` - Serviços globais
- `GameplayLifetimeScope` - Serviços de sessão

## 10. Build Settings

### Android
- Min SDK: 24 (Android 7.0)
- Target SDK: 33
- Scripting Backend: IL2CPP
- Architecture: ARM64

### iOS
- Min iOS: 12.0
- Architecture: ARM64

## 11. Testes

Execute testes via:
```
Unity Editor > Window > General > Test Runner
```

Ou via CLI:
```bash
Unity -batchmode -runTests -testPlatform EditMode -testResults results.xml
```

## 12. Troubleshooting

### AR não inicia
- Verifique se ARCore/ARKit está habilitado
- Teste em dispositivo real (AR não funciona no editor)

### Colisões não funcionam
- Verifique a Layer Collision Matrix
- Verifique se os objetos estão nos Layers corretos

### DI não funciona
- Verifique se os Scopes estão na cena
- Verifique se os serviços estão registrados

## Contato

Para suporte, consulte a documentação em `docs/` ou abra uma issue no repositório.
