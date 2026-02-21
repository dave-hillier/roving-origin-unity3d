# roving-origin-unity3d

A Unity3D proof-of-concept demonstrating the **roving origin** technique to work around floating-point precision loss at large world coordinates.

## The problem

Unity uses 32-bit floats for transform positions. A float has ~7 significant decimal digits, so at a world position of `(1000000, 0, 1000000)` the smallest representable step is around 0.06 units. Objects jitter, physics break, and rendering artifacts appear. This is a well-known limitation in any game engine that uses single-precision world coordinates.

## How roving origin works

Instead of letting the player wander far from the world origin, we:

1. **Store global positions as `decimal`** (28-29 significant digits) in a custom `Vector3g` struct, keeping full precision regardless of distance from the origin.
2. **Maintain a local origin offset** (`CoordinateRemap.LocalOrigin`) that maps global coordinates into a small neighborhood around `(0, 0, 0)` for Unity's float-based transforms.
3. **Shift everything** when the player moves too far from the local origin. All root transforms are translated by the offset, and `LocalOrigin` is updated. Because Unity objects always stay near `(0, 0, 0)`, float precision remains high.

The key insight is that relative distances between nearby objects are small and well-represented by floats — it's only the absolute position that causes trouble.

## Project structure

```
Assets/
  Vector3g.cs             High-precision 3D vector using decimal
  CoordinateRemap.cs      MonoBehaviour that detects and applies origin shifts
  SimpleKeyControls.cs    Player movement (arrow keys + Space/Shift for vertical)
  TestMapping.cs          Orbiting test cubes that demonstrate the system
  GlobalCoordScene.unity  Main demo scene
  StaticCube.prefab       Static environment objects
  GloballyPositionedCube.prefab  Objects with TestMapping attached
  Editor/
    Vector3gTests.cs      NUnit tests for Vector3g
```

### `Vector3g`

A value type storing `(x, y, z)` as `decimal`. Provides:

- Arithmetic operators: `+`, `-`, unary `-`, `*`
- Equality: `==`, `!=`, `IEquatable<Vector3g>`, proper `GetHashCode`
- Implicit conversion to/from `Vector3` (applies origin offset — see note below)
- Explicit methods `ToVector3()` (raw, no offset), `ToLocalVector3()`, and `FromLocalVector3()` for when you want to be clear about what conversion is happening

**Note on implicit conversions:** The implicit `Vector3g ↔ Vector3` operators apply `CoordinateRemap.LocalOrigin` automatically. This is convenient (`transform.position = myGlobalPos` just works) but means the result depends on hidden global state. When this matters, use the explicit named methods instead.

### `CoordinateRemap`

Attached to a scene object with `StaticObject` and `PositionedObject` prefab references. On `Start()` it spawns a grid of static cubes and three orbiting test cubes. On `Update()` it checks whether `DesiredLocalOrigin != LocalOrigin` and, if so, translates all root transforms by the delta. Only root transforms are moved — children follow their parents automatically.

### `SimpleKeyControls`

Moves the player with arrow keys (XZ plane) and Space/LeftShift (Y axis). When any axis of the Unity transform exceeds 3 units from center, it updates `CoordinateRemap.DesiredLocalOrigin` to trigger an origin shift.

### `TestMapping`

Orbits around a start position using sin/cos. Exposes `Move` (toggle motion on/off) and `FetchedGlobalPosition` (round-trip verification: transform position converted back to global coordinates). Compare `CurrentGlobalPosition` and `GlobalFromTransform` in the inspector to see whether precision is maintained.

## Running the project

1. Open the project in Unity 5.0+ (originally built with Unity 5.0.0f4).
2. Open `Assets/GlobalCoordScene.unity`.
3. Press Play.
4. Use arrow keys to move. Watch the console for origin shift messages.
5. Inspect any `GloballyPositionedCube` to compare `CurrentGlobalPosition` vs `GlobalFromTransform`.

## Running the tests

Open the Unity Test Runner (Window > Test Runner in Unity 5.3+) and run the Editor tests. The `Vector3gTests` suite covers arithmetic, equality, conversions, and precision.

## Limitations

- **Performance**: `FindObjectsOfType<Transform>()` is called on each origin shift. For scenes with many thousands of objects, a registration-based approach (objects register/deregister with the coordinator) would scale better.
- **Physics**: Rigidbodies and colliders are not explicitly handled during shifts. A production implementation would need to account for physics state.
- **Networking**: In a multiplayer game, all clients need to agree on global coordinates and handle origin shifts independently.
- **Unity version**: Targets Unity 5.0.0f4 / .NET 3.5. The concepts apply to any Unity version, but the API usage may need updating for modern Unity.
