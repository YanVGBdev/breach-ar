# Physics System Specification

**Status:** Implemented | **Last Updated:** 2026-08-03

---

## Overview

The Physics System handles all physics-based interactions in BreachAR, including orb launching, collision detection, material properties, and deterministic simulations.

---

## Systems Implemented

### 1. Launch System (PH-002, PH-011)

**File:** `Assets/_Project/Scripts/Physics/LaunchSystem.cs`

**Features:**
- Drag-based input for orb launching
- Configurable force calculation (min/max force)
- Cooldown between launches (anti-spam)
- Trajectory preview with physics simulation

**Configuration:**
```csharp
[System.Serializable]
public class LaunchConfig
{
    public float minDragDistance = 50f;
    public float maxDragDistance = 300f;
    public float pixelsPerUnit = 100f;
    public float minForce = 5f;
    public float maxForce = 20f;
    public float forceMultiplier = 1f;
    public float cooldown = 0.3f;
    public int trajectoryPoints = 30;
    public float trajectoryTimeStep = 0.05f;
}
```

**Acceptance Criteria:**
- ✅ Force applied consistently regardless of frame rate
- ✅ Cooldown prevents launch spam
- ✅ Drag distance clamped to max
- ✅ Trajectory preview reflects real physics

---

### 2. Physics Manager (PH-001, PH-003, PH-004)

**File:** `Assets/_Project/Scripts/Physics/PhysicsManager.cs`

**Features:**
- Layer configuration for collision filtering
- Physics materials per surface type (Wall, Furniture, Floor)
- Scalable gravity system
- Fixed timestep for deterministic physics

**Physics Materials:**
| Material | Bounciness | Dynamic Friction | Static Friction |
|----------|------------|------------------|-----------------|
| Wall | 0.7 | 0.4 | 0.4 |
| Furniture | 0.4 | 0.5 | 0.5 |
| Floor | 0.2 | 0.6 | 0.6 |

**Collision Layers:**
- Orb
- Fragment
- RealWorldSurface
- Core
- PowerUp
- ARPlane

**Acceptance Criteria:**
- ✅ Collisions occur only between allowed layers
- ✅ Bounciness varies correctly by surface type
- ✅ Gravity scale affects orb trajectory
- ✅ Fixed timestep ensures deterministic results

---

### 3. Orb-Fragment Collision (PH-006)

**File:** `Assets/_Project/Scripts/Physics/OrbFragmentCollision.cs`

**Features:**
- Collision detection between orbs and fragments
- Damage calculation with combo multiplier
- Impact VFX spawning
- Physics force application on fragments

**Damage Formula:**
```
TotalDamage = BaseDamage * ComboMultiplier
```

**Acceptance Criteria:**
- ✅ Damage applied correctly
- ✅ No double-hit in same frame
- ✅ Impact VFX spawned at collision point
- ✅ Fragment receives physics impulse

---

### 4. Fragment Death Sequence (PH-007)

**File:** `Assets/_Project/Scripts/Physics/FragmentDeathSequence.cs`

**Features:**
- Dissolve shader effect on death
- Death VFX spawning
- Sound effect playback
- Pool return after animation

**Dissolve Settings:**
- Duration: 0.5 seconds
- Curve: Ease-in-out
- Shader Property: `_DissolveAmount`

**Acceptance Criteria:**
- ✅ Dissolve effect plays smoothly
- ✅ VFX spawned at death position
- ✅ Sound effect plays
- ✅ Fragment returns to pool after sequence

---

### 5. Rift Closing Sequence (PH-008)

**File:** `Assets/_Project/Scripts/Physics/RiftClosingSequence.cs`

**Features:**
- Implosion animation
- Light fade effect
- Particle system trigger
- AR anchor release

**Implosion Settings:**
- Duration: 1 second
- Scale: Shrinks to 10% of original
- Light: Fades to zero

**Acceptance Criteria:**
- ✅ Implosion animation plays
- ✅ Light fades smoothly
- ✅ Particles trigger on close
- ✅ Anchor released for AR cleanup

---

### 6. Trajectory Preview (PH-015)

**File:** `Assets/_Project/Scripts/Gameplay/Orbs/TrajectoryPreview.cs`

**Features:**
- Real-time trajectory visualization
- Collision detection along path
- Color coding based on distance
- Configurable parameters

**Visual Settings:**
| Distance | Color | Width |
|----------|-------|-------|
| < 5m | White | 0.05 |
| 5-10m | Yellow | 0.08 |
| > 10m | Red | 0.05 |

**Acceptance Criteria:**
- ✅ Preview matches real trajectory
- ✅ Stops at collision points
- ✅ Color indicates danger level
- ✅ Smooth animation

---

## Deterministic Physics (PH-019)

**File:** `Assets/_Project/Scripts/Tests/EditMode/DeterministicPhysicsTests.cs`

**Test Coverage:**
1. Fixed timestep consistency
2. Gravity scaling verification
3. Trajectory simulation determinism
4. Projectile motion parabolic path
5. Physics material bounciness ranges
6. Collision matrix configuration
7. Frame-rate independent force application
8. Ricochet angle physical limits

**Acceptance Criteria:**
- ✅ All tests pass
- ✅ Results consistent across runs
- ✅ No floating-point drift

---

## Integration Points

### With Gameplay Systems
- **OrbController:** Receives launch force, handles ricochets
- **FragmentController:** Takes damage, triggers death sequence
- **RiftController:** Triggers closing sequence on HP=0
- **ComboSystem:** Provides multiplier for damage calculation
- **ScoreSystem:** Receives hit events for scoring

### With AR Systems
- **ARSessionService:** Provides surface detection for physics materials
- **CorePlacementService:** Uses physics for core positioning

### With Pool System
- **PoolManager:** Returns objects after death/closing sequences
- **ObjectPoolGeneric:** Generic pooling for VFX and projectiles

---

## Performance Considerations

| Metric | Target | Implementation |
|--------|--------|----------------|
| Physics CPU | < 2ms/frame | Fixed timestep, optimized collision checks |
| GC Alloc | 0 bytes/frame | Object pooling, no allocations in Update |
| Collision Checks | Optimized | Layer filtering, spatial partitioning |

---

## Known Limitations

1. **Depth API:** Currently uses placeholder for depth raycasting
2. **Mesh Colliders:** Dynamic mesh generation not yet optimized
3. **Multi-fragment collisions:** Single-frame batch processing pending

---

## Future Enhancements

1. **PH-005:** Ricochet damage falloff per bounce
2. **PH-012:** Particle system pooling optimization
3. **PH-013:** VFX Graph for GPU particles
4. **PH-016:** Area damage for Overcharge/Sobrecarga
5. **PH-020:** Mesh collider generation optimization

---

## Changelog

- 2026-08-03: Initial implementation of physics systems
- 2026-08-03: Added deterministic physics tests
- 2026-08-03: Integrated with pool system
