# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

This repo is a Unity coursework project ("InteligenciaArtificial" — Spanish for Artificial Intelligence) implementing classic game-AI techniques. The Unity project lives in `Desert Escape/`; everything else at the repo root is just the README.

- **Unity Editor version:** 2022.3.16f1 (see `Desert Escape/ProjectSettings/ProjectVersion.txt`) — open the project with this exact Editor version via Unity Hub.
- **Render pipeline:** URP.
- **All gameplay/AI code lives under `Desert Escape/Assets/`**, mostly as plain C# `MonoBehaviour`/POCO scripts (no asmdef split — everything compiles into `Assembly-CSharp`).

## Build, test, run

There is no CLI build, lint, or test setup — this is an Editor-driven Unity project:
- Open the project in Unity Hub / Unity Editor 2022.3.16f1, then use `File > Build Settings` to build, or Play mode to run.
- No automated test framework is wired up. `TestTree.cs` (`Assets/Scripts/Trees/`) and `TestLookUpTable.cs` (`Assets/Scripts/LookUpTable/`) are manual demo `MonoBehaviour`s you trigger with keypresses in Play mode, not unit tests — don't treat them as a CI-style test suite.
- `Desert Escape/Library`, `Logs`, and `UserSettings` are local Unity-generated caches (untracked by git aside from what's under `Desert Escape/.gitignore`) — don't hand-edit them.

## Architecture

The codebase demonstrates several independent game-AI patterns, implemented generically and then wired together per-entity (Player, Enemy, Drone) in each controller's `Start()`. When reading or extending any entity's AI, expect to find **both** an FSM and a decision tree driving it together — the decision tree's `ActionNode`s call `fsm.Transition(...)`, and both `_fsm.OnUpdate()` and `_root.Execute()` are invoked every `Update()`.

### Generic FSM (`Assets/Scripts/FSM/`)
`FSM<T>` / `IState<T>` / `State<T>` / `StateMono<T>` — a generic, reusable finite state machine keyed on an enum-like type `T`. States are created and wired with `AddTransition(input, state)` at init time (see `DroneController.InitializeFSM()`, `PlayerController.InitializeFSM()`, `EnemyController.InitializeFSM()` in `Assets/NEW SCRIPTS/`). `FSM<T>.Transition(input)` looks up the transition on the current state, calls `Sleep()`/`Enter()`, and swaps `_current`.

### Decision trees (`Assets/Scripts/Trees/`)
`ITreeNode` with two implementations: `QuestionNode` (a `Func<bool>` predicate branching to a true/false child node) and `ActionNode` (a `System.Action` leaf — typically an FSM transition closure). Trees are built once in `InitializedTree()` and re-evaluated via `_root.Execute()` every frame.

### Steering behaviors (`Assets/Scripts/Steerings/`)
`ISteering.GetDir()` is the common interface (`Seek`, `Flee`, `Pursuit`, `Evade`, `ObstacleAvoidanceV2`). Under `Steerings/FlockigScripts/` is a flocking system: `IBoid` (`Position`/`Front`) is implemented by entities (e.g. `Drone : Player, IBoid`), `IFlockingBehaviour.GetDir(boids, self)` is implemented by `AlignmentBehaviour`, `CohesionBehaviour`, `AvoidanceBehaviour`, `LeaderBehaviour`, `PredatorBehaviour`, and `FlockingManager` (itself an `ISteering`) aggregates them by querying nearby boids with `Physics.OverlapSphereNonAlloc` against a `boidMask` layer and summing each behaviour's contribution.

### Pathfinding (`Assets/NEW SCRIPTS/Pathfinding/`)
`AStar.Run<T>(start, getConnections, isSatisfies, getCost, heuristic, watchdog)` is a fully generic A* over delegates (not tied to any grid/node type), backed by `ScriptsExtras/PriorityQueue.cs`. `AgentController` and `ScriptsExtras/Node.cs` provide the grid-node type A* runs over in practice; `EnemyController.IncreaseWaypontIndex()` picks a new target node and calls `_agentController.RunAStar()`.

### Two parallel enemy AI implementations — know which one you're touching
- **`Assets/Scripts/Enemy/`** — the older/simpler design: a hand-rolled `StateMachine` (plain enum switch, not the generic `FSM<T>`) plus a separate `DecisionTree` component that does its own line-of-sight check and calls `stateMachine.SetState(...)` directly. Patrol movement (`PatrolState.cs`) drives a `NavMeshAgent` between waypoints and has its own local roulette-wheel wheel-selection for picking the next waypoint.
- **`Assets/NEW SCRIPTS/`** — the newer/generic design: `EnemyController` + `EnemyModel` wired through the generic `FSM<StatesEnum>` and `ITreeNode` decision tree, with states in `EnemyStates/` (`EnemyIdleState`, `EnemyChaseState`, `EnemyAttackState`, `EnemyPatrollState`, plus `DroneIdleState`/`DroneFollowState` reused by the drone). Line-of-sight is abstracted behind `ILineOfSight`/`LineOfSight.cs` (`CheckRange`/`CheckAngle`/`CheckView`), and patrol waypoint selection goes through `AgentController`/`AStar` instead of `NavMeshAgent`.

Before changing enemy behavior, check which folder the relevant prefab actually references (`Assets/NEW PREFABS/Enemy.prefab` vs. older prefabs) rather than assuming — both implementations are live in the project.

### Roulette-wheel selection appears in three independent places
`Assets/Scripts/Enemy/PatrolState.cs` (distance-weighted waypoint pick), `Assets/Scripts/Enemy/DecisionTree.cs` (alert-speed pick), and `Assets/NEW SCRIPTS/Random/MyRandoms.cs` (`MyRandoms.Roulette<T>(Dictionary<T,int>)`, a generic reusable version). Recent history (`Fixed Roulette Bug`, `Roulette Patrol State Rework`) has focused on `PatrolState.cs`'s implementation specifically — if fixing a roulette-selection bug, check whether it's local to that file or would also affect the other two copies.

### Player
Two parallel player implementations, same pattern as the enemy split:
- `Assets/Scripts/Player/Player.cs` implements `IPlayerModel` directly (`Move`/`LookDir` on a `Rigidbody`) and is also the base class `Drone` extends for flocking.
- `Assets/PlayerController2.cs` + `Assets/PlayerModel.cs` + `Assets/PlayerView2.cs` (repo root of `Assets/`, not under `Scripts/`) is an MVC split driven by the generic `FSM<PlayerStatesEnum>` and a decision tree (idle/walk/dead states).

### Misc
- `LookUpTable<T, U>` (`Assets/Scripts/LookUpTable/`) is a generic memoizing cache: wraps a `Func<T, U>` and caches results per key.
- `GameConditions/GameOver.cs` and `GameConditions/Win.cs` hold win/lose trigger logic.
- `Steerings/ObstacleAvoidanceV2.cs` and `NEW SCRIPTS/ILineOfSight.cs`/`LineOfSight.cs` are constructed with explicit params (angle, radius, layer masks) rather than read from `[SerializeField]` in most consumers — check the constructor call site for tuning values instead of the Inspector alone.
